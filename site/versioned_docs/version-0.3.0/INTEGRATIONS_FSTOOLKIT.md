---
title: FsToolkit ErrorHandling
description: How FsFlow fits beside existing FsToolkit.ErrorHandling code.
---

# FsToolkit.ErrorHandling

This page shows how FsFlow can fit beside existing `FsToolkit.ErrorHandling` code, especially `AsyncResult`, `TaskResult`, and result-heavy application code.

If you already have a codebase built around `Result`, `Async<Result<_,_>>`, or `Task<Result<_,_>>`, FsFlow does not need to force a rewrite of the pure parts. It can take over the boundary when you want explicit environment threading and typed runtime execution.

`FsToolkit.ErrorHandling` remains a strong fit because it stays close to core types, uses familiar module functions and computation-expression builders, and keeps overhead low for codebases that already speak in `Result`, `AsyncResult`, and `TaskResult`.

## Keep The Pure Pieces Pure

Existing `Result` helpers can stay `Result` helpers.

That means:

- pure validation stays pure
- transformation helpers stay pure
- effectful orchestration moves into `Flow`, `AsyncFlow`, or `TaskFlow`

This mirrors the way `FsToolkit.ErrorHandling` already encourages separation between validation and orchestration.

## What Usually Moves

- the boundary code that chooses between sync, async, and task runtime shapes
- the environment or request context that needs to flow through the boundary
- the final error-provisioning step when the error itself needs effectful work

## What Usually Stays

- existing `Result` and `Async<Result<_,_>>` helpers
- module functions that already work well on the current types
- existing computation-expression builders when they are already readable in the codebase

## Move The Boundary, Not The Whole Codebase

If a function currently returns `Async<Result<'value, 'error>>`, the most direct FsFlow migration is usually `AsyncFlow<'env, 'error, 'value>`.

If a function is already task-based, `TaskFlow<'env, 'error, 'value>` is the natural endpoint.

Use the same migration rule in either case:

1. keep pure validation and mapping in plain functions
2. lift the honest runtime boundary into FsFlow
3. keep the result shape unchanged until you have a reason to rename it

## Bridge Patterns

Typical bridges look like this:

- `Result<'value, unit>` validation helpers become `FsFlow.Validate` calls
- `Async<Result<'value, 'error>>` becomes `AsyncFlow.fromAsyncResult`
- `Task<Result<'value, 'error>>` becomes `TaskFlow.fromTaskResult`
- `Async<Result<'value, unit>>` or `Result<'value, unit>` can use `orElse*` bridges when error creation itself needs environment or runtime work

## Example

```fsharp
let validateName name =
    if System.String.IsNullOrWhiteSpace name then
        Error "name required"
    else
        Ok name

let loadGreeting : AsyncFlow<AppEnv, string, string> =
    asyncFlow {
        let! loadName = AsyncFlow.read _.LoadName
        let! name = loadName 42
        let! validName = validateName name
        let! prefix = AsyncFlow.read _.Prefix
        return $"{prefix} {validName}"
    }
```

The `AsyncFlow` layer can sit on top of the old `Async<Result<_,_>>` shape instead of replacing it everywhere at once.

## When FsToolkit Still Wins

Keep `FsToolkit.ErrorHandling` where its existing combinators already make the code clearer.

FsFlow is not trying to delete that ecosystem. It is trying to make the boundary explicit when the runtime and environment shape matter.
