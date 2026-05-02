# Unified Flow API (STRP + Two-Context Model)

Status: Proposal / Design Direction
Author: Gemini CLI
Date: 2026-05-03

## The Vision: "Invisible" Orchestration

The goal of the Unified Flow API is to make the distinction between synchronous, `Async`, and `.NET Task` boundaries vanish during orchestration. 

Instead of choosing between `Flow.map`, `AsyncFlow.map`, and `TaskFlow.map`, the user uses a single **`Flow` module** with curried functions. The compiler resolves the correct implementation using Static Resolved Type Parameters (STRP).

## 1. The Two-Context Model

Every workflow type now accepts two distinct inputs:

| Context | Responsibility | Example Content |
| :--- | :--- | :--- |
| **`RuntimeConfig`** | System/Operational concerns | Logger, Clock, TraceID, Metrics |
| **`'env`** | Application dependencies | Database, HTTP Clients, Domain Services |

### The Core Types
The internal representation of the three families becomes consistent:

*   **`Flow<'env, 'e, 'v>`**: `RuntimeConfig -> 'env -> Result<'v, 'e>`
*   **`AsyncFlow<'env, 'e, 'v>`**: `RuntimeConfig -> 'env -> Async<Result<'v, 'e>>`
*   **`TaskFlow<'env, 'e, 'v>`**: `RuntimeConfig -> 'env -> CancellationToken -> Task<Result<'v, 'e>>`

> *Note: `TaskFlow` remains the only family to accept a `CancellationToken` explicitly, preserving .NET-native cancellation semantics.*

---

## 2. The Unified `Flow` Module

The `Flow` module provides curried, pipeable functions that work across all three types.

### A. Universal Helpers (Works Everywhere)
These functions are available for `Flow`, `AsyncFlow`, and `TaskFlow`.

*   **Creation & Lifting:**
    *   `succeed`, `fail`, `value`: Create immediate terminals.
    *   `fromResult`: Lift a standard F# `Result`.
    *   `fromOption`, `fromValueOption`: Lift with an explicit error.
*   **Environment & Runtime:**
    *   `env`: Read the application dependencies (`'env`).
    *   `read`: Project a field from `'env` (e.g., `Flow.read _.Db`).
    *   `readRuntime`: Project a field from `RuntimeConfig` (e.g., `Flow.readRuntime _.Logger`).
    *   `log`: Log a message using the runtime logger.
    *   `logWith`: Log a message computed from the `RuntimeConfig`.
*   **Composition:**
    *   `map`, `bind`, `tap`: Standard monadic orchestration.
    *   `mapError`, `tapError`: Error channel observation/mapping.
    *   `orElse`: Flow-based fallback.
    *   `catch`: Translate exceptions into typed failures.
*   **Multi-Flow:**
    *   `zip`, `map2` ... `map5`: Combine independent flows.
    *   `traverse`, `sequence`: Orchestrate collections.
*   **Slicing:**
    *   `localEnv`: Reshapes/narrows the application `'env`.
    *   `localRuntime`: Reshapes/overrides the `RuntimeConfig`.

### B. Effectful Helpers (⚡ STRP-Locked)
These use STRPs to ensure they only appear for `AsyncFlow` and `TaskFlow`.

*   `sleep`: Suspend execution (TimeSpan -> 'Flow).
*   `timeout`, `timeoutToOk`, `timeoutWith`: Boundary races.
*   `retry`: Retrying logic with optional delays.
*   `useWithAcquireRelease`: Effectful resource management.

### C. Task-Exclusive Helpers (⚡⚡ TaskFlow Only)
*   `cancellationToken`: Reads the `System.Threading.CancellationToken`.
*   `ensureNotCanceled`: Short-circuits with a failure if the token is triggered.

---

## 3. Execution (The `run` Boundary)

Execution functions are curried and overloaded to handle the divergence of `TaskFlow`.

*   **`Flow.run`**: `'env -> 'Flow -> 'Result`
    *   The basic entry point. Uses a default `RuntimeConfig`.
*   **`Flow.runFull`**: `RuntimeConfig -> 'env -> 'Flow -> 'Result`
    *   The complete entry point for system-aware execution.
*   **`Flow.runWithToken`**: `'env -> CancellationToken -> TaskFlow -> Task<Result>`
    *   Ergonomic entry point for .NET task-oriented code.

---

## 4. Implementation Pattern (STRP)

To achieve curried "Invisible Async," we use a `static member` approach on the types and a wrapper module.

```fsharp
// Internal representation (example)
type Flow<'env, 'e, 'v> = 
    static member Map (f, x: Flow<_,_,_>) = ...
    static member Bind (f, x: Flow<_,_,_>) = ...

// Unified Module
module Flow =
    let inline map f x = 
        (^T : (static member Map : ('v -> 'u) * ^T -> ^U) (f, x))

    let inline bind f x = 
        (^T : (static member Bind : ('v -> ^U) * ^T -> ^U) (f, x))
        
    let inline log message =
        (^T : (static member Log : string -> ^T) message)
```

---

## 5. Benefits & Implications

### Portability
You can write logic that uses `Flow.map` and `Flow.log`, and it will work whether the underlying type is sync, `Async`, or `Task`. You can move a function from one boundary to another by only changing the signature and the builder—all orchestrations remain identical.

### No "Lifting" Friction
The need for `AsyncFlow.fromFlow` or `TaskFlow.fromAsyncFlow` is eliminated for common operators. The compiler handles the "lift" at the function call site.

### High Performance
Because STRPs are resolved at compile time, there is zero runtime overhead for the abstraction. The resulting code is as fast as calling the specialized module functions directly.

### Sync remains "Safe"
Because `Flow.sleep` is STRP-locked, you cannot accidentally call it from a synchronous flow. The compiler ensures that synchronous code stays synchronous while still allowing it to participate in the runtime system (Logging, TraceID, etc.).

### Conclusion
This model delivers the most ergonomic F# experience possible: **One Module, One Vocabulary, Three Runtimes.**
