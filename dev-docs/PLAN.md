# FsFlow Plan

This file tracks the live architectural direction and the open decisions that still need resolution.

`dev-docs/TASKS.md` is the executable backlog.
`dev-docs/decisions/README.md` indexes the settled decisions and rationale that have been split out of this plan.

## Current Direction

The next iteration keeps the three-family model:

- `Flow<'env,'error,'value>` for sync/result-oriented work
- `AsyncFlow<'env,'error,'value>` for async workflows in core `FsFlow`
- `TaskFlow<'env,'error,'value>` for .NET task-based workflows in `FsFlow.Net`

The package split stays task-aware only where it needs to be:

- `FsFlow` owns the task-free core surface
- `FsFlow.Net` owns task interop, `ColdTask`, and .NET-specific runtime helpers

Workflow semantics stay cold and restartable. Hot `Task` and `ValueTask` inputs remain interop shapes, and `ColdTask` remains the deferred shape when rerun fidelity and token flow matter.

The detailed rationale, benchmarks, and settled packaging choices are archived in the decision logs.

## Decision Logs

- [Flow architecture](decisions/flow-architecture.md)
- [TaskFlow and ValueTask](decisions/taskflow-valuetask.md)
- [Benchmark history](decisions/benchmark-history.md)
- [Docs source extraction](decisions/docs-source-extraction.md)
- [Reader-env `yield`](decisions/reader-env-yield.md)

## Open Questions

- `Option<'value>` and `ValueOption<'value>` short-circuit behavior and implicit binding rules.
- Core logging abstraction versus .NET `ILogger` adapters and ergonomics.
