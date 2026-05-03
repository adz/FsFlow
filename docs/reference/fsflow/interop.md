---
title: Interop
description: Source-documented task and async interop helpers for FsFlow.
---

# Interop

This page shows the interop helpers that bridge task, async, and synchronous boundaries in FsFlow.

## TaskFlow bridges

- [`TaskFlow.fromFlow`](./taskflow-fromflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L176)
- [`TaskFlow.fromAsyncFlow`](./taskflow-fromasyncflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L179)
- [`TaskFlow.orElseTask`](./taskflow-orelsetask.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L104)
- [`TaskFlow.orElseAsync`](./taskflow-orelseasync.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L116)
- [`TaskFlow.orElseFlow`](./taskflow-orelseflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L128)
- [`TaskFlow.orElseAsyncFlow`](./taskflow-orelseasyncflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L142)
- [`TaskFlow.orElseTaskFlow`](./taskflow-orelsetaskflow.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L160)

## Builder extensions

- module `TaskFlowBuilderExtensions` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L907)
- module `AsyncFlowBuilderExtensions`: [omit] [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L981)

