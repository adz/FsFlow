---
title: ColdTask
description: Source-documented delayed task helpers used by FsFlow.
---

# ColdTask

This page shows the source-documented `ColdTask` surface: the delayed task helper used to anchor execution to the runtime context.

## Core type

- type `ColdTask`: Represents delayed task work that can observe a runtime cancellation token when it is started. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L23)

## Module functions

- module `ColdTask`: Core functions for creating and executing cold tasks. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L31)
- [`ColdTask.run`](./coldtask-run.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L55)
- [`ColdTask.create`](./coldtask-create.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L32)
- [`ColdTask.fromTaskFactory`](./coldtask-fromtaskfactory.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L35)
- [`ColdTask.fromTask`](./coldtask-fromtask.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L38)
- [`ColdTask.fromValueTaskFactory`](./coldtask-fromvaluetaskfactory.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L41)
- [`ColdTask.fromValueTaskFactoryWithoutCancellation`](./coldtask-fromvaluetaskfactorywithoutcancellation.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L46)
- [`ColdTask.fromValueTask`](./coldtask-fromvaluetask.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L51)

