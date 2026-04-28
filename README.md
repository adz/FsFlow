# FsFlow

<picture>
  <source media="(prefers-color-scheme: dark)" srcset="docs/content/img/fsflow-readme-dark.svg">
  <source media="(prefers-color-scheme: light)" srcset="docs/content/img/fsflow-readme-light.svg">
  <img alt="FsFlow" src="docs/content/img/fsflow-readme-light.svg" width="160">
</picture>

FsFlow keeps F# application workflows cold, typed, and explicit.

Use it when dependency threading, async work, and expected failures all show up in the same use case,
but you still want the code to read like ordinary F#.

[![ci](https://github.com/adz/FsFlow/actions/workflows/ci.yml/badge.svg)](https://github.com/adz/FsFlow/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/FsFlow.svg)](https://www.nuget.org/packages/FsFlow)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](LICENSE)

Start with the docs site: [adz.github.io/FsFlow](https://adz.github.io/FsFlow/)

## Why FsFlow

- Keep dependencies visible instead of hiding them in globals or service locators
- Keep expected failures in the type instead of pushing them into exceptions
- Keep the runtime shape honest, whether that shape is sync, `Async`, or `Task`
- Use one family that can fit a booted app environment, explicit feature dependencies, or a conventional `.NET` host

## Start Here

- [`docs/TINY_EXAMPLES.md`](docs/TINY_EXAMPLES.md) for the smallest runnable snippets
- [`docs/ARCHITECTURAL_STYLES.md`](docs/ARCHITECTURAL_STYLES.md) for the supported architecture shapes
- [`docs/GETTING_STARTED.md`](docs/GETTING_STARTED.md) for the workflow-family overview
- [`docs/INTEGRATIONS_FSTOOLKIT.md`](docs/INTEGRATIONS_FSTOOLKIT.md) for FsToolkit coexistence and migration

## Install

- `FsFlow` for `Flow` and `AsyncFlow`
- `FsFlow.Net` for `TaskFlow`

## What You Get

- `Flow` for synchronous workflows
- `AsyncFlow` for `Async`-based workflows in the core package
- `TaskFlow` for `.NET Task`-based workflows in `FsFlow.Net`
