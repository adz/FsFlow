---
title: Interop
description: Task and async interop helpers for FsFlow.Net.
---

# Interop

This page shows the interop helpers that bridge task-based boundaries to sync and async boundaries when that is the honest runtime shape.

Use this page when you need to move between `Task`, `ValueTask`, `Async`, `ColdTask`, `Flow`, `AsyncFlow`, and `TaskFlow`.

## TaskFlow bridges

- `TaskFlow.fromFlow`
- `TaskFlow.fromAsyncFlow`
- `TaskFlow.orElseTask`
- `TaskFlow.orElseAsync`
- `TaskFlow.orElseFlow`
- `TaskFlow.orElseAsyncFlow`
- `TaskFlow.orElseTaskFlow`

## Builder extensions

The builder extension modules are the supported customization surface:

- `TaskFlowBuilderExtensions`
- `AsyncFlowBuilderExtensions`

The builder types themselves are plumbing and are intentionally not part of the docs narrative.

## Bridge map

The task package surface gives you these bridge points:

- `TaskFlow.fromFlow` and `TaskFlow.fromAsyncFlow` to lift sync and async boundaries into the task family
- `TaskFlow.fromTask` and `TaskFlow.fromTaskResult` when the task is already the right runtime shape
- `TaskFlow.orElseTask`, `TaskFlow.orElseAsync`, `TaskFlow.orElseFlow`, `TaskFlow.orElseAsyncFlow`, and `TaskFlow.orElseTaskFlow` for effectful fallback shapes
- `TaskFlowBuilderExtensions` and `AsyncFlowBuilderExtensions` for builder-level customization only

## Example

```fsharp
let asTaskFlow =
    TaskFlow.fromAsyncFlow (AsyncFlow.succeed "ok")
```

## Source

- [TaskFlow.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow.Net/TaskFlow.fs)
