---
title: mapError
description: API reference for Result.mapError
---

# mapError

Maps the failure value of a result.


```fsharp
let mapError (mapper: 'error -> 'nextError) (result: Result<'value, 'error>) : Result<'value, 'nextError>
```




## Parameters

- `mapper`: A function of type `'error -> 'nextError`.
- `result`: The source `Result`.

## Returns

A new `Result` with the mapped error.

## Information

- **Module**: `Result`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L151)

