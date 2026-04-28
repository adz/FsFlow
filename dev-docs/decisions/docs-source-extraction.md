# Docs Source Extraction

Status: decided direction, implementation pending.
Recorded: 2026-04-29.

## Extracted From

- `dev-docs/TASKS.md`:
  - the `LLM progress` block around the API page work
  - the follow-up note requesting a real source-doc extraction pass and source links

## Source Date

- 2026-04-29: `Migrate to Docusaurus and prepare 0.3.0 release`

## Decision

Replace hand-authored lifted API pages with a real source-doc extraction pass.
Keep source links in the rendered docs.

## Why

- The API pages were being maintained as lifted markdown, not generated from code comments.
- That setup drifted away from the source of truth and made the reference harder to trust.
- Source-aware links make the reference easier to audit and keep current.

## Consequences

- API pages should transclude XML-style summaries and remarks from the implementation.
- Hand-written lifted notes should shrink to exceptions and cross-cutting commentary.
- The extraction pass must preserve links back to the source definitions.
