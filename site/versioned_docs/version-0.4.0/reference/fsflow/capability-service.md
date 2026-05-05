---
title: service
description: API reference for Capability.service
---

# service

Reads a capability from a record-based environment projection.


```fsharp
let service (projection: 'env -> 'service) : TaskFlow<'env, 'error, 'service>
```




## Information

- **Module**: `Capability`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L719)

