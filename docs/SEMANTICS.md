# Semantics

Read this page when you want the exact execution model of `Flow`, `AsyncFlow`, `TaskFlow`, and `ColdTask`.

Task-oriented semantics on this page refer to the `FsFlow.Net` package.
The core `FsFlow` package keeps only sync and `Async` concepts in its public surface.

## Success And Typed Failure

- `Flow.succeed value` returns `Ok value`
- `AsyncFlow.succeed value` returns `Ok value`
- `TaskFlow.succeed value` returns `Ok value`
- `fail` produces the typed `Error`
- `map` only transforms successful values
- `mapError` only transforms typed failures
- `tapError` runs only on typed failure and preserves the original error when the tap succeeds
- `orElse` switches to a fallback workflow when the first workflow returns a typed failure

## Cold By Default

All three workflow families are cold.
Building a workflow does not run it.

Rerun behavior:

- `Flow` reruns from scratch each time you call `Flow.run`
- `AsyncFlow` reruns from scratch each time you call `AsyncFlow.run` or `AsyncFlow.toAsync`
- `TaskFlow` reruns from scratch each time you call `TaskFlow.run` or `TaskFlow.toTask`

The `delay` combinator preserves that behavior in each family.

## Execution Is Explicit

Run each workflow family with its own execution function:

- `Flow.run env flow`
- `AsyncFlow.toAsync env flow`
- `TaskFlow.toTask env cancellationToken flow`

The runtime shape is part of the public contract.
That is why the library keeps three families instead of forcing every workflow through one wrapper.

## Exceptions

Each family exposes `catch` to convert exceptions into typed errors:

- `Flow.catch`
- `AsyncFlow.catch`
- `TaskFlow.catch`

This handles exceptions that occur while the workflow is being executed.
Typed failures still stay in `Result`.

## Environments

Each family reads dependencies explicitly:

- `env` reads the whole environment
- `read` projects one dependency
- `localEnv` runs a smaller workflow inside a larger environment

The environment semantics are aligned across all three families.

## Pairing And Small Composition

Each family also exposes low-ceremony helpers for common composition shapes:

- `zip` runs two workflows in sequence and returns a tuple
- `map2` runs two workflows in sequence and combines both successful values with a mapper
- both helpers short-circuit on the first typed failure

These helpers are useful when a full computation expression would add more ceremony than value.

## Task Temperature

`TaskFlow` and the `.NET` extensions for `asyncFlow {}` distinguish between:

- already-started task values such as `Task<'value>` and `ValueTask<'value>`
- delayed task work represented by `ColdTask<'value>`

`ColdTask<'value>` means:

```fsharp
CancellationToken -> Task<'value>
```

That distinction matters because reruns behave differently:

- rerunning a workflow that binds a started `Task` or `ValueTask` re-awaits the same started work
- rerunning a workflow that binds a `ColdTask` calls the factory again

It also affects cancellation:

- a started `Task` or `ValueTask` is already running before the workflow executes
- the current workflow `CancellationToken` cannot be injected into that already-started work
- a `ColdTask` starts when the workflow runs, so the current workflow `CancellationToken` can be passed in

Use a hot task input only when reusing the same already-started work is the behavior you want.
Use `ColdTask` when the effect should start at workflow execution time, rerun from scratch, or observe the runtime cancellation token.

Example with a started task:

```fsharp
let started = Task.FromResult 42

let workflow : TaskFlow<unit, string, int> =
    taskFlow {
        let! value = started
        return value
    }
```

Each run re-awaits `started`.
It does not create a new task.

Example with a `ColdTask`:

```fsharp
let readValue : ColdTask<int> =
    ColdTask(fun cancellationToken ->
        Task.FromResult 42)

let workflow : TaskFlow<unit, string, int> =
    taskFlow {
        let! value = readValue
        return value
    }
```

Each run calls the `ColdTask` factory again and passes in the current workflow cancellation token.

## Runtime Helpers

The shared runtime helpers live on `Flow.Runtime` and `AsyncFlow.Runtime`:

- `cancellationToken`
- `catchCancellation`
- `ensureNotCanceled`
- `sleep`
- `log`
- `logWith`
- `useWithAcquireRelease`
- `timeout`
- `retry`

Use `Flow.Runtime` for synchronous workflows and `AsyncFlow.Runtime` for async workflows.

The `FsFlow.Net` package also provides `TaskFlow.Runtime` for task-native workflows:

- `TaskFlow.Runtime.cancellationToken`
- `TaskFlow.Runtime.catchCancellation`
- `TaskFlow.Runtime.ensureNotCanceled`
- `TaskFlow.Runtime.sleep`
- `TaskFlow.Runtime.log`
- `TaskFlow.Runtime.logWith`
- `TaskFlow.Runtime.useWithAcquireRelease`
- `TaskFlow.Runtime.timeout`
- `TaskFlow.Runtime.retry`

Use `TaskFlow.Runtime` when the public boundary is task-based and the helper belongs in task execution.

## Validation Helpers

`FsFlow.Validate` provides pure `Result<'value, unit>` checks for booleans, options, value options, nulls, collections, equality, and strings.
Use `Validate.orElse` or `Validate.orElseWith` to attach a typed error after the pure validation step.

When the error value itself needs environment or effectful evaluation, use the bridge helpers on `Flow`, `AsyncFlow`, or `TaskFlow`.

## Family Direction

The workflow families intentionally compose upward:

- `AsyncFlow` can lift `Flow`
- `TaskFlow` can lift `Flow`
- `TaskFlow` can lift `AsyncFlow`

Keep the smallest honest workflow at each boundary, then lift it only when the outer runtime actually changes.

## What The Tests Cover

The test suite currently verifies:

- sync, async, and task execution
- rerun behavior for `delay`
- direct binding across the supported wrapper shapes
- `ColdTask` hot and cold adaptation behavior
- cancellation-token propagation into `ColdTask`
- environment projection through `localEnv`
- option and value-option behavior across all builders

## Next

Read [`docs/GETTING_STARTED.md`](./GETTING_STARTED.md) for the workflow-family overview,
[`docs/TASK_ASYNC_INTEROP.md`](./TASK_ASYNC_INTEROP.md) for the direct binding surface,
or [`src/FsFlow/Flow.fs`](../src/FsFlow/Flow.fs) and [`src/FsFlow.Net/TaskFlow.fs`](../src/FsFlow.Net/TaskFlow.fs)
for the full API surface.
