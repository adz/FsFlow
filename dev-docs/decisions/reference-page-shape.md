# API Reference Page Shape

## Extracted From

- `dev-docs/TASKS.md`
- `dev-docs/PLAN.md`

## Source Date

- 2026-05-03: docs audit for per-API reference coverage

## Decision

FsFlow documents every public API surface with its own reference page.

The reference shape is:

- one page per public type, module, computation expression, or runtime helper surface
- one concise package hub per package
- one side-menu entry per public page
- one explaining example on each page
- one source link on each page

Documentation rules:

- **Source-driven:** Lift the reference page content from the XML doc comments so the rendered docs and IDE experience stay in sync.
- **FsToolkit-style:** Keep each page focused: short summary, explaining example, member map, and source link.
- **Validation story:** Lead with `Check`, `Diagnostics`, `Validation`, and applicative `validate {}`.
- **Zero legacy:** Remove `Validate`-as-primary or `Validate`-as-compatibility language from narrative docs. Compatibility aliases are removed before 1.0.

Versioning rules:

- released docs link source pages to the matching release snapshot
- unreleased docs link source pages to `main`
- versioned docs stay aligned with the tagged release tree

## Why

- A page-per-surface layout makes the reference easier to scan.
- Side-menu visibility prevents important APIs from being buried behind a package hub.
- Versioned source links make the docs auditable against the exact code release.
- `main` links for next docs keep unreleased pages useful without pretending they are frozen.

## Consequences

- The current reference pages should be split until every public surface is directly reachable.
- Package hubs should stay short and delegate to the per-surface pages.
- The docs generation workflow must preserve versioned and next-branch source links.
- Public docs should not rely on narrative pages to stand in for missing API pages.
