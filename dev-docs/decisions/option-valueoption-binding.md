# Option and ValueOption Binding

Status: decided.
Recorded: 2026-05-03.

## Extracted From

- `dev-docs/PLAN.md`:
  - `Option<'value>` and `ValueOption<'value>` short-circuit behavior and implicit binding rules

## Source Date

- 2026-05-03: task 22 decision

## Decision

Keep implicit binding for `option<'value>` and `voption<'value>` only when the workflow error type is `unit`.
For any typed error workflow, require explicit conversion helpers such as `Flow.fromOption`, `AsyncFlow.fromOption`, `TaskFlow.fromOption`, and the `ValueOption` equivalents.

## Why

- `unit`-error workflows are the natural shape for guard-style optional inputs.
- Typed errors should stay explicit so the chosen failure value is visible at the call site.
- The split matches the existing `Result`-first design: direct binding is for a narrow ergonomic path, while explicit adapters express intent when the error payload matters.
- This keeps `option`/`voption` semantics aligned across `Flow`, `AsyncFlow`, and `TaskFlow` without adding a separate implicit conversion path for typed errors.

## Consequences

- `flow {}`, `asyncFlow {}`, and `taskFlow {}` continue to accept `Some` / `None` and `ValueSome` / `ValueNone` directly only when the error type is `unit`.
- Documentation should point typed-error callers to explicit adapters instead of suggesting broader implicit binding.
- The current tests for unit-error binding and typed-error adapters remain the expected behavior.

