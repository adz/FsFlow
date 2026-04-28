# FsFlow Tasks

This backlog is driven by one question:

"How do we make the published docs feel like a polished product site, with versioned API reference, executable examples, and a short README, rather than a generated dump?"

## Active Backlog

1. [x] Move the docs site onto a polished, versioned Docusaurus frontend.
   - Keep the existing guide set and present it as a clean reading path.
   - Give `FsFlow` and `FsFlow.Net` separate API home pages.
   - Make the site feel like a reference manual instead of a blog or portal.
2. [x] Define and publish the first versioned docs snapshot.
   - Keep a stable latest site while preserving release-tagged docs.
   - Expose the version switcher in the site shell.
   - Keep canonical links and source-aware edit links working.
3. [ ] Raise API docstring quality to reference-manual level.
   - Define a docstring rubric for summaries, signatures, remarks, examples, and `seealso`.
   - Rewrite the public surface comments to match it.
   - Ensure core combinators, bridges, and tricky edge cases have examples and precise wording.
   - Make the generated reference read like FsToolkit-level API docs or better.
   - Wire a real source-doc extraction pass so the API pages are generated from code comments, with source links preserved.
   - See `dev-docs/decisions/docs-source-extraction.md` for the current decision note.
4. [ ] Expand executable documentation examples into docs-as-tests.
   - Keep example projects runnable during docs generation.
   - Capture evaluated output back into generated docs so the rendered page shows real results.
   - Use `Unquote` or similar assertions for examples that should stay green.
   - Prefer examples that reuse real library types and realistic app shapes.
5. [ ] Tighten docs release automation.
   - Make docs generation part of release and tag workflows.
   - Publish docs artifacts for each release and validate the current site on pull requests.
   - Add link checking and a docs preview path if practical.
   - Ensure release notes and docs versioning stay in sync.
6. [ ] Trim the README into a short motivational entry point.
   - Keep the value proposition, install/use snippet, and link to docs.
   - Remove most tutorial-level content from the README and move it into docs pages.
   - Preserve the README as the NuGet-facing landing page.
   - Keep it motivating and concise, not exhaustive.
7. [ ] Add reader-env `yield` ergonomics to the builder surface.
   - Allow `yield _.Field` to project from the reader environment.
   - Mirror the same pattern in async and task builders.
   - Keep `Flow.read` as the explicit API and document the ambiguity around function-valued yields.
   - See `dev-docs/decisions/reader-env-yield.md` for the current decision note.

## Deferred

1. [ ] Add head-to-head benchmark peers for `FsToolkit.ErrorHandling`, `Ply`, and `IcedTasks` so the abstraction tax can be compared against the closest ecosystem alternatives.
2. [ ] Extend the benchmark suite so the same scenarios can be run consistently across both `Task` and `ValueTask` backbones where that comparison is meaningful.

## Completed Work

- Core workflow split: `Flow`, `AsyncFlow`, and `TaskFlow` are separated, with package boundaries and CE surfaces aligned to that split.
- Cold execution model: `ColdTask<'value>` replaced the older aliases, and the docs explain hot versus cold lifting.
- Builder coverage: async, task, value-task, option, and `ColdTask` interop are implemented on the relevant builders.
- ValueTask decision: the task backbone was benchmarked, the risks were evaluated, and `TaskFlow` remains `Task`-backed for now.
- Docs: the user-facing docs now explain the workflow family, semantics, migration path, and benchmark story.
- Docs site: the site now builds with Docusaurus, has package-oriented API hubs, and includes generated runnable examples.
- Docs tooling: `scripts/build-docs-site.sh`, `scripts/preview-docs.sh`, and `scripts/generate-example-docs.sh` drive the site build and example generation.
- Benchmarks: the suite now uses BenchmarkDotNet, includes shared scenario helpers, and publishes results for reader overhead, railway short-circuiting, composition depth, cancellation flow, and synchronous completion.

## Done Means

This backlog is done when:

- the docs read like product documentation for the user
- the API reference is useful without opening the source
- semantic edge cases are documented and tested
- the project feels like a maintained library, not a design notebook
