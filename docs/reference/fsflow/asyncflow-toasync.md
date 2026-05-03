---
title: toAsync
description: API reference for AsyncFlow.toAsync
---

# toAsync

Converts an async flow into its raw async result shape.


```fsharp
let toAsync (environment: 'env) (flow: AsyncFlow<'env, 'error, 'value>) : Async<Result<'value, 'error>>
```




## Information

- **Module**: `AsyncFlow`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L471)

