---
title: Diagnostics
description: Source-documented validation diagnostics graph for FsFlow.
---

# Diagnostics

This page shows the source-documented `Diagnostics` surface: the path-aware graph types and the merge/flatten helpers.

## Graph types

- type `PathSegment`: Location markers used to describe where a diagnostic belongs in a validation graph. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L7)
- type `Path`: A path through a validation graph, represented as a list of `PathSegment`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L16)
- type `Diagnostic`: A single failure item attached to a path in a validation graph. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L19)
- type `Diagnostics`: A mergeable validation graph that carries local diagnostics and nested child branches. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L34)

## Module functions

- module `Diagnostics`: Helpers for building, merging, and flattening validation diagnostics graphs. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L57)
- [`Diagnostics.empty`](./diagnostics-empty.md): Creates an empty diagnostics graph with no errors. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L60)
- [`Diagnostics.singleton`](./diagnostics-singleton.md): Creates a diagnostics graph containing exactly one diagnostic item at the root. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L69)
- [`Diagnostics.merge`](./diagnostics-merge.md): Recursively merges two diagnostics graphs, combining shared branches and local errors. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L83)
- [`Diagnostics.flatten`](./diagnostics-flatten.md): Flattens the structured diagnostics graph into a linear list of diagnostics. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L101)

