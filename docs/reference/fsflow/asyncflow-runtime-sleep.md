---
title: sleep
description: API reference for AsyncFlow.Runtime.sleep
---

# sleep

Suspends the flow for the specified duration, observing cancellation.

## Remarks

If the runtime token is canceled, the underlying task raises cancellation which can be translated with `catchCancellation`.


## AsyncFlow.Runtime.sleep

- **Module**: `AsyncFlow.Runtime`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L697)

