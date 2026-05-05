---
title: Result
description: Source-documented fail-fast result helpers for FsFlow.
---

# Result

This page shows the source-documented `Result` surface: the module functions and the `result { }` builder.

## Builder

- [`Builders.result`](./builders-result.md): The fail-fast `result { }` computation expression. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L1434)

## Module functions

- module `Result`: Helpers for fail-fast `Result` workflows and the bridge from
placeholder unit failures into application errors. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L122)
- [`Result.map`](./result-map.md): Maps the successful value of a result. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L127)
- [`Result.bind`](./result-bind.md): Sequences a result-producing continuation after a successful value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L139)
- [`Result.mapError`](./result-maperror.md): Maps the failure value of a result. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L151)
- [`Result.mapErrorTo`](./result-maperrorto.md): Replaces the unit failure from a predicate result with the supplied error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L163)
- [`Result.sequence`](./result-sequence.md): Runs a sequence of results until the first failure or the end of the sequence. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L172)
- [`Result.traverse`](./result-traverse.md): Maps a sequence with a fail-fast result-producing function. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L191)

