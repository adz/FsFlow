---
title: Capability
description: Source-documented capabilities and layers for FsFlow.
---

# Capability

This page shows the source-documented capability and layer surface, used for dependency injection and environment management in task workflows.

## Capabilities

- module `Capability`: Capability helpers for record-based environments and .NET service-provider interop. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L633)
- `Capability.MissingCapability`: Describes a missing service-provider capability. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L635)
- `Capability.service`: Reads a capability from a record-based environment projection. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L642)
- `Capability.runtime`: Reads a capability from the runtime half of a two-context runtime environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L646)
- `Capability.environment`: Reads a capability from the application half of a two-context runtime environment. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L652)
- `Capability.serviceFromProvider`: Reads a service from `IServiceProvider` and fails when it is not registered. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L658)

## Layers

- type `Layer`: Layer helpers for deriving an environment in one flow and consuming it in another. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs#L675)

## Source

- [TaskFlow.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/TaskFlow.fs)
