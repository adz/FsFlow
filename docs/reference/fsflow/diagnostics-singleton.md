---
title: singleton
description: API reference for Diagnostics.singleton
---

# singleton

Creates a diagnostics graph containing exactly one diagnostic item at the root.


```fsharp
let singleton (diagnostic: Diagnostic<'error>) : Diagnostics<'error>
```




## Parameters

- `diagnostic`: The `Diagnostic` to wrap.

## Returns

A `Diagnostics` with a single error.

## Information

- **Module**: `Diagnostics`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L69)

