---
title: asyncFlow
description: API reference for Builders.asyncFlow
---

# asyncFlow

The core `asyncFlow { }` computation expression.


```fsharp
let asyncFlow
```


## Remarks

<para>
Use this builder when the runtime boundary is async-first and you need to compose
`Async` work with the same explicit environment model as `Flow`.
</para>
<para>
It is the right landing point for async orchestration that still wants typed failures
instead of exceptions.
</para>


## Information

- **Module**: `Builders`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L1483)

## Examples

```fsharp
let fetchProfile =
    asyncFlow {
        let! api = AsyncFlow.read (fun env -> env.Api)
        let! profile = api.LoadProfile()
        return profile
    }
```

