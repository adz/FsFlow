---
title: TaskFlowSpec
description: Source-documented task workflow specification for FsFlow.
---

# TaskFlowSpec

This page shows the source-documented `TaskFlowSpec` surface, used for defining and running task workflows with explicit configurations.

## Core type

- type `TaskFlowSpec`: Describes a task-flow program that is built against a runtime context and later executed with a cancellation token. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L592)

## Module functions

- module `TaskFlowSpec`: Helpers for creating and running `TaskFlowSpec{runtime, env, error, value}` values. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L607)
- `TaskFlowSpec.create`: Creates a task-flow spec from runtime services, application dependencies, and a build function. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L609)
- `TaskFlowSpec.run`: Runs the spec with the supplied cancellation token. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L621)

## Source

- [TaskFlow.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs)
