# Reader-Env Yield

Status: accepted direction.
Recorded: 2026-04-29.

## Extracted From

- `dev-docs/TASKS.md`:
  - the `yield` ergonomics chat around reader environments

## Source Date

- 2026-04-29: `Migrate to Docusaurus and prepare 0.3.0 release`

## Decision

Allow `yield` inside reader-style computation expressions to project from the environment.

## Shape

- `Yield(value)` keeps plain-value yielding.
- `Yield(project: 'env -> 'value)` maps to `Flow.read project`.
- `YieldFrom(flow)` remains the normal flow passthrough.

## Why

- `yield _.Field` is a compact shorthand for reader projection.
- It improves ergonomics inside CE blocks without replacing `Flow.read`.

## Caveats

- Functions are values, so `yield` of a function can be ambiguous.
- This is nonstandard F# CE style and needs explicit documentation.
- It is an ergonomic shorthand, not a replacement for the explicit API.

## Consequences

- Mirror the same pattern in `AsyncFlowBuilder` and `TaskFlowBuilder`.
- Keep `Flow.read` as the canonical explicit entry point.
