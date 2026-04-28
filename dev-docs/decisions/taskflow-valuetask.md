# TaskFlow and ValueTask

Status: decided for now.
Recorded: 2026-04-28.

## Extracted From

- `dev-docs/PLAN.md`:
  - `ValueTask Direction`
  - `Task 18 evaluation notes`
  - `Task 19 decision`

## Source Dates

- 2026-04-28: `Decide TaskFlow stays Task-backed`
- 2026-04-28: `Add Task vs ValueTask backbone benchmark`
- 2026-04-28: `Confirm no separate valueTaskFlow abstraction`

## Decision

Keep `TaskFlow` internally `Task`-backed.
Treat `ValueTask` as a boundary and interop shape, not the stored execution representation.
Do not add a separate `valueTaskFlow` abstraction for now.

## Why

- `TaskFlow` is cold and restartable, so it stores and reuses composed operations across helpers.
- `ValueTask` has single-consumption hazards that make it a poor reusable backbone.
- The existing builder already accepts `ValueTask` at the boundary and normalizes it to `Task`.
- The `Task`-backed representation keeps combinators simpler and more uniform.

## Evidence

Benchmarking did show a real synchronous fast-path advantage for a `ValueTask`-backed candidate, especially on composed success paths. The gain was not enough to outweigh the correctness and maintenance risks of a stored `ValueTask` backbone.

## Consequences

- `TaskFlow.run` continues to normalize to `Task<Result<'value,'error>>`.
- `ValueTask` support stays in `TaskFlow` and `ColdTask` interop.
- Revisit a dedicated `valueTaskFlow` only if future benchmarks show a durable win that survives the API and documentation cost.
