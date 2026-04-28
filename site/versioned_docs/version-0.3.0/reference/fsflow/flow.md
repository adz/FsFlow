---
title: Flow
description: The synchronous workflow surface in FsFlow.
---

# Flow

This page shows the synchronous boundary family in one place: the `Flow<'env, 'error, 'value>` type, the `Flow` module, and the builder entry point as syntax on top.

Use this page when the runtime boundary is synchronous and you want explicit environment access with typed failure.

## Core shape

`Flow<'env, 'error, 'value>` is the cold boundary type for synchronous code.

- `Flow<'env, 'error, 'value>` stores a function from environment to `Result<'value, 'error>`.
- `Flow.run` executes the boundary against an environment.
- `Flow.toResult` is the shape you get back from execution.

## What you can do

`Flow` is the right home for synchronous orchestration when the runtime boundary needs to stay visible:

- start from `succeed`, `fail`, `fromResult`, or option/value-option bridges
- read environment directly with `env` and `read`
- compose with `map`, `bind`, `tap`, `tapError`, `mapError`, and `orElse`
- catch ordinary exceptions and translate them into typed failures
- run work with explicit cancellation, timeout, retry, and cleanup helpers
- lift validation with `Validate.orElse` and `Validate.orElseWith`

## Builder entry point

The `flow {}` builder is the ergonomic syntax on top of the module surface.

- use it for linear code paths that read more like application code
- use `Flow` members for the actual primitives
- treat `FlowBuilder` as plumbing rather than a docs destination

## Create and run

The creation helpers cover the normal entry points:

- `Flow.run` executes a synchronous flow with the provided environment.
- `Flow.succeed` creates a successful synchronous flow.
- `Flow.value` is an alias for `Flow.succeed` when the call site reads better that way.
- `Flow.fail` creates a failing synchronous flow.
- `Flow.fromResult` lifts a `Result<'value, 'error>` into the boundary.
- `Flow.fromOption` and `Flow.fromValueOption` lift optional values with an explicit error.
- `Flow.env` reads the whole environment as the flow value.
- `Flow.read` projects a value from the current environment.

## Compose

The composition helpers are the ones people reach for most:

- `Flow.map` transforms the successful value.
- `Flow.bind` sequences the next step after success.
- `Flow.tap` observes the success path without changing it.
- `Flow.tapError` observes the failure path without changing it.
- `Flow.mapError` transforms the failure value.
- `Flow.catch` turns an ordinary exception into a typed failure.
- `Flow.orElse` and `Flow.orElseFlow` bridge validation into the boundary.
- `Flow.zip` and `Flow.map2` combine independent flows.

## Environment and lifecycle

The environment helpers keep dependency reading explicit:

- `Flow.env` gives you the current environment value.
- `Flow.read` projects a value from the environment.
- `Flow.localEnv` narrows or reshapes the environment for a subflow.
- `Flow.delay` defers construction until execution time.

## Collections

The traversal helpers keep list and sequence code readable:

- `Flow.traverse` runs a flow-producing function over each item.
- `Flow.sequence` turns a collection of flows into a flow of a collection.

## Runtime helpers

Runtime helpers are the operational layer that sits on top of the main flow model:

- `Flow.Runtime.catchCancellation` maps cancellation into a typed failure.
- `Flow.Runtime.useWithAcquireRelease` scopes resource lifetime inside the boundary.
- `Flow.Runtime.timeout`, `timeoutToOk`, `timeoutToError`, and `timeoutWith` bound the work in time.
- `Flow.Runtime.retry` repeats the boundary using `RetryPolicy<'error>`.
- logging and cleanup helpers remain explicit in the environment rather than hidden.

## Interop

The `Flow` surface deliberately bridges out to the other families and to validation:

- `Flow.orElseFlow` turns a `Result<'value, unit>` into a typed synchronous boundary when the error itself needs a flow.
- `AsyncFlow.fromFlow` lifts a synchronous flow into an async boundary.
- `Validate.orElse` and `Validate.orElseWith` convert pure validation into an application error at the boundary.

## Example

```fsharp
type AppEnv =
    { Prefix: string }

type AppError =
    | MissingName

let validateName name =
    if System.String.IsNullOrWhiteSpace name then
        Error MissingName
    else
        Ok name

let greet input : Flow<AppEnv, AppError, string> =
    flow {
        let! name = validateName input |> Flow.fromResult
        let! prefix = Flow.read _.Prefix
        return $"{prefix} {name}"
    }
```

## Member map

If you want the short version of the page, reach for:

- `Flow.succeed` and `Flow.fail` to create immediate boundaries
- `Flow.fromResult`, `Flow.fromOption`, and `Flow.fromValueOption` to lift ordinary values
- `Flow.read` and `Flow.env` for explicit environment access
- `Flow.map`, `Flow.bind`, `Flow.tap`, `Flow.tapError`, and `Flow.orElse` for composition
- `Flow.traverse` and `Flow.sequence` for lists and sequences
- `Flow.Runtime.*` for cancellation, timeout, retry, and resource management

## Source-Lifted Notes

The source comments on `Flow` are intentionally short and direct. The most useful ones are:

- `Flow.run` is execution, not construction.
- `Flow.succeed` and `Flow.fail` are the immediate terminals.
- `Flow.fromResult`, `Flow.fromOption`, and `Flow.fromValueOption` are the ordinary lift path.
- `Flow.orElseFlow` is the bridge from pure validation into environment-aware failure creation.
- `Flow.read` keeps dependency access explicit without pulling the whole environment into local variables.
- `Flow.Runtime.*` collects operational helpers so the main flow surface stays readable.

## Source

- [Flow.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs)
