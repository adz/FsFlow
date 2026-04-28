# Benchmark History

Status: archived reference.
Recorded: 2026-04-28.

## Extracted From

- `dev-docs/PLAN.md`:
  - `Task 20 benchmark notes`
  - `Task 20 recorded run`
  - `Task 24 direction`
  - `Task 24 recorded run`

## Source Dates

- 2026-04-28: `Add Task vs ValueTask backbone benchmark`
- 2026-04-28: `Restore runtime helpers and benchmark suite`

## Task 20

Recorded on 2026-04-28 with `.NET 10.0.5`, using 2,000,000 iterations per scenario and the median of 5 measured runs after warmup.

Comparison:

- current `TaskFlow` backbone
- local `ValueTask<Result<_,_>>` candidate

Headline results:

- `run/succeed`: `TaskFlow` `27.46 ns/op`, `80 B/op`; candidate `23.74 ns/op`, `0 B/op`
- `map` chain: `TaskFlow` `301.19 ns/op`, `848 B/op`; candidate `48.48 ns/op`, `0 B/op`
- `bind` chain: `TaskFlow` `475.12 ns/op`, `1392 B/op`; candidate `101.25 ns/op`, `192 B/op`
- short computation-expression chain: `TaskFlow` `440.95 ns/op`, `1344 B/op`; candidate `107.08 ns/op`, `256 B/op`

Conclusion:

- the candidate had a meaningful synchronous fast-path advantage
- the benchmark alone did not justify changing the backbone
- the correctness model still dominated the decision

## Task 24

BenchmarkDotNet replaced ad hoc stopwatch loops.

Published comparisons:

- `AsyncFlow` versus direct `Async<Result<_,_>>`
- `TaskFlow` versus direct `Task<Result<_,_>>`
- `TaskFlow` versus raw `Task`
- `TaskFlow.localEnv` versus manual env passing and `AsyncLocal`

First conclusions:

- `AsyncFlow` was slower than direct `Async<Result<_,_>>`, with a larger gap on the earliest-failure path
- `TaskFlow` was slower than direct `Task<Result<_,_>>`, with the largest gap on the earliest-failure path
- `TaskFlow.localEnv` was slower than manual env passing but faster than `AsyncLocal` mutation in the measured reader scenario
- the candidate `ValueTask` backbone still showed a large synchronous-completion advantage over the current `TaskFlow` backbone
