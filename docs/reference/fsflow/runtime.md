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
- [`RetryPolicy.noDelay`](./retrypolicy-nodelay.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L65)

## Async operational helpers

- module `AsyncFlow.Runtime`: Runtime helpers for operational concerns like logging, timeout, retry, and cleanup. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L709)
- [`AsyncFlow.Runtime.cancellationToken`](./asyncflow-runtime-cancellationtoken.md): Reads the current cancellation token from the flow. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L717)
- [`AsyncFlow.Runtime.catchCancellation`](./asyncflow-runtime-catchcancellation.md): Catches `OperationCanceledException` and converts it into a typed error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L730)
- [`AsyncFlow.Runtime.ensureNotCanceled`](./asyncflow-runtime-ensurenotcanceled.md): Checks if cancellation has been requested and returns a typed error if it has. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L747)
- [`AsyncFlow.Runtime.sleep`](./asyncflow-runtime-sleep.md): Suspends the flow for the specified duration, observing cancellation. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L763)
- [`AsyncFlow.Runtime.log`](./asyncflow-runtime-log.md): Writes a log entry using the writer provided by the environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L777)
- [`AsyncFlow.Runtime.logWith`](./asyncflow-runtime-logwith.md): Writes a log entry using a message produced from the environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L799)
- [`AsyncFlow.Runtime.useWithAcquireRelease`](./asyncflow-runtime-usewithacquirerelease.md): Safely acquires a resource, uses it, and ensures it is released via a task-based action. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L821)
- [`AsyncFlow.Runtime.timeout`](./asyncflow-runtime-timeout.md): Wraps a flow with a timeout. If the flow does not complete within the specified duration, returns a typed error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L849)
- [`AsyncFlow.Runtime.timeoutToOk`](./asyncflow-runtime-timeouttook.md): Wraps a flow with a timeout. If the flow does not complete within the specified duration, returns a success value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L873)
- [`AsyncFlow.Runtime.timeoutToError`](./asyncflow-runtime-timeouttoerror.md): Transitions to a failure value on timeout. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L897)
- [`AsyncFlow.Runtime.timeoutWith`](./asyncflow-runtime-timeoutwith.md): Transitions to a fallback workflow on timeout. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L907)
- [`AsyncFlow.Runtime.retry`](./asyncflow-runtime-retry.md): Retries a flow according to the specified policy. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L933)

