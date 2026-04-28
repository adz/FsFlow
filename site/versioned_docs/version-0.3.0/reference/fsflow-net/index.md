---
title: Overview
description: Landing page for the task-oriented FsFlow API surface.
---
# FsFlow.Net

This page shows the task-oriented FsFlow surface by what you do with it, not by namespace.

Use this page when the boundary is task-based or you need task-native interop.

## What belongs here

This package covers the task-oriented branch of the library:

- `TaskFlow<'env, 'error, 'value>` and the `TaskFlow` module
- `ColdTask<'value>` for delayed task work
- task-native interop with `Task`, `ValueTask`, `Async`, and `ColdTask`
- the `taskFlow {}` entry point for task-shaped orchestration

Treat the builder as syntax. Treat the module and type pages as the actual API.

<div class="docs-grid">

<section class="docs-card">
<span class="label">TaskFlow</span>
<h2>Task boundaries</h2>
<p>The `TaskFlow&lt;'env, 'error, 'value&gt;` type and the `TaskFlow` module cover task-oriented composition, environment reads, cancellation, and execution.</p>
<p>The next page expands the full member map, including creation helpers, composition helpers, environment access, traversals, runtime helpers, and task bridges.</p>
<div class="docs-card-links">
<a href="taskflow">Open the TaskFlow page</a>
</div>
</section>

<section class="docs-card">
<span class="label">ColdTask</span>
<h2>Delayed task work</h2>
<p>`ColdTask&lt;'value&gt;` represents delayed task work that starts when the boundary runs and can observe the runtime cancellation token.</p>
<p>The next page explains the cold/hot distinction and the creation helpers in more detail.</p>
<div class="docs-card-links">
<a href="coldtask">Open the ColdTask page</a>
</div>
</section>

<section class="docs-card">
<span class="label">Interop</span>
<h2>Task and async bridges</h2>
<p>The `taskFlow {}` and `asyncFlow {}` task-aware entry points adapt `Task`, `ValueTask`, `Async`, and `ColdTask` inputs when they are the honest runtime shape.</p>
<p>The next page details the exact bridge points and the builder extensions that are intentionally public.</p>
<div class="docs-card-links"><a href="interop">Open the interop page</a></div>
</section>

</div>

## Read the subpages

<div class="docs-grid">

<section class="docs-card">
<span class="label">TaskFlow</span>
<h3><a href="taskflow">Task boundaries</a></h3>
<p>Type, module, composition, environment, cancellation, and execution in the task family.</p>
</section>

<section class="docs-card">
<span class="label">ColdTask</span>
<h3><a href="coldtask">Delayed task work</a></h3>
<p>The cancellation-aware task helper surface used by `TaskFlow` and the interop builders.</p>
</section>

<section class="docs-card">
<span class="label">Interop</span>
<h3><a href="interop">Task and async bridges</a></h3>
<p>The bridge surface that adapts `Task`, `ValueTask`, `Async`, and `ColdTask` inputs without losing the task shape.</p>
</section>

</div>
