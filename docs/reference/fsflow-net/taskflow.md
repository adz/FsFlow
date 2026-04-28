---
title: TaskFlow
description: The task-oriented workflow surface in FsFlow.Net.
---

# TaskFlow

This page shows the task-oriented boundary family in one place: the `TaskFlow<'env, 'error, 'value>` type, the `TaskFlow` module, and the task builder entry point.

Use this page when the runtime boundary is task-based or when cancellation is part of the honest shape of the workflow.

## Core shape

`TaskFlow<'env, 'error, 'value>` stores a function from environment and cancellation token to `Task<Result<'value, 'error>>`.

- `TaskFlow.run` executes the boundary.
- `TaskFlow.toTask` exposes the task result directly.

## What you can do

`TaskFlow` is the task-native branch of the FsFlow family:

- lift from `Flow` and `AsyncFlow`
- build from `Task`, `ValueTask`, `Async`, `ColdTask`, and `Result`
- keep cancellation visible all the way through execution
- compose with the same result-first flow as the other families
- bridge to task-native runtime concerns like timeout, retry, and cleanup

## Builder entry point

The `taskFlow {}` builder is the readable orchestration layer for task-based code.

- use it when the runtime boundary is genuinely task-shaped
- use `TaskFlow` members for the real primitives
- keep `TaskFlowBuilder` as implementation detail, not a primary landing page

## Create and run

The creation helpers mirror the sync and async families:

- `TaskFlow.run` executes the task boundary with environment and cancellation.
- `TaskFlow.toTask` exposes the raw task result.
- `TaskFlow.succeed` and `TaskFlow.fail` create immediate task boundaries.
- `TaskFlow.fromResult` lifts a plain `Result<'value, 'error>`.
- `TaskFlow.fromOption` and `TaskFlow.fromValueOption` lift optional values with an explicit error.
- `TaskFlow.fromFlow` and `TaskFlow.fromAsyncFlow` bridge the other FsFlow families into task shape.
- `TaskFlow.fromTask` and `TaskFlow.fromTaskResult` bridge delayed task work into the task boundary.
- `TaskFlow.env` reads the full environment.
- `TaskFlow.read` projects a dependency from the environment.

## Compose

The composition helpers preserve the same typed failure model:

- `TaskFlow.map` transforms the success value.
- `TaskFlow.bind` sequences the next task-bound step.
- `TaskFlow.tap` and `TaskFlow.tapError` observe success or failure without changing them.
- `TaskFlow.mapError` reshapes the failure value.
- `TaskFlow.catch` turns an exception into a typed failure.
- `TaskFlow.orElse` and its flow/task bridges lift validation into task-shaped runtime work.
- `TaskFlow.zip` and `TaskFlow.map2` combine independent task flows.

## Environment and lifecycle

Task-based boundaries still keep dependency access explicit:

- `TaskFlow.env` exposes the current environment value.
- `TaskFlow.read` projects a dependency from the environment.
- `TaskFlow.localEnv` narrows or reshapes the environment for a subflow.
- `TaskFlow.delay` defers construction until execution time.

## Collections

The traversal helpers keep task composition honest:

- `TaskFlow.traverse` runs a task-flow-producing function over each item.
- `TaskFlow.sequence` turns a collection of task flows into a task flow of a collection.

## Runtime helpers

The task-oriented helper surface covers the operational boundaries:

- `TaskFlow.Runtime.catchCancellation` maps cancellation into a typed failure.
- `TaskFlow.Runtime.useWithAcquireRelease` scopes cleanup for task-based resources.
- `TaskFlow.Runtime.timeout`, `timeoutToOk`, `timeoutToError`, and `timeoutWith` bound task work in time.
- `TaskFlow.Runtime.retry` repeats execution using `RetryPolicy<'error>`.
- cancellation-aware reads and delay helpers remain explicit rather than implicit.

## Interop

The `TaskFlow` surface is where the other families and task-native helpers meet:

- `TaskFlow.fromFlow` and `TaskFlow.fromAsyncFlow` bridge the other FsFlow families.
- `TaskFlow.fromTask` and `TaskFlow.fromTaskResult` bridge delayed task work.
- `TaskFlow.orElseTask`, `TaskFlow.orElseAsync`, and the flow/task-flow bridges lift validation when the error itself needs a task-shaped boundary.

## Example

```fsharp
let workflow : TaskFlow<unit, string, int> =
    taskFlow {
        let! value = TaskFlow.fromTaskResult (Task.FromResult (Ok 42))
        return value
    }
```

## Member map

If you want the short version of the page, reach for:

- `TaskFlow.succeed` and `TaskFlow.fail` to create immediate boundaries
- `TaskFlow.fromResult`, `TaskFlow.fromOption`, and `TaskFlow.fromValueOption` to lift ordinary values
- `TaskFlow.fromFlow`, `TaskFlow.fromAsyncFlow`, `TaskFlow.fromTask`, and `TaskFlow.fromTaskResult` for task bridges
- `TaskFlow.read` and `TaskFlow.env` for explicit environment access
- `TaskFlow.map`, `TaskFlow.bind`, `TaskFlow.tap`, `TaskFlow.tapError`, and `TaskFlow.orElse` for composition
- `TaskFlow.traverse` and `TaskFlow.sequence` for lists and sequences
- `TaskFlow.Runtime.*` for cancellation, timeout, retry, and resource management

## Source-Lifted Notes

The source comments on `TaskFlow` are intentionally minimal. The important ones are:

- `TaskFlow.run` is execution with both environment and cancellation.
- `TaskFlow.toTask` exposes the underlying task result.
- `TaskFlow.succeed`, `fail`, `fromResult`, `fromOption`, and `fromValueOption` are the direct creation path.
- `TaskFlow.fromFlow`, `fromAsyncFlow`, `fromTask`, and `fromTaskResult` are the bridge path from other runtimes.
- `TaskFlow.read` keeps environment access explicit.
- `TaskFlow.Runtime.*` keeps operational behavior separate from the main composition surface.

## Source

- [TaskFlow.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow.Net/TaskFlow.fs)
