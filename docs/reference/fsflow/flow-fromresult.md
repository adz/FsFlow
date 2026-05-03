---
title: fromResult
description: API reference for Flow.fromResult
---

# fromResult

Lifts a `Result` into a synchronous flow.



## Flow.fromResult

- **Module**: `Flow`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L201)

## Examples

```fsharp
let res = Ok "success"
let flow = Flow.fromResult res
```

