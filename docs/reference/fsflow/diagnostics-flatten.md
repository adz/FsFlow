---
title: flatten
description: API reference for Diagnostics.flatten
---

# flatten

Flattens the structured diagnostics graph into a linear list of diagnostics.


```fsharp
let flatten (graph: Diagnostics<'error>) : Diagnostic<'error> list
```


## Remarks

During flattening, child paths are correctly prefixed with their parent segments,
ensuring each `Diagnostic` in the resulting list has a full absolute path.


## Parameters

- `graph`: The `Diagnostics` to flatten.

## Returns

A list of type `Diagnostic` list.

## Information

- **Module**: `Diagnostics`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L101)

