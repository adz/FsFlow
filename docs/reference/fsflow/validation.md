---
title: Validation
description: Source-documented accumulating validation for FsFlow.
---

# Validation

This page shows the source-documented `Validation` surface: the accumulating result type, the module functions, and the `validate { }` builder.

## Core type

- type `Validation`: An accumulating validation result that keeps the structured diagnostics graph visible. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L50)

## Module functions

- module `Validation`: Helpers for accumulating validation results with mergeable diagnostics. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L214)
- [`Validation.toResult`](./validation-toresult.md): Converts a `Validation` into a standard `Result`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L220)
- [`Validation.succeed`](./validation-succeed.md): Creates a successful validation result. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L226)
- [`Validation.fail`](./validation-fail.md): Creates a failing validation result with the provided diagnostics. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L232)
- [`Validation.fromResult`](./validation-fromresult.md): Lifts a standard `Result` into the `Validation` context. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L241)
- [`Validation.map`](./validation-map.md): Maps the successful value of a validation. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L250)
- [`Validation.bind`](./validation-bind.md): Sequences a validation-producing continuation. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L265)
- [`Validation.mapError`](./validation-maperror.md): Maps the error type of a validation graph. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L277)
- [`Validation.map2`](./validation-map2.md): Combines two validations, accumulating errors if both fail. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L306)
- [`Validation.apply`](./validation-apply.md): Applies a validation-wrapped function to a validation-wrapped value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L323)
- [`Validation.collect`](./validation-collect.md): Collects a sequence of validations into a single validation of a list. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L335)
- [`Validation.sequence`](./validation-sequence.md): Transforms a sequence of validations into a validation of a list. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L346)
- [`Validation.merge`](./validation-merge.md): Merges two validations into a validation of a tuple. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L353)

## Builder

- [`Builders.validate`](./builders-validate.md): The accumulating `validate { }` computation expression. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L1329)

