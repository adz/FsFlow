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

## Summary

- Use `Check` for reusable, pure predicates.
- Use `Result` and `result {}` for fail-fast logic.
- Use `Validation` and `validate {}` for structured error accumulation.
- Bind them directly into `Flow`, `AsyncFlow`, or `TaskFlow` as needed.
