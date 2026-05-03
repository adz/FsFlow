---
title: fail
description: API reference for Flow.fail
---

# fail

Creates a failing synchronous flow.



## Flow.fail

- **Module**: `Flow`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L191)

## Examples

```fsharp
let flow = Flow.fail "error"
let result = Flow.run () flow
// result = Error "error"
```

