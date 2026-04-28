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

LLM progress:
The API pages were light because they were hand-authored hub pages, not a source-doc extraction
  pass. Docusaurus was only rendering the markdown we wrote in docs/reference/..., so the XML-style
  summaries from code were not being transcluded automatically.

  I’ve pushed the lifted summaries into the API docs now:

  - docs/reference/fsflow/flow.md
  - docs/reference/fsflow/asyncflow.md
  - docs/reference/fsflow-net/taskflow.md
  - docs/reference/fsflow/validate.md

  What’s in there now:

  - function-level summaries for the creation, composition, interop, and runtime helpers
  - a Source-Lifted Notes section on each page
  - the auto-lift matrix and the explicit-lift case in docs/TASK_ASYNC_INTEROP.md

  I also synced the versioned docs snapshot and rebuilt the site, so the 0.3.0 docs reflect the
  same changes.

  If you want the next step, I can wire a real source-doc extraction pass so the API pages are
  generated from the code comments instead of being maintained as lifted markdown by hand.


› yes you should write a real source-doc extracton pass, and we should retain links to source in
  the doc


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

7. [ ] consider yield to improve ergonomics on reader env



› i was hoping to use yield to yield from the reader env


• I’m checking the exact shape of what you’re asking before I answer: whether you want yield to be a synonym for return, or whether you want
  yield to mean “pull from the reader environment without spelling read.” Those are different, and only one of them is a small change.

• Yes. That is the one yield use that actually buys you something.

  You can make yield mean “project from the reader environment” by overloading the builder:

  type FlowBuilder with
      member _.Yield(value: 'value) : Flow<'env, 'error, 'value> =
          Flow.succeed value

      member _.Yield(project: 'env -> 'value) : Flow<'env, 'error, 'value> =
          Flow.read project

      member _.YieldFrom(flow: Flow<'env, 'error, 'value>) : Flow<'env, 'error, 'value> =
          flow

  Then this becomes valid:

  let wf : Flow<{ Name: string }, string, string> =
      flow {
          yield _.Name
      }

  and it behaves like:

  Flow.read _.Name

  You can mirror the same idea for AsyncFlowBuilder and TaskFlowBuilder:

  - Yield(project) maps to AsyncFlow.read project / TaskFlow.read project
  - YieldFrom(flow) maps to return! flow

  So the ergonomic pattern would be:

  flow {
      yield _.Prefix
      yield! Flow.read _.Name
  }

  A couple of caveats:

  - yield of a function value can become ambiguous, because functions are values too.
  - This is nonstandard CE style in F#, so it may surprise some users.
  - It helps most inside computation expressions; it does not replace Flow.read as the explicit API.


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
