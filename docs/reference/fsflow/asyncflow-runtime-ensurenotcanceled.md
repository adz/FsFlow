---
title: ensureNotCanceled
description: API reference for AsyncFlow.Runtime.ensureNotCanceled
---

# ensureNotCanceled

Checks if cancellation has been requested and returns a typed error if it has.

## Remarks

This observes the current token state and returns a typed error immediately instead of waiting for an exception.


## AsyncFlow.Runtime.ensureNotCanceled

- **Module**: `AsyncFlow.Runtime`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L681)

