---
title: mapErrorTo
description: API reference for Result.mapErrorTo
---

# mapErrorTo

Replaces the unit failure from a predicate result with the supplied error.


```fsharp
let mapErrorTo (error: 'nextError) (result: Result<'value, unit>) : Result<'value, 'nextError>
```




## Parameters

- `error`: The error of type `'nextError` to return on failure.
- `result`: The source result of type `FSharpResult`.

## Returns

A result with the new error type.

## Information

- **Module**: `Result`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L163)

