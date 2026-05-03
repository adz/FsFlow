# Repository Instructions

This file is for agent instructions, not user-facing documentation.

Keep a strict split between:

- agent instructions for contributors and coding agents
- user-facing documentation for library users

Do not put agent guidance in `README.md` or under `docs/`.

When writing or editing user-facing docs, follow the documentation guide in [`dev-docs/DOCS.md`](dev-docs/DOCS.md).

Refer to [`dev-docs/PLAN.md`](dev-docs/PLAN.md) for architectural direction and [`dev-docs/TASKS.md`](dev-docs/TASKS.md) for the active backlog.

## Versioning and Compatibility

- **Before 1.0:** Bravely iterate. Remove old APIs and "old ways" immediately when a better alternative is established. Do not maintain compatibility aliases or stale patterns.
- **Post 1.0:** Standard semantic versioning applies. Maintain compatibility and use deprecation cycles for breaking changes.

## Documentation Integrity

- **Always Validate:** Every change to the codebase or documentation must be followed by a full documentation build to ensure no broken links or rendering crashes.
- **Build and Preview:** Run `./scripts/generate-api-docs.sh` and then verify the site build (e.g., `npm run build` inside the `site` directory).
- **Broken Link Check:** Docusaurus will fail the build on broken links. Do not ignore these errors.
- **MDX Safety:** Ensure all code examples are wrapped in triple-backticks with language hints to prevent MDX execution crashes.

