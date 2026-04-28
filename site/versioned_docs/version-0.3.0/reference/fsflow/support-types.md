---
title: Support Types
description: Supporting types in the core FsFlow package.
---

# Support Types

This page shows the support types that are useful but not the headline boundary surface.

These types show up when you need runtime logging or retry behavior, but they stay out of the way the rest of the time.

## LogLevel

`LogLevel` is the runtime logging severity used by logging helpers.

- `Trace`
- `Debug`
- `Information`
- `Warning`
- `Error`
- `Critical`

## LogEntry

`LogEntry` is the structured log record used by runtime logging helpers.

- `Level`
- `Message`
- `TimestampUtc`

## RetryPolicy

`RetryPolicy<'error>` defines how retry helpers decide whether to keep going.

- `MaxAttempts`
- `Delay`
- `ShouldRetry`

The standard helper is:

- `RetryPolicy.noDelay`

## Member map

- `LogLevel` for structured severity
- `LogEntry` for structured runtime events
- `RetryPolicy<'error>` for retry decision and delay behavior

## Example

```fsharp
let policy =
    RetryPolicy.noDelay 3
```

## Source

- [Flow.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs)
