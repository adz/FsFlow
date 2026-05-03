---
title: Result
description: Source-documented fail-fast result helpers for FsFlow.
---

# Result

This page shows the source-documented `Result` surface: the module functions and the `result { }` builder.

## Module functions

- module `Result`: Helpers for fail-fast `Result` workflows and the bridge from placeholder unit failures into application errors. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L86)
- `Result.map`: Maps the successful value of a result. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L88)
- `Result.bind`: Sequences a result-producing continuation after a successful value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L97)
- `Result.mapError`: Maps the failure value of a result. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L106)
- `Result.mapErrorTo`: Replaces the unit failure from a predicate result with the supplied error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L115)
- `Result.sequence`: Runs a sequence of results until the first failure or the end of the sequence. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L122)
- `Result.traverse`: Maps a sequence with a fail-fast result-producing function. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L138)

## Builder

- `Builders.result`: The fail-fast `result { }` computation expression. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L1210)

## Source

- [Validate.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs)
