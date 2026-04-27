# FsFlow Plan

This file is for internal planning and design rationale. It is not user-facing documentation.

`TASKS.md` is the executable backlog.
`PLAN.md` captures the architectural direction, rationale, tradeoffs, and release checklist.

## Current Direction

The next major iteration should stop treating one `Async<Result<_,_>>`-based `Flow` abstraction as the center of every scenario.

The target model is:

- `Flow<'env,'error,'value>` for sync/result-oriented work with no cancellation token in the representation
- `AsyncFlow<'env,'error,'value>` for `Async`-based workflows in core `FsFlow`
- `TaskFlow<'env,'error,'value>` for `.NET` task-based workflows in `FsFlow.Net`

This split is driven by three goals:

- preserve a strong Fable/core story without task-oriented types in the base package
- allow a task-oriented `.NET` workflow with better performance characteristics
- avoid forcing sync flows to carry async runtime concerns such as `CancellationToken`

## Package Boundary

The intended package split is:

- `FsFlow`
  - `Flow`
  - `AsyncFlow`
  - `flow`
  - `asyncFlow`
  - sync and async combinators
  - cross-target logging abstraction
- `FsFlow.Net`
  - `TaskFlow`
  - `taskFlow`
  - `ColdTask<'value>`
  - task and value-task interop
  - `.NET`-specific runtime helpers
  - `ILogger` adapters and conveniences

Task-oriented concepts should not appear in the public contract of core `FsFlow`.

## Workflow Semantics

The workflow types should be cold and restartable abstractions.

- `Flow` should rerun from scratch on each execution
- `AsyncFlow` should rerun from scratch on each execution
- `TaskFlow` should rerun from scratch on each execution

Hot `Task` and `ValueTask` inputs are interop conveniences, not the semantic identity of the workflow.

When a workflow binds an already-started `Task` or `ValueTask`:

- rerunning the workflow re-awaits the same started work
- the effect is not re-executed
- the current workflow `CancellationToken` cannot be injected into that work

When a workflow binds a `ColdTask<'value>`:

- rerunning the workflow invokes the factory again
- the effect can run again from scratch
- the current workflow `CancellationToken` can be passed in

This distinction must be documented clearly.

## ColdTask

`ColdTask` should remain a focused concept.

The intended public meaning is:

- `ColdTask<'value>` represents delayed `CancellationToken -> Task<'value>`
- it is nominal, not just a type alias
- `ColdTask<Result<'value,'error>>` is the typed-failure cold-task shape
- there should be no separate `ColdTaskResult` abstraction

`ColdTask` should be the preferred interop shape when restartability and runtime token fidelity matter.

`ColdTask` does not guarantee meaningful cancellation behavior. An implementation may ignore the token and still be a valid cold task because it remains deferred and restartable.

## Bind Surface

The intended bind surface is:

- `flow {}`
  - plain values
  - `Result<'value,'error>`
  - sync-only helpers
- `asyncFlow {}`
  - `Flow`
  - `Async<'value>`
  - `Async<Result<'value,'error>>`
  - and, when `FsFlow.Net` is referenced, task-oriented lifts
- `taskFlow {}`
  - `Flow`
  - `Async<'value>`
  - `Async<Result<'value,'error>>`
  - `Task`
  - `Task<'value>`
  - `Task<Result<'value,'error>>`
  - `ValueTask`
  - `ValueTask<'value>`
  - `ValueTask<Result<'value,'error>>`
  - `ColdTask<'value>`
  - `ColdTask<Result<'value,'error>>`

The sync builder should no longer directly bind task-oriented shapes.

## ValueTask Direction

`ValueTask` should be treated as a first-class input in `.NET` builders.

Open question:

- should `TaskFlow` remain internally `Task`-backed
- or should some part of the backbone become `ValueTask`-based

Current view:

- do not create a separate `valueTaskFlow` unless benchmarking and ergonomics clearly justify it
- separate workflow types by semantics, not by transport optimization alone
- be careful not to make `ValueTask` the default backbone unless the benefits are demonstrated and the single-await/storage pitfalls are acceptable

Important distinction:

- returning or running as `ValueTask` is a boundary/API choice
- being based on `ValueTask` is an internal semantic and implementation commitment

## Option And ValueOption

`Option<'value>` and `ValueOption<'value>` should be short-circuiting inputs.

Current direction:

- allow implicit option binding only when the workflow error type is `unit`
- provide explicit adapters when the caller wants a custom error value
- avoid using `null` as an implicit error payload

## Logging And Capabilities

Logging should work in core across Fable and all supported targets.

The problem is not logging itself. The problem is coupling the core contract to `.NET`-specific `ILogger`.

Current direction:

- keep a cross-target logging abstraction in core `FsFlow`
- add `.NET` adapters and conveniences in `FsFlow.Net`
- make the DX as close as possible to `logError "Text"`

The design pressure here is really about environment representation and capability expression.

Key observations:

- logging is a capability, not a reason to add Writer semantics to the workflow core
- Writer-style accumulation is a different concern from immediate operational logging
- environment shape should make capabilities explicit

Likely design direction:

- core logging helpers should work against a library-owned logging abstraction
- env representation should make the logging capability explicit
- projection-based helpers should remain available
- `.NET` can add `ILogger` adapters and convenience helpers

Anonymous records may be useful for user env composition, but they are not by themselves a full capability-container strategy. They are best treated as one env-shaping option rather than the entire abstraction story.

## IcedTasks

`IcedTasks` is best treated as an optional integration target, not as the foundation of the public model.

Useful takeaways:

- its taxonomy distinguishes hot vs cold and cancellable vs non-cancellable task forms
- its lambda-based cancellable task shape is semantically close to the intended `ColdTask`

Reservations:

- much of the taxonomy is alias-based rather than nominal
- the public model of `FsFlow` should stay under local control

If support is added, it should likely be via an optional `FsFlow.IcedTasks` package rather than by making IcedTasks foundational to `FsFlow` or `FsFlow.Net`.

## Release Checklist

Use this checklist before cutting a public release.

### Documentation

- `README.md` still matches the current public API and package positioning
- main docs pages build cleanly through `bash scripts/generate-api-docs.sh`
- generated API reference renders correctly under `output/reference/`
- examples page still points to runnable example projects
- release notes include the version being shipped

### Examples

- main example still runs
- maintenance example still runs
- playground example still shows the smallest current surface honestly
- migration guide still reflects the recommended adoption path

### API Docs And Packaging

- `dotnet pack src/FsFlow/FsFlow.fsproj --configuration Release` succeeds
- package metadata points at `adz/FsFlow`
- packed `README.md` is still suitable for the NuGet package page
- symbol package is produced alongside the main package

### Semantic Edge Cases

- timeout behavior is still documented and covered by tests
- exception capture behavior is still documented and covered by tests
- cleanup on success, typed failure, and cancellation is still covered by tests
- retry attempt semantics are still documented and covered by tests

### Release

- CI is green on `main`
- GitHub Pages deployment is healthy
- release tag matches the package version
- release artifacts include `.nupkg` and `.snupkg`
- NuGet publish is completed
