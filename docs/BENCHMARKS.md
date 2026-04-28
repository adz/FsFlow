# Benchmarks

Read this page when you want to understand what FsFlow measures against, what each benchmark is trying to prove, and how to rerun the suite locally.

The point of these benchmarks is not to pretend FsFlow has zero overhead.

The point is to measure the abstraction tax honestly:

- environment passing through `localEnv`
- typed short-circuiting through `Result`
- cold execution for task-oriented workflows
- implicit cancellation-token flow
- the cost of staying `Task`-backed instead of moving the backbone to `ValueTask`

## What The Suite Measures

The benchmark project lives at `benchmarks/FsFlow.Benchmarks`.

It currently covers these scenarios:

- `ReaderOverheadBenchmarks`
  Measures 10 nested environment projections.
  Compares `TaskFlow.localEnv`, manual environment passing, and `AsyncLocal` updates.
- `AsyncRailwayBenchmarks`
  Measures 20-step `Async<Result<_,_>>`-style short-circuiting.
  Compares `AsyncFlow` with a direct manual `Async<Result<_,_>>` chain.
- `TaskRailwayBenchmarks`
  Measures the same 20-step short-circuiting shape for task-oriented workflows.
  Compares `TaskFlow` with a direct manual `Task<Result<_,_>>` chain.
- `CompositionChainBenchmarks`
  Measures 100-step composition depth.
  Covers `Flow`, `AsyncFlow`, `TaskFlow`, direct `Async<Result<_,_>>`, direct `Task<Result<_,_>>`, and raw `Task`.
- `CancellationFlowBenchmarks`
  Measures the steady-state cost of checking cancellation at 5 points.
  Compares `TaskFlow` implicit token propagation with explicit token threading in a manual `Task<Result<_,_>>` workflow.
- `SynchronousCompletionBenchmarks`
  Measures already-completed task-oriented success paths.
  Compares the current `TaskFlow` backbone with the local candidate `ValueTask` prototype used for the backbone decision.

## Recorded Results

Recorded on 2026-04-28 with:

- BenchmarkDotNet `0.15.8`
- `.NET SDK 10.0.201`
- host runtime `.NET 10.0.5`
- Linux Fedora 43
- Intel Core i5-10310U

This first published run uses an in-process BenchmarkDotNet job with `3` warmup iterations and `3` measured iterations.

That is enough to make the cost model visible, but not enough to claim final absolute numbers.
Treat the current tables as directional and repeat them on release hardware before using them in marketing material or release notes.

### `Async<Result<_,_>>` Migration Cost

20 bind steps with short-circuit at step `1` or step `20`:

| Workflow | FailAt | Mean | Allocated |
| --- | ---: | ---: | ---: |
| Direct `Async<Result<_,_>>` | 1 | `6.733 us` | `1000 B` |
| `AsyncFlow` | 1 | `12.011 us` | `7977 B` |
| Direct `Async<Result<_,_>>` | 20 | `11.150 us` | `7393 B` |
| `AsyncFlow` | 20 | `13.159 us` | `9353 B` |

Interpretation:

- `AsyncFlow` currently pays a visible premium over direct `Async<Result<_,_>>`
- the premium is much larger on the earliest-failure path than on the near-success path
- the migration story is still defensible because the gap narrows when more of the workflow actually runs

### Task-Oriented Railway Cost

20 bind steps with short-circuit at step `1` or step `20`:

| Workflow | FailAt | Mean | Allocated |
| --- | ---: | ---: | ---: |
| Direct `Task<Result<_,_>>` | 1 | `179.2 ns` | `256 B` |
| `TaskFlow` | 1 | `2718.1 ns` | `5944 B` |
| Direct `Task<Result<_,_>>` | 20 | `1749.4 ns` | `3392 B` |
| `TaskFlow` | 20 | `3864.5 ns` | `7320 B` |

Interpretation:

- `TaskFlow` pays a much steeper tax on the earliest short-circuit path than a hand-written task chain
- the relative gap shrinks as more of the workflow executes
- the `Result` layer plus cold-composition machinery is a real cost center in task-oriented hot paths

### Reader Overhead

10 nested environment transformations:

| Workflow | Mean | Allocated |
| --- | ---: | ---: |
| Manual env passing | `105.7 ns` | `80 B` |
| `TaskFlow.localEnv` | `734.6 ns` | `1472 B` |
| `AsyncLocal` updates | `1147.4 ns` | `2000 B` |

Interpretation:

- manual explicit passing is the floor
- `TaskFlow.localEnv` is meaningfully slower than raw function passing
- `TaskFlow.localEnv` still beats mutating ambient state through `AsyncLocal` in this benchmark

### Synchronous Completion

Already-completed success path:

| Workflow | Mean | Allocated |
| --- | ---: | ---: |
| Candidate `ValueTask` backbone | `97.57 ns` | `96 B` |
| `TaskFlow` | `545.50 ns` | `928 B` |

Interpretation:

- the old conclusion still holds: a `ValueTask` backbone has a real synchronous fast-path advantage
- the benchmark justifies continued optimization work on the completed-success path
- it still does not override the correctness and reuse hazards of storing composed workflows as `ValueTask`

### Composition Depth And Raw `Task`

100 bind steps:

| Workflow | Mean | Allocated |
| --- | ---: | ---: |
| Raw `Task` | `4.593 us` | `13.92 KB` |
| Direct `Task<Result<_,_>>` | `6.264 us` | `17.05 KB` |
| `TaskFlow` | `14.412 us` | `34.45 KB` |
| Direct `Async<Result<_,_>>` | `18.604 us` | `33.64 KB` |
| `AsyncFlow` | `27.182 us` | `42.13 KB` |

Interpretation:

- the mandatory `Result` layer does add measurable cost over raw `Task`
- `TaskFlow` still beats the direct `Async<Result<_,_>>` chain in this 100-bind task-oriented scenario
- `TaskFlow` is roughly `3.14x` slower than raw `Task` here, which is a useful first estimate for the combined cold-task plus typed-failure tax

## How To Read The Results

Use the suite to answer concrete product questions:

- Is `AsyncFlow` close enough to direct `Async<Result<_,_>>` to justify the better environment model?
- How much extra cost does `TaskFlow` pay for typed failures compared with raw `Task`?
- Is `localEnv` materially cheaper or more expensive than mutating ambient state through `AsyncLocal`?
- Does implicit cancellation propagation stay within an acceptable constant-factor overhead?
- Is the synchronous-success gain from a `ValueTask` backbone large enough to outweigh its correctness and reuse hazards?

The important comparisons are usually relative, not absolute:

- `AsyncFlow` versus direct `Async<Result<_,_>>` for migration cost
- `TaskFlow` versus direct `Task<Result<_,_>>` for task-oriented abstraction tax
- `TaskFlow` versus raw `Task` for the cost of mandatory `Result`
- `TaskFlow.localEnv` versus manual passing and `AsyncLocal` for environment strategy

## Running The Suite

Build:

```bash
dotnet build benchmarks/FsFlow.Benchmarks/FsFlow.Benchmarks.fsproj -c Release
```

Run everything:

```bash
dotnet run -c Release --project benchmarks/FsFlow.Benchmarks/FsFlow.Benchmarks.fsproj -- --filter '*'
```

Run only the `Async<Result<_,_>>` migration comparison:

```bash
dotnet run -c Release --project benchmarks/FsFlow.Benchmarks/FsFlow.Benchmarks.fsproj -- --filter '*AsyncRailwayBenchmarks*'
```

Run only the task-oriented short-circuit comparison:

```bash
dotnet run -c Release --project benchmarks/FsFlow.Benchmarks/FsFlow.Benchmarks.fsproj -- --filter '*TaskRailwayBenchmarks*'
```

BenchmarkDotNet writes full reports under `BenchmarkDotNet.Artifacts`.

## Current Intent

Task 24 is not just "beat `Async<Result<_,_>>`."

It is to make the cost model legible:

- where FsFlow is close to direct code
- where the abstraction pays a real premium
- which premium buys a concrete semantic benefit
- which hot paths may justify future optimization work
