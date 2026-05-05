---
title: bind
description: API reference for Result.bind
---

# bind

Sequences a result-producing continuation after a successful value.


```fsharp
let bind (binder: 'value -> Result<'next, 'error>) (result: Result<'value, 'error>) : Result<'next, 'error>
```




## Parameters

- `binder`: A function of type `'value -> Result&lt;'next, 'error&gt;`.
- `result`: The source `Result`.

## Returns

The result of the binder or the original error.

## Information

- **Module**: `Result`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L139)

