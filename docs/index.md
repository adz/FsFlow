---
title: Home
description: FsFlow technical guides, semantics, and API reference.
---

<div class="docs-home-hero">

<div class="docs-home-copy">

<span class="eyebrow">Typed results, explicit context, and async/task interop</span>

<p class="page-summary">Standardize application boundaries with a unified model for dependency access, typed failure, and runtime interop. `flow { ... }` combines `Result` with an added `'env` for explicit context, while `asyncFlow { ... }` and `taskFlow { ... }` extend the same style to F# `Async` and .NET `Task`.</p>

# Type-safe, effectful boundaries for F#.

<p class="lede">
FsFlow provides a lightweight abstraction for the `Env -> Result<'T, 'E>` shape across sync, async, and task-based code. It preserves cold execution and rerun behavior while simplifying mixed-runtime composition without hiding execution details.
</p>

<div class="docs-home-meta">
<span class="docs-chip">Reader-style environment</span>
<span class="docs-chip">Typed failure (Result)</span>
<span class="docs-chip">Async / Task interop</span>
<span class="docs-chip">Cold execution semantics</span>
</div>

</div>

<aside class="docs-home-panel">
<section class="docs-home-panel-card">
<span class="label">Documentation Model</span>
<strong>Technical guides for architectural integration, semantic rules for execution, and comprehensive API reference.</strong>
</section>

<section class="docs-home-panel-card">
<span class="label">Unified API Surface</span>
<strong>`Flow`, `AsyncFlow`, and `TaskFlow` share an aligned member surface for mapping, binding, and env access.</strong>
</section>

<section class="docs-home-panel-card">
<span class="label">Zero-Wrapper Pure Validation</span>
<strong>Bind plain `Result<'T, 'E>` directly into any computation family without additional lifting ceremony.</strong>
</section>
</aside>

</div>

## Explicit boundaries, three runtimes

```fsharp
type AppEnv =
    { Prefix: string }

type AppError =
    | MissingName

let validateName name =
    if System.String.IsNullOrWhiteSpace name then
        Error MissingName
    else
        Ok name

let greet input : Flow<AppEnv, AppError, string> =
    flow {
        let! name = validateName input
        let! prefix = Flow.read _.Prefix
        return prefix + " " + name
    }
```

The FsFlow model aligns pure validation, environment access, and typed failure across synchronous and asynchronous boundaries.

<div class="docs-grid">

<section class="docs-card">
<span class="label">Fundamentals</span>
<h2><a href="GETTING_STARTED">Getting Started</a></h2>
<p>An overview of the `Flow`, `AsyncFlow`, and `TaskFlow` computation families and when to choose each runtime.</p>
</section>

<section class="docs-card">
<span class="label">Patterns</span>
<h2><a href="TINY_EXAMPLES">Common Shapes</a></h2>
<p>Technical reference for `flow {}`, `asyncFlow {}`, `taskFlow {}`, and `ColdTask` composition patterns.</p>
</section>

<section class="docs-card">
<span class="label">Boundaries</span>
<h2><a href="TASK_ASYNC_INTEROP">Runtime Interop</a></h2>
<p>Type-level interop rules for binding `Async`, `Task`, `ValueTask`, and `Result` across different families.</p>
</section>

</div>

<div class="docs-grid">

<section class="docs-card">
<span class="label">Architecture</span>
<h3><a href="WHY_FSFLOW">Design Rationale</a></h3>
<p>The motivation for Reader-style boundaries and the tradeoffs compared to manual dependency threading.</p>
</section>

<section class="docs-card">
<span class="label">Runtime</span>
<h3><a href="SEMANTICS">Execution Semantics</a></h3>
<p>Detailed rules for cold execution, rerun behavior, exception handling, and cancellation propagation.</p>
</section>

<section class="docs-card">
<span class="label">Ecosystem</span>
<h3><a href="INTEGRATIONS">Integrations</a></h3>
<p>Compatibility surface with `FsToolkit.ErrorHandling`, `Validus`, `IcedTasks`, and `FSharpPlus`.</p>
</section>

</div>

<div class="docs-stack">

<section class="docs-card">
<span class="label">Reference</span>
<h3><a href="reference/">API Reference</a></h3>
<p>Complete member documentation for the <code>FsFlow</code> and <code>FsFlow.Net</code> packages.</p>
</section>

<section class="docs-card">
<span class="label">Implementation</span>
<h3><a href="examples/">Reference Examples</a></h3>
<p>Full application-shaped programs demonstrating library usage within complex orchestration layers.</p>
</section>

<section class="docs-card">
<span class="label">Performance</span>
<h3><a href="BENCHMARKS">Benchmarks</a></h3>
<p>Empirical measurements of abstraction overhead, allocation costs, and task-oriented execution tradeoffs.</p>
</section>

</div>
