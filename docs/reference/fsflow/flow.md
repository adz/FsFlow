---
title: Flow
description: Source-documented synchronous workflow surface in FsFlow.
---

# Flow

This page shows the source-documented `Flow` surface: the core type, the module functions, and the `flow { }` builder.

## Core type

- type `Flow`: Represents a cold synchronous workflow that reads an environment, returns a typed result, and is executed explicitly through `Flow.run`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L13)

## Module functions

- module `Flow`: Core functions for creating, composing, executing, and adapting synchronous flows. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L151)
- `Flow.run`: Executes a synchronous flow with the provided environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L153)
- `Flow.succeed`: Creates a successful synchronous flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L157)
- `Flow.value`: Alias for `succeed` that reads well in some call sites. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L161)
- `Flow.fail`: Creates a failing synchronous flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L165)
- `Flow.fromResult`: Lifts a `Result` into a synchronous flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L169)
- `Flow.fromOption`: Lifts an option into a synchronous flow with the supplied error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L173)
- `Flow.fromValueOption`: Lifts a value option into a synchronous flow with the supplied error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L179)
- `Flow.orElseFlow`: Turns a pure validation result into a synchronous flow with environment-provided failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L185)
- `Flow.env`: Reads the current environment as the flow value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L198)
- `Flow.read`: Projects a value from the current environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L202)
- `Flow.map`: Maps the successful value of a synchronous flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L206)
- `Flow.bind`: Sequences a synchronous continuation after a successful value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L213)
- `Flow.tap`: Runs a synchronous side effect on success and preserves the original value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L229)
- `Flow.tapError`: Runs a synchronous side effect on failure and preserves the original error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L240)
- `Flow.mapError`: Maps the error value of a synchronous flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L253)
- `Flow.catch`: Catches exceptions raised during execution and maps them to a typed error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L265)
- `Flow.orElse`: Falls back to another flow when the source flow fails. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L276)
- `Flow.zip`: Combines two flows into a tuple of their values. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L286)
- `Flow.map2`: Combines two flows with a mapping function. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L297)
- `Flow.localEnv`: Transforms the environment before running the flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L306)
- `Flow.delay`: Defers flow construction until execution time. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L313)
- `Flow.traverse`: Transforms a sequence of values into a flow and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L317)
- `Flow.sequence`: Transforms a sequence of flows into a flow of a sequence and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L336)

## Builder

- `Builders.flow`: The sync-only `flow { }` computation expression. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L1215)

## Source

- [Flow.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs)
