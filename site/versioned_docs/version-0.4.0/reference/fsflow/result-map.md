---
title: map
description: API reference for Result.map
---

# map

Maps the successful value of a result.


```fsharp
let map (mapper: 'value -> 'next) (result: Result<'value, 'error>) : Result<'next, 'error>
```




## Parameters

- `mapper`: A function of type `'value -> 'next`.
- `result`: The source `Result`.

## Returns

A new `Result` with the mapped value.

## Information

- **Module**: `Result`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L127)

