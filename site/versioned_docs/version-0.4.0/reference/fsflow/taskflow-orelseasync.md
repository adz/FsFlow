---
title: orElseAsync
description: API reference for TaskFlow.orElseAsync
---

# orElseAsync




```fsharp
let orElseAsync (errorAsync: Async<'error>) (result: Result<'value, unit>) : Task<Result<'value, 'error>>
```




## Information

- **Module**: `TaskFlow`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L149)

