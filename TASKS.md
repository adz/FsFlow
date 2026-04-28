# FsFlow Tasks

This backlog is driven by one question:

"Why would an F# developer, comparing this with `Async<Result<_,_>>` plus FsToolkit, decide not to adopt it?"

The current answer is not mainly "the core idea is bad." The current answer is that the project still feels under-explained, under-proven, and unevenly polished.

## Active Backlog

1. [ ] Decide whether the direct `Async<Result<_,_>>` migration story needs more helpers beyond the current `Flow.fromAsyncResult` and `Flow.toAsyncResult` shape after the redesign lands.
2. [ ] Add head-to-head benchmark peers for `FsToolkit.ErrorHandling`, `Ply`, and `IcedTasks` so the abstraction tax can be compared against the closest ecosystem alternatives.
3. [ ] Extend the benchmark suite so the same scenarios can be run consistently across both `Task` and `ValueTask` backbones where that comparison is meaningful.

## Completed Work

- Core workflow split: `Flow`, `AsyncFlow`, and `TaskFlow` are separated, with package boundaries and CE surfaces aligned to that split.
- Cold execution model: `ColdTask<'value>` replaced the older aliases, and the docs explain hot versus cold lifting.
- Builder coverage: async, task, value-task, option, and `ColdTask` interop are implemented on the relevant builders.
- ValueTask decision: the task backbone was benchmarked, the risks were evaluated, and `TaskFlow` remains `Task`-backed for now.
- Docs: the user-facing docs now explain the workflow family, semantics, migration path, and benchmark story.
- Benchmarks: the suite now uses BenchmarkDotNet, includes shared scenario helpers, and publishes results for reader overhead, railway short-circuiting, composition depth, cancellation flow, and synchronous completion.

## Done Means

This backlog is done when:

- the docs read like product documentation for the user
- the API reference is useful without opening the source
- semantic edge cases are documented and tested
- the project feels like a maintained library, not a design notebook
