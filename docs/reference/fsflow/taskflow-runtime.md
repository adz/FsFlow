---
title: TaskFlow Runtime
description: Source-documented task runtime helpers for FsFlow.
---

# TaskFlow Runtime

This page shows the source-documented task runtime surface: the runtime context and the task-specific operational helpers.

## Runtime context

- type `RuntimeContext`: Captures the two-context shape of a task workflow execution: runtime services, application capabilities, and the cancellation token for the current run. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Runtime.fs#L14)
- module `RuntimeContext`: Helpers for building and reshaping `RuntimeContext{runtime, env}` values. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Runtime.fs#L29)
- `RuntimeContext.create`: Creates a runtime context from the supplied runtime services, environment, and cancellation token. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Runtime.fs#L31)
- `RuntimeContext.runtime`: Reads the runtime half of a runtime context. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Runtime.fs#L43)
- `RuntimeContext.environment`: Reads the application environment half of a runtime context. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Runtime.fs#L46)
- `RuntimeContext.cancellationToken`: Reads the cancellation token stored in a runtime context. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Runtime.fs#L49)
- `RuntimeContext.mapRuntime`: Maps the runtime half of a runtime context. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Runtime.fs#L52)
- `RuntimeContext.mapEnvironment`: Maps the application environment half of a runtime context. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Runtime.fs#L63)
- `RuntimeContext.withRuntime`: Replaces the runtime half of a runtime context. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Runtime.fs#L74)
- `RuntimeContext.withEnvironment`: Replaces the environment half of a runtime context. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Runtime.fs#L81)

## Runtime helpers

- `TaskFlow.Runtime`: Task-native runtime helpers for operational concerns like logging, timeout, retry, and scoped cleanup. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L406)
- `TaskFlow.Runtime.cancellationToken`: Reads the current runtime cancellation token. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L408)
- `TaskFlow.Runtime.catchCancellation`: Converts an `OperationCanceledException` into a typed error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L412)
- `TaskFlow.Runtime.ensureNotCanceled`: Returns a typed error immediately when the runtime token is already canceled. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L425)
- `TaskFlow.Runtime.sleep`: Suspends the flow for the specified duration while observing cancellation. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L433)
- `TaskFlow.Runtime.log`: Writes a fixed log message through the environment-provided logger. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L441)
- `TaskFlow.Runtime.logWith`: Writes a log message computed from the current environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L456)
- `TaskFlow.Runtime.useWithAcquireRelease`: Acquires a resource, uses it, and always runs the release action. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L471)
- `TaskFlow.Runtime.timeout`: Fails with the supplied error when the flow does not complete before the timeout. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L496)
- `TaskFlow.Runtime.timeoutToOk`: Returns the supplied success value when the flow times out. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L514)
- `TaskFlow.Runtime.timeoutToError`: Forwards to `timeout` for a typed failure on timeout. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L532)
- `TaskFlow.Runtime.timeoutWith`: Runs a fallback flow when the original flow times out. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L540)
- `TaskFlow.Runtime.retry`: Retries a flow according to the supplied policy. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L558)

## Source

- [Runtime.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Runtime.fs)
- [TaskFlow.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs)
