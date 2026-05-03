---
title: Diagnostics
description: Source-documented validation diagnostics graph for FsFlow.
---

# Diagnostics

This page shows the source-documented `Diagnostics` surface: the path-aware graph types and the merge/flatten helpers.

## Graph types

- type `PathSegment`: Location markers used to describe where a diagnostic belongs in a validation graph. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L7)
- type `Path`: A path through a validation graph. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L13)
- type `Diagnostic`: A single failure item attached to a path in a validation graph. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L16)
- type `Diagnostics`: A mergeable validation graph that carries local diagnostics and nested child branches. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L25)

## Module functions

- module `Diagnostics`: Helpers for building, merging, and flattening validation diagnostics graphs. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L41)
- `Diagnostics.empty` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L42)
- `Diagnostics.singleton` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L48)
- `Diagnostics.merge` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L54)
- `Diagnostics.flatten` [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L65)

## Source

- [Validate.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs)
