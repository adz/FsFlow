---
title: Home
description: FsFlow guides, examples, and API reference.
---

<div class="docs-home-hero">

<div class="docs-home-copy">

<span class="eyebrow">Boundary-first docs and runnable examples</span>

<p class="page-summary">If async/result composition has started to get in the way, FsFlow gives you a cleaner boundary to stand on. Start with the guides, run the examples, then open the API reference when you want the exact member behavior.</p>

# Make F# boundaries feel ordinary again.

<p class="lede">
FsFlow helps new F# users get past awkward async/result composition and gives experienced teams a Reader-style boundary for dependency access, typed failure, and task interop.
</p>

<div class="docs-home-meta">
<span class="docs-chip">Reader-style boundaries</span>
<span class="docs-chip">Async / Result</span>
<span class="docs-chip">Task interop</span>
<span class="docs-chip">Source-linked docs</span>
</div>

</div>

<aside class="docs-home-panel">
<section class="docs-home-panel-card">
<span class="label">What this site shows</span>
<strong>Guides for getting oriented, runnable examples for seeing the shape, and API reference for exact member behavior.</strong>
</section>

<section class="docs-home-panel-card">
<span class="label">Family model</span>
<strong>Flow, AsyncFlow, and TaskFlow with the same shape where it matters.</strong>
</section>

<section class="docs-home-panel-card">
<span class="label">Validation bridge</span>
<strong>Pure `Result` checks first, effectful error lookup only on failure.</strong>
</section>
</aside>

</div>

## One boundary, three runtimes

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

The same pattern keeps pure validation, dependency access, and typed failure in one place.

<div class="docs-grid">

<section class="docs-card">
<span class="label">Start here</span>
<h2><a href="GETTING_STARTED">Getting Started</a></h2>
<p>The fastest path from plain `Result` code to the right workflow family for a real application boundary.</p>
</section>

<section class="docs-card">
<span class="label">Examples</span>
<h2><a href="TINY_EXAMPLES">Tiny Examples</a></h2>
<p>Small runnable snippets for `flow {}`, `asyncFlow {}`, `taskFlow {}`, and `ColdTask` interop.</p>
</section>

<section class="docs-card">
<span class="label">Interop</span>
<h2><a href="TASK_ASYNC_INTEROP">Task And Async Interop</a></h2>
<p>The direct binding surface, including which shapes bind in each family and when to choose them.</p>
</section>

</div>

<div class="docs-grid">

<section class="docs-card">
<span class="label">Design</span>
<h3><a href="WHY_FSFLOW">Why FsFlow</a></h3>
<p>The motivation, tradeoffs, and the readable-before-adoption argument.</p>
</section>

<section class="docs-card">
<span class="label">Semantics</span>
<h3><a href="SEMANTICS">Semantics</a></h3>
<p>The rules for cold execution, runtime helpers, and the family boundary model.</p>
</section>

<section class="docs-card">
<span class="label">Integrations</span>
<h3><a href="INTEGRATIONS">Integrations</a></h3>
<p>How FsFlow fits beside `FsToolkit.ErrorHandling`, `Validus`, `IcedTasks`, and `FSharpPlus`.</p>
</section>

</div>

<div class="docs-stack">

<section class="docs-card">
<span class="label">Reference</span>
<h3><a href="reference/">API Reference</a></h3>
<p>The package hubs for <code>FsFlow</code> and <code>FsFlow.Net</code>, with user-interest landing pages beneath each hub.</p>
</section>

<section class="docs-card">
<span class="label">Examples</span>
<h3><a href="examples/">Runnable examples</a></h3>
<p>Application-shaped examples that demonstrate the intended library use in the repository itself.</p>
</section>

<section class="docs-card">
<span class="label">More</span>
<h3><a href="BENCHMARKS">Benchmarks</a></h3>
<p>Published measurements for the abstraction cost, rerun behavior, and task-oriented tradeoffs.</p>
</section>

</div>
