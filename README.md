# FsFlow

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="docs/content/img/fsflow-readme-dark.svg">
  <source media="(prefers-color-scheme: light)" srcset="docs/content/img/fsflow-readme-light.svg">
  <img alt="FsFlow" src="docs/content/img/fsflow-readme-light.svg" width="160">
</picture>

F# application workflows that compose with normal `Result`, `Async`, and `.NET Task`.

[![ci](https://github.com/adz/FsFlow/actions/workflows/ci.yml/badge.svg)](https://github.com/adz/FsFlow/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/FsFlow.svg)](https://www.nuget.org/packages/FsFlow)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

Docs: [adz.github.io/FsFlow](https://adz.github.io/FsFlow/)

When one F# use case starts mixing `Result`, `async {}`, `.NET Task`, and dependency management,
the code often stops reading like the happy path.

## Why This Exists In F#

Most real F# application code ends up mixing:

- dependencies passed through several layers, whether as one app environment or explicit feature dependencies
- `Result` for expected business errors
- `Async` or `.NET Task` for IO

That often turns into one of these shapes:

```fsharp
AppEnv -> Result<'value, 'error>
AppEnv -> Async<Result<'value, 'error>>
AppEnv -> CancellationToken -> Task<Result<'value, 'error>>
```

FsFlow gives those use cases three explicit, related workflow types instead of one ad hoc wrapper per
application:

- `Flow<'env, 'error, 'value>` for synchronous workflows
- `AsyncFlow<'env, 'error, 'value>` for `Async`-based workflows in the core `FsFlow` package
- `TaskFlow<'env, 'error, 'value>` for `.NET Task`-based workflows in `FsFlow.Net`

Package split:

- `FsFlow` contains `Flow`, `AsyncFlow`, `flow {}`, and `asyncFlow {}`
- `FsFlow.Net` adds `TaskFlow`, `taskFlow {}`, and task-oriented interop

## Choose The Workflow Family

Use:

- `Flow` when the workflow itself is synchronous and you want no runtime cancellation token in the representation
- `AsyncFlow` when the workflow is naturally `Async`-based or you want to stay in the core package
- `TaskFlow` when the workflow is naturally `.NET Task`-based or task interop is central to the use case

The families stay aligned on purpose:

- they are all cold
- they all keep expected failures in `Result`
- they all read dependencies from an explicit environment
- `AsyncFlow` can lift `Flow`
- `TaskFlow` can lift both `Flow` and `AsyncFlow`

For task interop, treat started `Task` and `ValueTask` inputs as hot values:

- rerunning the workflow re-awaits the same started work
- the workflow cancellation token does not get injected into that already-started work

Use `ColdTask<'value>` when task work should start at workflow execution time, rerun from scratch, and observe the runtime cancellation token.

## Before And After

Before:

```fsharp
let handle (deps: UserDeps) userId =
    async {
        let! loaded = deps.LoadName userId |> Async.AwaitTask

        match loaded with
        | Error error ->
            return Error (GatewayFailed error)
        | Ok loadedName ->
            match validateName loadedName with
            | Error error ->
                return Error error
            | Ok validName ->
                return Ok $"{deps.Prefix} {validName}"
    }
```

After, if the use case is task-oriented:

```fsharp
let handle userId : TaskFlow<UserDeps, AppError, string> =
    taskFlow {
        let! loadName = TaskFlow.read _.LoadName
        let! loadedName = loadName userId
        let! validName = validateName loadedName
        let! prefix = TaskFlow.read _.Prefix
        return $"{prefix} {validName}"
    }
```

This is the same application flow without the plumbing taking over the happy path.

## What It Actually Is

FsFlow is a small, focused F# library built around composable workflows:

- explicit environment requirements
- typed failures via `Result`
- a sync family, an `Async` family, and a `.NET Task` family

The point is to keep mixed application code in ordinary F# rather than spreading wrapper-shape
adaptation across helper modules and ad hoc computation expressions.

The execution boundary is explicit for each family:

- `Flow.run env flow`
- `AsyncFlow.toAsync env flow`
- `TaskFlow.toTask env cancellationToken flow`

It does not replace F# `Async`, `.NET Task`, or `Result`.
It gives you a smaller, more consistent way to compose them in application code.

## Full Code

### `Flow`

```fsharp
type AppEnv =
    { Prefix: string }

type AppError =
    | MissingName

let validateName (name: string) =
    if System.String.IsNullOrWhiteSpace name then
        Error MissingName
    else
        Ok name

let greet input : Flow<AppEnv, AppError, string> =
    flow {
        let! validName = validateName input
        let! prefix = Flow.read _.Prefix
        return $"{prefix} {validName}"
    }

let flowResult =
    greet "Ada"
    |> Flow.run { Prefix = "Hello" }
```

### `AsyncFlow`

```fsharp
type AsyncEnv =
    { Prefix: string
      LoadName: int -> Async<string> }

let greetAsync userId : AsyncFlow<AsyncEnv, AppError, string> =
    asyncFlow {
        let! loadName = AsyncFlow.read _.LoadName
        let! loadedName = loadName userId
        let! validName = validateName loadedName
        let! prefix = AsyncFlow.read _.Prefix
        return $"{prefix} {validName}"
    }

let asyncResult =
    greetAsync 42
    |> AsyncFlow.toAsync
        { Prefix = "Hello"
          LoadName = fun _ -> async { return "Ada" } }
    |> Async.RunSynchronously
```

### `TaskFlow`

```fsharp
type TaskEnv =
    { Prefix: string
      LoadName: int -> System.Threading.Tasks.Task<string> }

let greetTask userId : TaskFlow<TaskEnv, AppError, string> =
    taskFlow {
        let! loadName = TaskFlow.read _.LoadName
        let! loadedName = loadName userId
        let! validName = validateName loadedName
        let! prefix = TaskFlow.read _.Prefix
        return $"{prefix} {validName}"
    }

let taskResult =
    greetTask 42
    |> TaskFlow.toTask
        { Prefix = "Hello"
          LoadName = fun _ -> System.Threading.Tasks.Task.FromResult "Ada" }
        System.Threading.CancellationToken.None
    |> Async.AwaitTask
    |> Async.RunSynchronously
```

## Where To Use It

Use FsFlow at the effectful application boundary:

- handlers
- use cases
- service orchestration
- infrastructure-facing application services

Keep the domain plain F# where possible:

- plain functions
- plain domain types
- plain `Result` when that already reads well

## Supported Architectural Styles

FsFlow supports three valid architectural styles:

- Booted App Environment
- Explicit Dependencies + Context
- Standard `.NET` AppHost + DI

The library does not require one application shape.
Choose the style that fits your codebase and team.

Read [`docs/ARCHITECTURAL_STYLES.md`](docs/ARCHITECTURAL_STYLES.md) for examples and trade-offs.

## When FsFlow Fits Well

FsFlow is a good fit when:

- a workflow needs 2 to 5 dependencies
- validation, IO, and error translation all belong in one use case
- you want expected failures in the type rather than scattered exception handling
- you want one workflow family that matches the real boundary: sync, `Async`, or `.NET Task`

FsFlow is usually not worth it when:

- the code is mostly pure
- plain `Result` already reads well
- a direct `Task<'T>` or `Async<'T>` boundary is already the clearest option

## Learn The Library In This Order

1. [`docs/GETTING_STARTED.md`](docs/GETTING_STARTED.md)
2. [`docs/TINY_EXAMPLES.md`](docs/TINY_EXAMPLES.md)
3. [`docs/ARCHITECTURAL_STYLES.md`](docs/ARCHITECTURAL_STYLES.md)
4. [`docs/WHY_FSFLOW.md`](docs/WHY_FSFLOW.md)
5. [`docs/TASK_ASYNC_INTEROP.md`](docs/TASK_ASYNC_INTEROP.md)
6. [`docs/FSTOOLKIT_MIGRATION.md`](docs/FSTOOLKIT_MIGRATION.md)
7. [`docs/ENV_SLICING.md`](docs/ENV_SLICING.md)
8. [`docs/SEMANTICS.md`](docs/SEMANTICS.md)
9. [`examples/README.md`](examples/README.md)
10. [`docs/TROUBLESHOOTING_TYPES.md`](docs/TROUBLESHOOTING_TYPES.md)
11. [`src/FsFlow/Flow.fs`](src/FsFlow/Flow.fs)
12. [`src/FsFlow.Net/TaskFlow.fs`](src/FsFlow.Net/TaskFlow.fs)

## Compatibility

### AOT Verified

NativeAOT is verified in this repo through a small publish-and-run probe application.

### .NET only - no Fable story
