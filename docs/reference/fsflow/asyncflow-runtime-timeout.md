---
title: timeout
description: API reference for AsyncFlow.Runtime.timeout
---

# timeout

Wraps a flow with a timeout. If the flow does not complete within the specified duration, returns a typed error.

## Remarks

This helper translates timeout into a typed error. It does not automatically cancel the underlying work on timeout.


## AsyncFlow.Runtime.timeout

- **Module**: `AsyncFlow.Runtime`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L783)

