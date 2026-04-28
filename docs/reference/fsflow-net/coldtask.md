---
title: ColdTask
description: Delayed task helpers used by FsFlow.Net.
---

# ColdTask

This page shows the delayed task helper surface used by `TaskFlow`.

Use this page when you want to represent task work without starting it too early.

## Core shape

`ColdTask<'value>` stores a cancellation-aware task factory.

- `ColdTask.run` starts the task with a `CancellationToken`.

## Creation helpers

- `ColdTask.create`
- `ColdTask.fromTaskFactory`
- `ColdTask.fromTask`
- `ColdTask.fromValueTaskFactory`
- `ColdTask.fromValueTaskFactoryWithoutCancellation`
- `ColdTask.fromValueTask`

## Cold versus hot

The key distinction is when the work starts:

- hot `Task` and `ValueTask` inputs may already be running before the boundary starts
- `ColdTask` starts when the boundary runs
- rerunning a `ColdTask` starts it again from scratch with the current cancellation token

Use the delayed shape when you want task work to stay restartable and cancellation-aware.

## Example

```fsharp
let cold =
    ColdTask.fromTaskFactory (fun () -> Task.FromResult 42)
```

## Source

- [TaskFlow.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow.Net/TaskFlow.fs)
