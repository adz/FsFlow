---
title: run
description: API reference for Flow.run
---

# run

Executes a synchronous flow with the provided environment.



## Flow.run

- **Module**: `Flow`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L160)

## Examples

```fsharp
let flow = Flow.read (fun env -> $"Hello, {env}!")
let result = Flow.run "World" flow
// result = Ok "Hello, World!"
```

