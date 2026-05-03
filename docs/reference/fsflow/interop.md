---
title: Interop
description: Source-documented task and async interop helpers for FsFlow.
---

# Interop

This page shows the interop helpers that bridge task, async, and synchronous boundaries in FsFlow.

## TaskFlow bridges

- [`TaskFlow.fromFlow`](./taskflow-fromflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L209)
- [`TaskFlow.fromAsyncFlow`](./taskflow-fromasyncflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L212)
- [`TaskFlow.orElseTask`](./taskflow-orelsetask.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L137)
- [`TaskFlow.orElseAsync`](./taskflow-orelseasync.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L149)
- [`TaskFlow.orElseFlow`](./taskflow-orelseflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L161)
- [`TaskFlow.orElseAsyncFlow`](./taskflow-orelseasyncflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L175)
- [`TaskFlow.orElseTaskFlow`](./taskflow-orelsetaskflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L193)

## Builder extensions

- module `TaskFlowBuilderExtensions` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L984)
- module `AsyncFlowBuilderExtensions`: [omit] [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L1058)

