---
title: flow
description: API reference for Builders.flow
---

# flow

The sync-only `flow { }` computation expression.


```fsharp
let flow
```


## Remarks

<para>
Use this builder when the boundary is synchronous and you want explicit environment
reads without introducing async or task scheduling.
</para>
<para>
It is the simplest builder in the library and is a good default for pure composition
and deterministic orchestration.
</para>


## Information

- **Module**: `Builders`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L1458)

## Examples

```fsharp
let greeting =
    flow {
        let! name = Flow.read (fun env -> env.Name)
        return $"Hello, {name}"
    }
```

