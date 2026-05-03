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
- [`TaskFlow.run`](./taskflow-run.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L64)
- [`TaskFlow.runContext`](./taskflow-runcontext.md): Runs a task flow against a runtime context and its cancellation token. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L72)
- [`TaskFlow.toTask`](./taskflow-totask.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L78)
- [`TaskFlow.succeed`](./taskflow-succeed.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L85)
- [`TaskFlow.fail`](./taskflow-fail.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L88)
- [`TaskFlow.fromResult`](./taskflow-fromresult.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L91)
- [`TaskFlow.fromOption`](./taskflow-fromoption.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L94)
- [`TaskFlow.fromValueOption`](./taskflow-fromvalueoption.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L99)
- [`TaskFlow.orElseTask`](./taskflow-orelsetask.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L104)
- [`TaskFlow.orElseAsync`](./taskflow-orelseasync.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L116)
- [`TaskFlow.orElseFlow`](./taskflow-orelseflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L128)
- [`TaskFlow.orElseAsyncFlow`](./taskflow-orelseasyncflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L142)
- [`TaskFlow.orElseTaskFlow`](./taskflow-orelsetaskflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L160)
- [`TaskFlow.fromFlow`](./taskflow-fromflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L176)
- [`TaskFlow.fromAsyncFlow`](./taskflow-fromasyncflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L179)
- [`TaskFlow.fromTask`](./taskflow-fromtask.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L184)
- [`TaskFlow.fromTaskResult`](./taskflow-fromtaskresult.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L191)
- [`TaskFlow.env`](./taskflow-env.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L196)
- [`TaskFlow.read`](./taskflow-read.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L199)
- [`TaskFlow.readRuntime`](./taskflow-readruntime.md): Reads the runtime half of a runtime-context environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L203)
- [`TaskFlow.readEnvironment`](./taskflow-readenvironment.md): Reads the application environment half of a runtime-context environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L209)
- [`TaskFlow.map`](./taskflow-map.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L214)
- [`TaskFlow.bind`](./taskflow-bind.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L229)
- [`TaskFlow.tap`](./taskflow-tap.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L248)
- [`TaskFlow.tapError`](./taskflow-taperror.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L258)
- [`TaskFlow.mapError`](./taskflow-maperror.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L276)
- [`TaskFlow.catch`](./taskflow-catch.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L291)
- [`TaskFlow.orElse`](./taskflow-orelse.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L303)
- [`TaskFlow.zip`](./taskflow-zip.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L316)
- [`TaskFlow.map2`](./taskflow-map2.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L326)
- [`TaskFlow.localEnv`](./taskflow-localenv.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L334)
- [`TaskFlow.delay`](./taskflow-delay.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L361)
- [`TaskFlow.traverse`](./taskflow-traverse.md): Transforms a sequence of values into a task flow and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L369)
- [`TaskFlow.sequence`](./taskflow-sequence.md): Transforms a sequence of task flows into a task flow of a sequence and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L392)

## Builder

- [`TaskBuilders.taskFlow`](./taskbuilders-taskflow.md): The .NET `taskFlow { }` computation expression. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L1075)

