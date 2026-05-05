---
title: traverse
description: API reference for Result.traverse
---

# traverse

Maps a sequence with a fail-fast result-producing function.


```fsharp
let traverse (mapper: 'source -> Result<'value, 'error>) (values: seq<'source>) : Result<'value list, 'error>
```




## Parameters

- `mapper`: A function of type `'source -> Result&lt;'value, 'error&gt;`.
- `values`: The source sequence.

## Returns

A result containing the list of successful mapped values.

## Information

- **Module**: `Result`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L191)

