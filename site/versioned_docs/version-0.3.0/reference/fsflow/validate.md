---
title: Validate
description: Pure Result-based validation helpers for FsFlow.
---

# Validate

This page shows the pure validation helpers that return `Result<'value, unit>` and only turn into an error value when you bridge them.

Use this page when you want a guard clause, a predicate, or an inverse predicate without carrying effectful error creation through the validation step.

## How to read the family

The names are intentionally symmetrical:

- `okIf*` returns success when the condition holds
- `failIf*` returns success when the condition does not hold
- `Some`, `None`, `Null`, `Empty`, and `Blank` variants match the shape they test
- `orElse` and `orElseWith` convert the unit error into the application error you actually want to surface

This makes the API read naturally in both positive and guard-clause style code.

## Member map

If you want the shortest path through the page, the family splits into:

- boolean guards: `okIf`, `failIf`
- option guards: `okIfSome`, `okIfNone`, `failIfSome`, `failIfNone`
- value-option guards: `okIfValueSome`, `okIfValueNone`, `failIfValueSome`, `failIfValueNone`
- null guards: `okIfNotNull`, `okIfNull`, `failIfNotNull`, `failIfNull`
- collection guards: `okIfNotEmpty`, `okIfEmpty`, `failIfNotEmpty`, `failIfEmpty`
- equality guards: `okIfEqual`, `okIfNotEqual`, `failIfEqual`, `failIfNotEqual`
- string guards: `okIfNonEmptyStr`, `okIfEmptyStr`, `failIfNonEmptyStr`, `failIfEmptyStr`, `okIfNotBlank`, `okIfBlank`, `failIfNotBlank`, `failIfBlank`
- bridges: `orElse`, `orElseWith`

## Common patterns

- validate required text with `okIfNotBlank`
- validate optional values with `okIfSome` or `failIfSome`
- validate collection presence with `okIfNotEmpty`
- validate equality by currying `okIfEqual` or `failIfEqual`
- bridge the result into an application error with `orElse`

## Source-Lifted Notes

The source comments keep the family deliberately small:

- `okIf` and `failIf` are the boolean gatekeepers.
- `okIfSome` / `failIfSome` and the value-option variants are the optional-value helpers.
- `okIfNotNull` / `failIfNull` keep null handling explicit.
- `okIfNotEmpty` / `failIfNotEmpty` handle collections.
- `okIfEqual` / `failIfEqual` and their negated siblings keep comparisons symmetric.
- `okIfNotBlank` / `failIfNotBlank` are the string guards readers usually want first.
- `orElse` and `orElseWith` are the bridge from `Result<'value, unit>` to the real application error.

## Booleans

- `Validate.okIf`
- `Validate.failIf`

## Options and value options

- `Validate.okIfSome`
- `Validate.okIfNone`
- `Validate.failIfSome`
- `Validate.failIfNone`
- `Validate.okIfValueSome`
- `Validate.okIfValueNone`
- `Validate.failIfValueSome`
- `Validate.failIfValueNone`

## Nulls

- `Validate.okIfNotNull`
- `Validate.okIfNull`
- `Validate.failIfNotNull`
- `Validate.failIfNull`

## Collections

- `Validate.okIfNotEmpty`
- `Validate.okIfEmpty`
- `Validate.failIfNotEmpty`
- `Validate.failIfEmpty`

## Equality

- `Validate.okIfEqual`
- `Validate.okIfNotEqual`
- `Validate.failIfEqual`
- `Validate.failIfNotEqual`

## Strings

- `Validate.okIfNonEmptyStr`
- `Validate.okIfEmptyStr`
- `Validate.failIfNonEmptyStr`
- `Validate.failIfEmptyStr`
- `Validate.okIfNotBlank`
- `Validate.okIfBlank`
- `Validate.failIfNotBlank`
- `Validate.failIfBlank`

## Bridges

These helpers convert the unit error into an application error only when needed:

- `Validate.orElse`
- `Validate.orElseWith`

## Example

```fsharp
let validateProfileName name =
    Validate.okIfNotBlank name
    |> Validate.orElse "Profile name is required"
```

The same shape works for guard clauses:

```fsharp
let validateUnlocked user =
    Validate.failIfSome user.LockoutReason
    |> Validate.orElse "Account is locked"
```

## Source

- [Validate.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs)
