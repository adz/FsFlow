---
title: IcedTasks
description: How FsFlow fits beside IcedTasks and task-oriented code.
---

# IcedTasks

This page shows how FsFlow can fit beside `IcedTasks`.

`IcedTasks` is a natural neighbor when the codebase already uses task computation expressions and cold or cancellable task helpers.

FsFlow does not need to replace that layer. It can add typed failure, environment threading, and boundary management where those concerns matter.

`IcedTasks` is especially interesting when you want task-native ergonomics and performance-sensitive helpers. FsFlow stays in the picture when you want to carry a typed result and an explicit environment together with the runtime shape.

## Shared Ground

The overlap is real:

- both libraries care about task-shaped boundaries
- both libraries care about delayed execution
- both libraries care about cancellation-aware execution

The difference is the model:

- `IcedTasks` focuses on task expression ergonomics
- `FsFlow.Net` focuses on orchestration, typed failure, and a named environment boundary

## Why The Pair Works

- `IcedTasks` keeps the task side fast and ergonomic
- `FsFlow.Net` keeps the boundary model explicit with `TaskFlow<'env, 'error, 'value>`
- the combination works well when you need task-native interop without giving up typed failures or environment/threaded context

## How To Combine Them

Use `IcedTasks` when the codebase already centers `task {}` and its adjacent task CE helpers.

Use `TaskFlow` when you want:

- a typed success/error boundary
- explicit environment threading
- `ColdTask` interop without losing the boundary shape

## Example

```fsharp
let loadConfig : ColdTask<AppConfig> =
    ColdTask.fromTaskFactory (fun () -> Task.FromResult { Prefix = "Hello" })

let workflow : TaskFlow<unit, string, string> =
    taskFlow {
        let! config = loadConfig
        let! prefix = TaskFlow.read _.Prefix
        return $"{prefix} {config.Prefix}"
    }
```

If a helper is already naturally task-shaped, keep it task-shaped. FsFlow can orchestrate the boundary, not force the task helper into a different semantic bucket.

## When To Prefer IcedTasks

Prefer `IcedTasks` when you mostly need:

- extra task CE variants
- cold or cancellable task helpers
- task-native ergonomics without typed orchestration

Prefer FsFlow when the boundary itself can carry the environment and typed result shape.
