---
title: TaskFlow
description: Source-documented task workflow surface in FsFlow.
---

# TaskFlow

This page shows the source-documented `TaskFlow` surface: the core type, the module functions, and the `taskFlow { }` builder.

## Core type

- type `TaskFlow`: Represents a cold task-based workflow that reads an environment, observes a runtime cancellation token, returns a typed result, and is executed explicitly through `TaskFlow.run`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L15)

## Module functions

- module `TaskFlow`: Core functions for creating, composing, executing, and adapting task flows. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L63)
- `TaskFlow.run` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L64)
- `TaskFlow.runContext`: Runs a task flow against a runtime context and its cancellation token. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L72)
- `TaskFlow.toTask` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L78)
- `TaskFlow.succeed` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L85)
- `TaskFlow.fail` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L88)
- `TaskFlow.fromResult` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L91)
- `TaskFlow.fromOption` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L94)
- `TaskFlow.fromValueOption` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L99)
- `TaskFlow.orElseTask` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L104)
- `TaskFlow.orElseAsync` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L116)
- `TaskFlow.orElseFlow` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L128)
- `TaskFlow.orElseAsyncFlow` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L142)
- `TaskFlow.orElseTaskFlow` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L160)
- `TaskFlow.fromFlow` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L176)
- `TaskFlow.fromAsyncFlow` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L179)
- `TaskFlow.fromTask` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L184)
- `TaskFlow.fromTaskResult` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L191)
- `TaskFlow.env` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L196)
- `TaskFlow.read` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L199)
- `TaskFlow.readRuntime`: Reads the runtime half of a runtime-context environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L203)
- `TaskFlow.readEnvironment`: Reads the application environment half of a runtime-context environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L209)
- `TaskFlow.map` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L214)
- `TaskFlow.bind` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L229)
- `TaskFlow.tap` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L248)
- `TaskFlow.tapError` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L258)
- `TaskFlow.mapError` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L276)
- `TaskFlow.catch` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L291)
- `TaskFlow.orElse` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L303)
- `TaskFlow.zip` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L316)
- `TaskFlow.map2` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L326)
- `TaskFlow.localEnv` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L334)
- `TaskFlow.delay` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L361)
- `TaskFlow.traverse`: Transforms a sequence of values into a task flow and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L369)
- `TaskFlow.sequence`: Transforms a sequence of task flows into a task flow of a sequence and stops at the first failure. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L392)

## Builder

- `TaskBuilders.taskFlow`: The .NET `taskFlow { }` computation expression. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L1075)

## Source

- [TaskFlow.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs)
