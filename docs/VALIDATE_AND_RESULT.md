---
title: Validate and Result
description: The graph-based model for pure checks, fail-fast results, and accumulating validation in FsFlow.
---

# Validate and Result

FsFlow uses a graph-based model for validation that scales from simple boolean checks to complex,
nested diagnostics.

## The Model

The validation model is built on three pillars:

- **`Check`**: Pure, reusable predicates that return `Result<'value, unit>`.
- **`Diagnostics`**: A structured graph that carries paths and typed errors.
- **`Validation`**: An accumulating result type that merges diagnostics during composition.

## Diagnostics Graph

Unlike traditional validation that uses a flat list of strings, FsFlow uses a `Diagnostics<'error>`
graph. This preserves the structure of your data and allows you to pinpoint exactly where an error
occurred.

```fsharp
type PathSegment =
    | Key of string
    | Index of int
    | Name of string

type Path = PathSegment list

type Diagnostic<'error> = { Path: Path; Error: 'error }

type Diagnostics<'error> =
    { Local: Diagnostic<'error> list
      Children: Map<PathSegment, Diagnostics<'error>> }
```

This graph is automatically merged when using the `validate {}` builder or `Validation.map2`.

## Pure Checks

The `Check` module provides pure predicates. These are "half-baked" results that use `unit` for
failure. You bridge them into your application errors using `Result.mapErrorTo`:

```fsharp
open FsFlow

type AppError = | Required | InvalidEmail

let requireName name =
    name |> Check.notBlank |> Result.mapErrorTo Required

let requireEmail email =
    email |> Check.notBlank |> Result.mapErrorTo InvalidEmail
```

## Fail-fast with `result {}`

When you want to stop at the first error, use the `result {}` builder. This works with standard
`Result<'v, 'e>` types:

```fsharp
let validateUser cmd =
    result {
        let! name = requireName cmd.Name
        let! email = requireEmail cmd.Email
        return { cmd with Name = name; Email = email }
    }
```

## Accumulating with `validate {}`

When you want to collect all errors across sibling fields, use the `validate {}` builder with the
`and!` keyword. This builder works with the `Validation<'v, 'e>` type:

```fsharp
let validateUserAccumulated cmd =
    validate {
        let! name = requireName cmd.Name
        and! email = requireEmail cmd.Email
        return { cmd with Name = name; Email = email }
    }
```

The `and!` syntax triggers the applicative merging of the `Diagnostics` graph.

### Example Output

```fsharp
let cmd = { Name = ""; Email = "" }
let result = validateUserAccumulated cmd

// result = Error { 
//   Local = []; 
//   Children = Map [
//     Name "Name", { Local = [ { Path = []; Error = "Name required" } ]; Children = Map [] }
//     Name "Email", { Local = [ { Path = []; Error = "Email required" } ]; Children = Map [] }
//   ] 
// }
```

## Bridging into Workflows

Every workflow family (`Flow`, `AsyncFlow`, `TaskFlow`) can bind `Result` directly. This means your
pure validation logic lifts unchanged into your effectful code:

```fsharp
let registerUser userId =
    taskFlow {
        let! user = loadUser userId
        let! validName = requireName user.Name
        // ... rest of the flow
    }
```

## Diagnostics Helpers

The `Diagnostics` module provides helpers for working with the graph:

- `Diagnostics.empty`: Create an empty graph.
- `Diagnostics.singleton`: Create a graph with a single error at the root.
- `Diagnostics.merge`: Merge two graphs together.
- `Diagnostics.flatten`: Turn the graph into a flat `Diagnostic<'error> list`.

## Advanced Graph Manipulation

The `Diagnostics` graph is not just for collecting flat errors; it's designed to represent
the hierarchical nature of domain models.

### Building Nested Errors

When validating a nested structure, you can wrap errors in a `PathSegment`:

```fsharp
let validateAddress address =
    validate {
        let! street = address.Street |> Check.notBlank |> Result.mapErrorTo "Street required"
        let! city = address.City |> Check.notBlank |> Result.mapErrorTo "City required"
        return { Street = street; City = city }
    }

let validateUser user =
    validate {
        let! name = user.Name |> Check.notBlank |> Result.mapErrorTo "Name required"
        and! address = 
            validateAddress user.Address
            |> Validation.mapError (fun diag -> 
                { Local = []; Children = Map.ofList [ PathSegment.Key "address", diag ] })
        return { Name = name; Address = address }
    }
```

### Path Reconstruction in `flatten`

When you call `Diagnostics.flatten`, the library recursively walks the tree and 
reconstructs the full path for each error. An error nested under `Key "address"` 
and then `Name "Street"` will have the path `[Key "address"; Name "Street"]`.

This allows your UI or API to report errors in a structured format (like JSON pointers):

```fsharp
let report errors =
    errors 
    |> Diagnostics.flatten
    |> List.map (fun d -> 
        let pathStr = d.Path |> List.map (function Key k -> k | Name n -> n | Index i -> string i) |> String.concat "."
        $"{pathStr}: {d.Error}")

// let errors = (validateUser { Name = ""; Address = { Street = ""; City = "" } }).Error
// report errors = [
//   "Name: Name required";
//   "address.Street: Street required";
//   "address.City: City required"
// ]
```

## Summary

- Use `Check` for reusable, pure predicates.
- Use `Result` and `result {}` for fail-fast logic.
- Use `Validation` and `validate {}` for structured error accumulation.
- Bind them directly into `Flow`, `AsyncFlow`, or `TaskFlow` as needed.
