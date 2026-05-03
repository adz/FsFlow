---
title: AsyncFlow
description: Source-documented async workflow surface in FsFlow.
---

# AsyncFlow

This page shows the source-documented `AsyncFlow` surface: the core type, the module functions, and the `asyncFlow { }` builder.

## Core type

- type `AsyncFlow`: Represents a cold async workflow that reads an environment, returns a typed result,
and is executed explicitly through `AsyncFlow.run`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L24)

## Module functions

- module `AsyncFlow`: Core functions for creating, composing, executing, and adapting async flows. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L448)
- [`AsyncFlow.run`](./asyncflow-run.md): Executes an async flow with the provided environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L450)
- [`AsyncFlow.toAsync`](./asyncflow-toasync.md): Converts an async flow into its raw async result shape. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L457)
- [`AsyncFlow.succeed`](./asyncflow-succeed.md): Creates a successful async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L461)
- [`AsyncFlow.fail`](./asyncflow-fail.md): Creates a failing async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L465)
- [`AsyncFlow.fromResult`](./asyncflow-fromresult.md): Lifts a `Result` into an async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L469)
- [`AsyncFlow.fromOption`](./asyncflow-fromoption.md): Lifts an option into an async flow with the supplied error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L473)
- [`AsyncFlow.fromValueOption`](./asyncflow-fromvalueoption.md): Lifts a value option into an async flow with the supplied error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L479)
- [`AsyncFlow.orElseAsync`](./asyncflow-orelseasync.md): Turns a pure validation result into an async flow with async-provided failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L485)
- [`AsyncFlow.orElseAsyncFlow`](./asyncflow-orelseasyncflow.md): Turns a pure validation result into an async flow whose failure value comes from another async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L498)
- [`AsyncFlow.fromFlow`](./asyncflow-fromflow.md): Lifts a synchronous flow into an async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L515)
- [`AsyncFlow.fromAsync`](./asyncflow-fromasync.md): Lifts an async value into an async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L519)
- [`AsyncFlow.fromAsyncResult`](./asyncflow-fromasyncresult.md): Lifts an async result into an async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L527)
- [`AsyncFlow.env`](./asyncflow-env.md): Reads the current environment as the flow value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L531)
- [`AsyncFlow.read`](./asyncflow-read.md): Projects a value from the current environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L535)
- [`AsyncFlow.map`](./asyncflow-map.md): Maps the successful value of an async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L539)
- [`AsyncFlow.bind`](./asyncflow-bind.md): Sequences an async continuation after a successful value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L555)
- [`AsyncFlow.tap`](./asyncflow-tap.md): Runs an async side effect on success and preserves the original value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L575)
- [`AsyncFlow.tapError`](./asyncflow-taperror.md): Runs an async side effect on failure and preserves the original error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L586)
- [`AsyncFlow.mapError`](./asyncflow-maperror.md): Maps the error value of an async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L605)
- [`AsyncFlow.catch`](./asyncflow-catch.md): Catches exceptions raised during execution and maps them to a typed error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L621)
- [`AsyncFlow.orElse`](./asyncflow-orelse.md): Falls back to another async flow when the source flow fails. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L634)
- [`AsyncFlow.zip`](./asyncflow-zip.md): Combines two async flows into a tuple of their values. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L648)
- [`AsyncFlow.map2`](./asyncflow-map2.md): Combines two async flows with a mapping function. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L659)
- [`AsyncFlow.localEnv`](./asyncflow-localenv.md): Transforms the environment before running the async flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L668)
- [`AsyncFlow.delay`](./asyncflow-delay.md): Defers async flow construction until execution time. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L675)
- [`AsyncFlow.traverse`](./asyncflow-traverse.md): Transforms a sequence of values into an async flow and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L679)
- [`AsyncFlow.sequence`](./asyncflow-sequence.md): Transforms a sequence of async flows into an async flow of a sequence and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L702)

## Builder

- [`Builders.asyncFlow`](./builders-asyncflow.md): The core `asyncFlow { }` computation expression. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L1324)

