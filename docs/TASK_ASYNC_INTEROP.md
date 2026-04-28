# Task And Async Interop

Read this page when you already know you need async work and want to choose the right FsFlow workflow family and binding surface.

Task-oriented APIs on this page belong in the `FsFlow.Net` package.
The core `FsFlow` package keeps only sync and `Async` concepts.

## The Main Rule

Choose the workflow family that matches the runtime shape of the workflow itself:

- `Flow` for synchronous workflows
- `AsyncFlow` for `Async`-based workflows
- `TaskFlow` for `.NET Task`-based workflows

Use interop to cross boundaries.
Do not keep a task-oriented workflow in `Flow` just because a helper can be adapted.

## Which Shapes Bind Directly

### `flow {}`

The sync builder binds:

- `Flow<'env, 'error, 'value>`
- `Result<'value, 'error>`
- `Option<'value>` when the error type is `unit`
- `ValueOption<'value>` when the error type is `unit`

Use `flow {}` when the workflow body is synchronous.

### `asyncFlow {}`

In the core `FsFlow` package, `asyncFlow {}` binds:

- `Flow<'env, 'error, 'value>`
- `AsyncFlow<'env, 'error, 'value>`
- `Async<'value>`
- `Async<Result<'value, 'error>>`
- `Result<'value, 'error>`
- `Option<'value>` when the error type is `unit`
- `ValueOption<'value>` when the error type is `unit`

When `FsFlow.Net` is referenced, `asyncFlow {}` also binds task-oriented inputs such as:

- `Task<'value>`
- `Task<Result<'value, 'error>>`
- `ValueTask<'value>`
- `ValueTask<Result<'value, 'error>>`
- `ColdTask<'value>`
- `ColdTask<Result<'value, 'error>>`

Example:

```fsharp
let workflow : AsyncFlow<unit, string, string> =
    asyncFlow {
        let! a = async { return "a" }
        let! b = async { return Ok "b" }
        return a + b
    }
```

### `taskFlow {}`

`taskFlow {}` binds:

- `Flow<'env, 'error, 'value>`
- `AsyncFlow<'env, 'error, 'value>`
- `TaskFlow<'env, 'error, 'value>`
- `Async<'value>`
- `Async<Result<'value, 'error>>`
- `Task<'value>`
- `Task<Result<'value, 'error>>`
- `ValueTask<'value>`
- `ValueTask<Result<'value, 'error>>`
- `ColdTask<'value>`
- `ColdTask<Result<'value, 'error>>`
- `Result<'value, 'error>`
- `Option<'value>` when the error type is `unit`
- `ValueOption<'value>` when the error type is `unit`

Example:

```fsharp
let workflow : TaskFlow<unit, string, int> =
    taskFlow {
        let! value = Task.FromResult 42
        return value
    }
```

## When To Choose `AsyncFlow`

Prefer `AsyncFlow` when:

- the outer application code already uses `Async`
- you want to stay in core `FsFlow`
- `Async` is the honest execution model for the workflow

Use `AsyncFlow.toAsync` to run it.

## When To Choose `TaskFlow`

Prefer `TaskFlow` when:

- the public boundary is `.NET Task`
- task interop is central to the workflow
- runtime cancellation belongs in execution

Use `TaskFlow.toTask` to run it.
Use `Flow.Runtime` or `AsyncFlow.Runtime` for shared operational helpers like `sleep`, `timeout`, `retry`, and `useWithAcquireRelease`.
Use `TaskFlow.Runtime` when you want the same helpers in a task-native shape.
Use `FsFlow.Validate` for pure `Result<'value, unit>` validation, then bridge into `Flow`, `AsyncFlow`, or `TaskFlow` only when the error value itself needs effectful evaluation.

## `ColdTask<'value>`

`ColdTask<'value>` is the delayed task shape used by `FsFlow.Net`:

```fsharp
CancellationToken -> Task<'value>
```

Use it when a helper should stay task-based but delayed until the workflow runs.

Example:

```fsharp
let readAll path : ColdTask<string> =
    ColdTask(fun ct -> System.IO.File.ReadAllTextAsync(path, ct))

let workflow : TaskFlow<unit, string, string> =
    taskFlow {
        let! text = readAll "config.json"
        return text
    }
```

## Hot `Task` And `ValueTask` Versus `ColdTask`

Binding a started `Task<'value>` or `ValueTask<'value>` is not the same as binding a `ColdTask<'value>`.

Started task inputs are hot:

- the work may already be running before the workflow starts
- rerunning the workflow re-awaits the same started work
- the workflow cancellation token cannot be pushed into that work after the fact

`ColdTask` inputs are cold:

- the work starts when the workflow runs
- rerunning the workflow starts the work again from scratch
- the current workflow cancellation token is passed into the `ColdTask` factory

Use a direct `Task` or `ValueTask` bind when you intentionally want to reuse existing started work.
Use `ColdTask` when the task helper is part of the workflow effect and should stay delayed, restartable, and cancellation-aware.

Example with a started task:

```fsharp
let started = Task.FromResult 42

let workflow : TaskFlow<unit, string, int> =
    taskFlow {
        let! value = started
        return value
    }
```

Example with delayed task work:

```fsharp
let loadValue : ColdTask<int> =
    ColdTask(fun cancellationToken ->
        Task.FromResult 42)

let workflow : TaskFlow<unit, string, int> =
    taskFlow {
        let! value = loadValue
        return value
    }
```

Read [`docs/SEMANTICS.md`](./SEMANTICS.md) when you need the exact rerun and cancellation behavior.

## Choosing Quickly

Use:

- `Flow` when the workflow is sync
- `AsyncFlow` when the workflow is `Async`-first
- `TaskFlow` when the workflow is `Task`-first
- `ColdTask<'value>` when a task helper should stay delayed, rerunnable, and cancellable at run time

If you are unsure between `AsyncFlow` and `TaskFlow`, choose the one that matches the boundary you
need to return and run today.

## Next

Read [`docs/GETTING_STARTED.md`](./GETTING_STARTED.md) for the family overview,
[`docs/TROUBLESHOOTING_TYPES.md`](./TROUBLESHOOTING_TYPES.md) when the compiler complains,
and [`docs/SEMANTICS.md`](./SEMANTICS.md) for the exact runtime behavior.
