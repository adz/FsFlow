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
- `Validation.toResult` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L164)
- `Validation.succeed` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L167)
- `Validation.fail` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L170)
- `Validation.fromResult` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L173)
- `Validation.map` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L178)
- `Validation.bind` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L184)
- `Validation.mapError` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L192)
- `Validation.map2` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L212)
- `Validation.apply` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L225)
- `Validation.collect` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L231)
- `Validation.sequence` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L239)
- `Validation.merge` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L242)

## Builder

- `Builders.validate`: The accumulating `validate { }` computation expression. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L1225)

## Source

- [Validate.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs)
