---
title: AsyncFlow
description: Source-documented async workflow surface in FsFlow.
---

# AsyncFlow

This page shows the source-documented `AsyncFlow` surface: the core type, the module functions, and the `asyncFlow { }` builder.

## Core type

- type `AsyncFlow`: Represents a cold async workflow that reads an environment, returns a typed result, and is executed explicitly through `AsyncFlow.run`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L24)

## Module functions

- module `AsyncFlow`: Core functions for creating, composing, executing, and adapting async flows. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L344)
- `AsyncFlow.run`: Executes an async flow with the provided environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L346)
- `AsyncFlow.toAsync`: Converts an async flow into its raw async result shape. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L353)
- `AsyncFlow.succeed`: Creates a successful async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L357)
- `AsyncFlow.fail`: Creates a failing async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L361)
- `AsyncFlow.fromResult`: Lifts a `Result` into an async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L365)
- `AsyncFlow.fromOption`: Lifts an option into an async flow with the supplied error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L369)
- `AsyncFlow.fromValueOption`: Lifts a value option into an async flow with the supplied error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L375)
- `AsyncFlow.orElseAsync`: Turns a pure validation result into an async flow with async-provided failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L381)
- `AsyncFlow.orElseAsyncFlow`: Turns a pure validation result into an async flow whose failure value comes from another async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L394)
- `AsyncFlow.fromFlow`: Lifts a synchronous flow into an async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L411)
- `AsyncFlow.fromAsync`: Lifts an async value into an async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L415)
- `AsyncFlow.fromAsyncResult`: Lifts an async result into an async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L423)
- `AsyncFlow.env`: Reads the current environment as the flow value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L427)
- `AsyncFlow.read`: Projects a value from the current environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L431)
- `AsyncFlow.map`: Maps the successful value of an async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L435)
- `AsyncFlow.bind`: Sequences an async continuation after a successful value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L451)
- `AsyncFlow.tap`: Runs an async side effect on success and preserves the original value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L471)
- `AsyncFlow.tapError`: Runs an async side effect on failure and preserves the original error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L482)
- `AsyncFlow.mapError`: Maps the error value of an async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L501)
- `AsyncFlow.catch`: Catches exceptions raised during execution and maps them to a typed error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L517)
- `AsyncFlow.orElse`: Falls back to another async flow when the source flow fails. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L530)
- `AsyncFlow.zip`: Combines two async flows into a tuple of their values. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L544)
- `AsyncFlow.map2`: Combines two async flows with a mapping function. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L555)
- `AsyncFlow.localEnv`: Transforms the environment before running the async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L564)
- `AsyncFlow.delay`: Defers async flow construction until execution time. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L571)
- `AsyncFlow.traverse`: Transforms a sequence of values into an async flow and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L575)
- `AsyncFlow.sequence`: Transforms a sequence of async flows into an async flow of a sequence and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L598)

## Builder

- `Builders.asyncFlow`: The core `asyncFlow { }` computation expression. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L1220)

## Source

- [Flow.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs)
