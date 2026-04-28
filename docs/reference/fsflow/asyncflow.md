---
title: AsyncFlow
description: The async workflow surface in FsFlow.
---

# AsyncFlow

This page shows the async boundary family in one place: the `AsyncFlow<'env, 'error, 'value>` type, the `AsyncFlow` module, and the async builder surface.

Use this page when the runtime boundary is async-first and you want the same explicit environment and typed failure model as `Flow`.

## Core shape

`AsyncFlow<'env, 'error, 'value>` is the cold boundary type for async code.

- `AsyncFlow<'env, 'error, 'value>` stores a function from environment to `Async<Result<'value, 'error>>`.
- `AsyncFlow.run` executes the boundary.
- `AsyncFlow.toAsync` gives you the raw async result.

## What you can do

`AsyncFlow` keeps the same shape as `Flow`, but its runtime is `Async`:

- lift from `Flow` when synchronous composition is already correct
- lift from `Async` and `Async<Result<_, _>>` when the boundary is async-native
- compose with the same `map`, `bind`, `tap`, `tapError`, and `orElse` style
- preserve explicit dependency reading while moving async work through the graph
- bridge into `TaskFlow` when the runtime boundary becomes task-based

## Builder entry point

The `asyncFlow {}` builder is the readable orchestration layer on top of the module surface.

- use it when the sequence of async steps is the point of the code
- use the `AsyncFlow` module for the actual primitives
- keep `AsyncFlowBuilder` out of the reader's way unless they need implementation detail

## Create and run

The creation helpers mirror `Flow`:

- `AsyncFlow.run` executes the async boundary.
- `AsyncFlow.succeed` and `AsyncFlow.fail` create immediate boundaries.
- `AsyncFlow.fromResult` lifts a plain `Result<'value, 'error>`.
- `AsyncFlow.fromOption` and `AsyncFlow.fromValueOption` lift optional values with an explicit error.
- `AsyncFlow.fromFlow` bridges synchronous boundary code into async shape.
- `AsyncFlow.fromAsync` and `AsyncFlow.fromAsyncResult` lift core async values.
- `AsyncFlow.env` reads the full environment.
- `AsyncFlow.read` projects a value from the environment.

## Compose

The async composition surface is intentionally symmetrical:

- `AsyncFlow.map` transforms the successful value.
- `AsyncFlow.bind` sequences async-bound steps without losing the typed failure.
- `AsyncFlow.tap` and `AsyncFlow.tapError` observe success or failure without changing them.
- `AsyncFlow.mapError` reshapes the typed failure.
- `AsyncFlow.catch` turns an exception into a typed failure.
- `AsyncFlow.orElse` and `AsyncFlow.orElseFlow` bridge validation and async error creation.
- `AsyncFlow.zip` and `AsyncFlow.map2` combine independent async flows.

## Environment and lifecycle

Use these helpers when the async workflow still needs explicit dependency access:

- `AsyncFlow.env` exposes the current environment value.
- `AsyncFlow.read` projects a dependency from the environment.
- `AsyncFlow.localEnv` narrows or reshapes the environment for a subflow.
- `AsyncFlow.delay` defers construction until execution time.

## Collections

The traversal helpers preserve async composition without flattening the shape too early:

- `AsyncFlow.traverse` runs an async flow-producing function over each item.
- `AsyncFlow.sequence` turns a collection of async flows into an async flow of a collection.

## Runtime helpers

Async boundaries can still participate in runtime concerns:

- `AsyncFlow.Runtime.catchCancellation` maps cancellation into a typed failure.
- `AsyncFlow.Runtime.useWithAcquireRelease` scopes resource cleanup.
- `AsyncFlow.Runtime.timeout`, `timeoutToOk`, `timeoutToError`, and `timeoutWith` bound async work in time.
- `AsyncFlow.Runtime.retry` repeats the boundary using `RetryPolicy<'error>`.
- logging remains an explicit environment concern.

## Interop

The async surface is the bridge between synchronous boundaries and async runtime code:

- `AsyncFlow.fromFlow` lifts a synchronous boundary into async shape.
- `AsyncFlow.fromAsync` and `AsyncFlow.fromAsyncResult` lift core async values.
- `Flow.orElseFlow` keeps pure validation outside the boundary until the error itself needs effectful creation.
- `Validate.orElse` and `Validate.orElseWith` turn `Result<'value, unit>` into the final application error.

## Example

```fsharp
let validateName name =
    if System.String.IsNullOrWhiteSpace name then
        Error "missing"
    else
        Ok name

let workflow : AsyncFlow<unit, string, string> =
    asyncFlow {
        let! name = validateName "Ada" |> AsyncFlow.fromResult
        let! suffix = async { return "!" }
        return name + suffix
    }
```

## Member map

If you want the short version of the page, reach for:

- `AsyncFlow.succeed` and `AsyncFlow.fail` to create immediate boundaries
- `AsyncFlow.fromResult`, `AsyncFlow.fromOption`, and `AsyncFlow.fromValueOption` to lift ordinary values
- `AsyncFlow.fromFlow`, `AsyncFlow.fromAsync`, and `AsyncFlow.fromAsyncResult` for async bridges
- `AsyncFlow.read` and `AsyncFlow.env` for explicit environment access
- `AsyncFlow.map`, `AsyncFlow.bind`, `AsyncFlow.tap`, `AsyncFlow.tapError`, and `AsyncFlow.orElse` for composition
- `AsyncFlow.traverse` and `AsyncFlow.sequence` for lists and sequences
- `AsyncFlow.Runtime.*` for cancellation, timeout, retry, and resource management

## Source-Lifted Notes

The source comments on `AsyncFlow` stay terse on purpose. The useful ones are:

- `AsyncFlow.run` is execution of the async boundary.
- `AsyncFlow.succeed`, `fail`, `fromResult`, `fromOption`, and `fromValueOption` are the direct creation and lift path.
- `AsyncFlow.fromFlow` is the bridge when the synchronous boundary is already correct.
- `AsyncFlow.fromAsync` and `AsyncFlow.fromAsyncResult` are the async-native bridges.
- `AsyncFlow.read` keeps dependency access explicit while leaving the runtime async.
- `AsyncFlow.Runtime.*` keeps operational helpers separate from business composition.

## Source

- [Flow.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs)
