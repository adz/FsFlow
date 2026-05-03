# DX Improvement: Tuple-Based Smart Binds

## Motivation
Traditional functional libraries (like FsToolkit.ErrorHandling) often use a proliferation of `require*` or `from*` helpers to lift different types into a computation expression (e.g., `AsyncResult.requireSome`). This creates several problems for both humans and AI agents:
1. **Naming Fatigue**: Users must memorize specific names for every type combination.
2. **Context Switching**: The mental model shifts from "Happy Path" to "Transformation Pipeline" and back.
3. **Agent Token Bloat**: LLMs spend significant context tokens searching for the exact helper name and signature.

The **Tuple-Based Smart Bind** provides a consistent, jargon-free syntax for the most common pattern: **"Unwrap this value, or fail with this domain error."**

## Proposed Syntax
By overloading the `Bind` method in `FlowBuilder`, `AsyncFlowBuilder`, and `TaskFlowBuilder`, we allow the user to provide an error directly alongside the source value in a tuple.

```fsharp
asyncFlow {
    // Option + Error
    let! user = tryGetUser username, orFailTo InvalidUser
    
    // Boolean + Error
    do! isPwdValid password user, orFailTo InvalidPwd
    
    // Check (Result<_, unit>) + Error
    do! Check.notBlank username, orFailTo NameRequired
}
```

### The `orFailTo` Label
While the syntax is just a tuple `(source, error)`, we provide a semantic label to clarify the intent:

```fsharp
[<AutoOpen>]
module FsFlowLabels =
    /// Semantic label for the second element of a smart-bind tuple.
    let inline orFailTo (error: 'E) = error
```

## Implementation Plan
1. **Overload `Bind`** in all three primary builders (`Flow`, `AsyncFlow`, `TaskFlow`) to accept:
   - `('T option * 'E)`
   - `('T voption * 'E)`
   - `(bool * 'E)`
   - `(Result<'T, unit> * 'E)`
2. **Internal Helpers**: Consolidate the logic in `InternalCombinatorCore` or specialized modules like `OptionFlow` and `ResultFlow`.
3. **Consistency**: Ensure that `TaskFlow` supports both `Task<'T option>` and `ValueTask<'T option>` in the tuple shape.

## Benefits
- **Zero Jargon**: No need to learn `requireSome`, `demandTrue`, etc.
- **Visual Alignment**: The domain error is always the second part of the `let!` or `do!` line.
- **Agent Precision**: Agents can use the tuple bind reliably without checking if a specific `fromTypeWithThisError` helper exists.
