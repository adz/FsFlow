---
title: Flow
description: Source-documented synchronous workflow surface in FsFlow.
---

# Flow

This page shows the source-documented `Flow` surface: the core type, the module functions, and the `flow { }` builder.

## Core type

- type `Flow`: Represents a cold synchronous workflow that reads an environment, returns a typed result,
and is executed explicitly through `Flow.run`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L13)

## Module functions

- module `Flow`: Core functions for creating, composing, executing, and adapting synchronous flows. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L151)
- [`Flow.run`](./flow-run.md): Executes a synchronous flow with the provided environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L160)
- [`Flow.succeed`](./flow-succeed.md): Creates a successful synchronous flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L171)
- [`Flow.value`](./flow-value.md): Alias for `succeed` that reads well in some call sites. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L180)
- [`Flow.fail`](./flow-fail.md): Creates a failing synchronous flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L191)
- [`Flow.fromResult`](./flow-fromresult.md): Lifts a `Result` into a synchronous flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L201)
- [`Flow.fromOption`](./flow-fromoption.md): Lifts an option into a synchronous flow with the supplied error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L211)
- [`Flow.fromValueOption`](./flow-fromvalueoption.md): Lifts a value option into a synchronous flow with the supplied error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L217)
- [`Flow.orElseFlow`](./flow-orelseflow.md): Turns a pure validation result into a synchronous flow with environment-provided failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L223)
- [`Flow.env`](./flow-env.md): Reads the current environment as the flow value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L236)
- [`Flow.read`](./flow-read.md): Projects a value from the current environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L240)
- [`Flow.map`](./flow-map.md): Maps the successful value of a synchronous flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L244)
- [`Flow.bind`](./flow-bind.md): Sequences a synchronous continuation after a successful value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L251)
- [`Flow.tap`](./flow-tap.md): Runs a synchronous side effect on success and preserves the original value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L267)
- [`Flow.tapError`](./flow-taperror.md): Runs a synchronous side effect on failure and preserves the original error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L278)
- [`Flow.mapError`](./flow-maperror.md): Maps the error value of a synchronous flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L291)
- [`Flow.catch`](./flow-catch.md): Catches exceptions raised during execution and maps them to a typed error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L303)
- [`Flow.orElse`](./flow-orelse.md): Falls back to another flow when the source flow fails. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L314)
- [`Flow.zip`](./flow-zip.md): Combines two flows into a tuple of their values. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L324)
- [`Flow.map2`](./flow-map2.md): Combines two flows with a mapping function. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L335)
- [`Flow.localEnv`](./flow-localenv.md): Transforms the environment before running the flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L344)
- [`Flow.delay`](./flow-delay.md): Defers flow construction until execution time. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L351)
- [`Flow.traverse`](./flow-traverse.md): Transforms a sequence of values into a flow and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L355)
- [`Flow.sequence`](./flow-sequence.md): Transforms a sequence of flows into a flow of a sequence and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L374)

## Builder

- [`Builders.flow`](./builders-flow.md): The sync-only `flow { }` computation expression. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L1253)

