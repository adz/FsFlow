---
title: succeed
description: API reference for Validation.succeed
---

# succeed

Creates a successful validation result.


```fsharp
let succeed (value: 'value) : Validation<'value, 'error>
```




## Parameters

- `value`: The success value of type `'value`.

## Returns

A successful `Validation`.

## Information

- **Module**: `Validation`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L226)

