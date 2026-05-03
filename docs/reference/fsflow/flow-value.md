---
title: value
description: API reference for Flow.value
---

# value

Alias for `succeed` that reads well in some call sites.


```fsharp
let value (item: 'value) : Flow<'env, 'error, 'value>
```




## Information

- **Module**: `Flow`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L180)

## Examples

```fsharp
let flow = Flow.value "constant"
```

