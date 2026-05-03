---
title: Runtime
description: Source-documented runtime support and helpers for FsFlow.
---

# Runtime

This page shows the source-documented runtime surface: logging, retry policies, and async operational helpers.

## Logging

- type `LogLevel`: Log levels used by runtime logging helpers and environment-provided logging functions. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L32)
- type `LogEntry`: A structured log entry written through a runtime logger. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L43)

## Retry policy

- type `RetryPolicy`: Defines how runtime retry helpers repeat typed failures in a controlled way. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L53)
- module `RetryPolicy`: Standard retry policies for runtime helpers. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L64)
- `RetryPolicy.noDelay` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L65)

## Async operational helpers

- `AsyncFlow.Runtime`: Runtime helpers for operational concerns like logging, timeout, retry, and cleanup. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L605)
- `AsyncFlow.Runtime.cancellationToken`: Reads the current cancellation token from the flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L613)
- `AsyncFlow.Runtime.catchCancellation`: Catches `OperationCanceledException` and converts it into a typed error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L626)
- `AsyncFlow.Runtime.ensureNotCanceled`: Checks if cancellation has been requested and returns a typed error if it has. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L643)
- `AsyncFlow.Runtime.sleep`: Suspends the flow for the specified duration, observing cancellation. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L659)
- `AsyncFlow.Runtime.log`: Writes a log entry using the writer provided by the environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L673)
- `AsyncFlow.Runtime.logWith`: Writes a log entry using a message produced from the environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L695)
- `AsyncFlow.Runtime.useWithAcquireRelease`: Safely acquires a resource, uses it, and ensures it is released via a task-based action. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L717)
- `AsyncFlow.Runtime.timeout`: Wraps a flow with a timeout. If the flow does not complete within the specified duration, returns a typed error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L745)
- `AsyncFlow.Runtime.timeoutToOk`: Wraps a flow with a timeout. If the flow does not complete within the specified duration, returns a success value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L769)
- `AsyncFlow.Runtime.timeoutToError`: Transitions to a failure value on timeout. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L793)
- `AsyncFlow.Runtime.timeoutWith`: Transitions to a fallback workflow on timeout. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L803)
- `AsyncFlow.Runtime.retry`: Retries a flow according to the specified policy. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L829)

## Source

- [Flow.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs)
