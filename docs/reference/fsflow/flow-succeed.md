---
title: succeed
description: API reference for Flow.succeed
---

# succeed

Creates a successful synchronous flow.


```fsharp
let succeed (value: 'value) : Flow<'env, 'error, 'value>
```




## Information

- **Module**: `Flow`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L171)

## Examples

```fsharp
let flow = Flow.succeed 42
let result = Flow.run () flow
// result = Ok 42
```

