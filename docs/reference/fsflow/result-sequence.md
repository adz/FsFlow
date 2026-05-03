---
title: sequence
description: API reference for Result.sequence
---

# sequence

Runs a sequence of results until the first failure or the end of the sequence.


```fsharp
let sequence (results: seq<Result<'value, 'error>>) : Result<'value list, 'error>
```




## Parameters

- `results`: A sequence of type `seq&lt;Result&lt;'value, 'error&gt;&gt;`.

## Returns

A result containing the list of values or the first error encountered.

## Information

- **Module**: `Result`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L172)

