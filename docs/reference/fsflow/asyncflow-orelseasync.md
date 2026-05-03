---
title: orElseAsync
description: API reference for AsyncFlow.orElseAsync
---

# orElseAsync

Turns a pure validation result into an async flow with async-provided failure.


```fsharp
let orElseAsync (errorAsync: Async<'error>) (result: Result<'value, unit>) : Async<Result<'value, 'error>>
```




## Information

- **Module**: `AsyncFlow`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L485)

