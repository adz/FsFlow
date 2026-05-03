---
title: TaskFlow
description: Source-documented task workflow surface in FsFlow.
---

# TaskFlow

This page shows the source-documented `TaskFlow` surface: the core type, the module functions, and the `taskFlow { }` builder.

## Core type

- type `TaskFlow`: Represents a cold task-based workflow that reads an environment, observes a runtime cancellation token,
returns a typed result, and is executed explicitly through `TaskFlow.run`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L15)

## Module functions

- module `TaskFlow`: Core functions for creating, composing, executing, and adapting task flows. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L63)
- [`TaskFlow.run`](./taskflow-run.md): Executes a task flow with the provided environment and cancellation token. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L69)
- [`TaskFlow.runContext`](./taskflow-runcontext.md): Runs a task flow against a `RuntimeContext` and its internal cancellation token. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L80)
- [`TaskFlow.toTask`](./taskflow-totask.md): Converts a task flow into a hot `Task`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L94)
- [`TaskFlow.succeed`](./taskflow-succeed.md): Creates a successful task flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L104)
- [`TaskFlow.fail`](./taskflow-fail.md): Creates a failing task flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L110)
- [`TaskFlow.fromResult`](./taskflow-fromresult.md): Lifts a standard `Result` into a task flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L116)
- [`TaskFlow.fromOption`](./taskflow-fromoption.md): Lifts an option into a task flow with the supplied error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L123)
- [`TaskFlow.fromValueOption`](./taskflow-fromvalueoption.md): Lifts a value option into a task flow with the supplied error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L132)
- [`TaskFlow.orElseTask`](./taskflow-orelsetask.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L137)
- [`TaskFlow.orElseAsync`](./taskflow-orelseasync.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L149)
- [`TaskFlow.orElseFlow`](./taskflow-orelseflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L161)
- [`TaskFlow.orElseAsyncFlow`](./taskflow-orelseasyncflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L175)
- [`TaskFlow.orElseTaskFlow`](./taskflow-orelsetaskflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L193)
- [`TaskFlow.fromFlow`](./taskflow-fromflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L209)
- [`TaskFlow.fromAsyncFlow`](./taskflow-fromasyncflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L212)
- [`TaskFlow.fromTask`](./taskflow-fromtask.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L217)
- [`TaskFlow.fromTaskResult`](./taskflow-fromtaskresult.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L224)
- [`TaskFlow.env`](./taskflow-env.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L229)
- [`TaskFlow.read`](./taskflow-read.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L232)
- [`TaskFlow.readRuntime`](./taskflow-readruntime.md): Reads the runtime half of a runtime-context environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L236)
- [`TaskFlow.readEnvironment`](./taskflow-readenvironment.md): Reads the application environment half of a runtime-context environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L242)
- [`TaskFlow.map`](./taskflow-map.md): Maps the successful value of a task flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L251)
- [`TaskFlow.bind`](./taskflow-bind.md): Sequences a task-flow-producing continuation after a successful value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L274)
- [`TaskFlow.tap`](./taskflow-tap.md): Runs a task-based side effect on success and preserves the original value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L297)
- [`TaskFlow.tapError`](./taskflow-taperror.md): Runs a task-based side effect on failure and preserves the original error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L311)
- [`TaskFlow.mapError`](./taskflow-maperror.md): Maps the error value of a task flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L333)
- [`TaskFlow.catch`](./taskflow-catch.md): Catches exceptions raised during execution and maps them to a typed error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L352)
- [`TaskFlow.orElse`](./taskflow-orelse.md): Falls back to another task flow when the source flow fails. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L368)
- [`TaskFlow.zip`](./taskflow-zip.md): Combines two task flows into a tuple of their values. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L385)
- [`TaskFlow.map2`](./taskflow-map2.md): Combines two task flows with a mapping function. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L400)
- [`TaskFlow.localEnv`](./taskflow-localenv.md): Transforms the environment before running a task flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L412)
- [`TaskFlow.delay`](./taskflow-delay.md): Defers task flow construction until execution time. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L426)
- [`TaskFlow.traverse`](./taskflow-traverse.md): Transforms a sequence of values into a task flow and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L437)
- [`TaskFlow.sequence`](./taskflow-sequence.md): Transforms a sequence of task flows into a task flow of a sequence and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L462)

## Builder

- [`TaskBuilders.taskFlow`](./taskbuilders-taskflow.md): The .NET `taskFlow { }` computation expression. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L1152)

