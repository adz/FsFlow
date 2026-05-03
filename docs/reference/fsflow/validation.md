---
title: Validation
description: Source-documented accumulating validation for FsFlow.
---

# Validation

This page shows the source-documented `Validation` surface: the accumulating result type, the module functions, and the `validate { }` builder.

## Core type

- type `Validation`: An accumulating validation result that keeps the structured diagnostics graph visible. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L34)

## Module functions

- module `Validation`: Helpers for accumulating validation results with mergeable diagnostics. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L161)
- [`Validation.toResult`](./validation-toresult.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L164)
- [`Validation.succeed`](./validation-succeed.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L167)
- [`Validation.fail`](./validation-fail.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L170)
- [`Validation.fromResult`](./validation-fromresult.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L173)
- [`Validation.map`](./validation-map.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L178)
- [`Validation.bind`](./validation-bind.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L184)
- [`Validation.mapError`](./validation-maperror.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L192)
- [`Validation.map2`](./validation-map2.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L212)
- [`Validation.apply`](./validation-apply.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L225)
- [`Validation.collect`](./validation-collect.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L231)
- [`Validation.sequence`](./validation-sequence.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L239)
- [`Validation.merge`](./validation-merge.md) [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L242)

## Builder

- [`Builders.validate`](./builders-validate.md): The accumulating `validate { }` computation expression. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L1263)

