---
title: Check
description: Source-documented pure predicate helpers for FsFlow.
---

# Check

This page shows the source-documented `Check` surface: the unit-failure result type and the reusable predicate helpers.

## Core type

- type `Check`: A reusable predicate result that carries a unit failure placeholder until the caller
maps it into a domain-specific error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L363)

## Module functions

- module `Check`: Pure predicate helpers that return `Result` values with a unit error,
plus the bridge functions that turn those checks into application errors. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L371)
- [`Check.fromPredicate`](./check-frompredicate.md): Builds a check from a predicate while preserving the successful value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L376)
- [`Check.not`](./check-not.md): Returns success when the supplied check fails. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L389)
- [`Check.and`](./check-and.md): Returns success when both checks succeed. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L402)
- [`Check.or`](./check-or.md): Returns success when either check succeeds. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L418)
- [`Check.all`](./check-all.md): Returns success when every check in the sequence succeeds. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L433)
- [`Check.any`](./check-any.md): Returns success when at least one check in the sequence succeeds. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L455)
- [`Check.okIf`](./check-okif.md): Returns success when the condition is true. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L473)
- [`Check.failIf`](./check-failif.md): Returns success when the condition is false. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L479)
- [`Check.okIfSome`](./check-okifsome.md): Returns the value when the option is `Some`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L485)
- [`Check.okIfNone`](./check-okifnone.md): Returns success when the option is `None`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L493)
- [`Check.failIfSome`](./check-failifsome.md): Returns success when the option is `None`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L501)
- [`Check.failIfNone`](./check-failifnone.md): Returns the value when the option is `Some`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L509)
- [`Check.okIfValueSome`](./check-okifvaluesome.md): Returns the value when the value option is `ValueSome`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L517)
- [`Check.okIfValueNone`](./check-okifvaluenone.md): Returns success when the value option is `ValueNone`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L525)
- [`Check.failIfValueSome`](./check-failifvaluesome.md): Returns success when the value option is `ValueNone`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L533)
- [`Check.failIfValueNone`](./check-failifvaluenone.md): Returns the value when the value option is `ValueSome`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L541)
- [`Check.okIfNotNull`](./check-okifnotnull.md): Returns the value when it is not null. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L549)
- [`Check.okIfNull`](./check-okifnull.md): Returns success when the value is null. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L555)
- [`Check.failIfNotNull`](./check-failifnotnull.md): Returns success when the value is null. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L561)
- [`Check.failIfNull`](./check-failifnull.md): Returns the value when it is null. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L567)
- [`Check.okIfNotEmpty`](./check-okifnotempty.md): Returns the sequence when it is not empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L573)
- [`Check.okIfEmpty`](./check-okifempty.md): Returns success when the sequence is empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L579)
- [`Check.failIfNotEmpty`](./check-failifnotempty.md): Returns success when the sequence is empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L585)
- [`Check.failIfEmpty`](./check-failifempty.md): Returns the sequence when it is not empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L591)
- [`Check.okIfEqual`](./check-okifequal.md): Returns success when the values are equal. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L598)
- [`Check.okIfNotEqual`](./check-okifnotequal.md): Returns success when the values are not equal. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L605)
- [`Check.failIfEqual`](./check-failifequal.md): Returns success when the values are equal. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L612)
- [`Check.failIfNotEqual`](./check-failifnotequal.md): Returns success when the values are not equal. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L619)
- [`Check.okIfNonEmptyStr`](./check-okifnonemptystr.md): Returns the string when it is not null or empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L625)
- [`Check.okIfEmptyStr`](./check-okifemptystr.md): Returns success when the string is null or empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L631)
- [`Check.failIfNonEmptyStr`](./check-failifnonemptystr.md): Returns success when the string is null or empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L637)
- [`Check.failIfEmptyStr`](./check-failifemptystr.md): Returns the string when it is null or empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L643)
- [`Check.okIfNotBlank`](./check-okifnotblank.md): Returns the string when it is not blank. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L649)
- [`Check.notBlank`](./check-notblank.md): Returns the string when it is not blank. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L655)
- [`Check.okIfBlank`](./check-okifblank.md): Returns success when the string is blank. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L661)
- [`Check.blank`](./check-blank.md): Returns success when the string is blank. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L667)
- [`Check.failIfNotBlank`](./check-failifnotblank.md): Returns success when the string is blank. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L673)
- [`Check.failIfBlank`](./check-failifblank.md): Returns the string when it is blank. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L679)
- [`Check.orElse`](./check-orelse.md): Maps a unit error into the supplied application error value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L716)
- [`Check.orElseWith`](./check-orelsewith.md): Maps a unit error into an application error produced on demand. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L726)
- [`Check.notNull`](./check-notnull.md): Returns the value when it is not null. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L685)
- [`Check.notEmpty`](./check-notempty.md): Returns the sequence when it is not empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L691)
- [`Check.equal`](./check-equal.md): Returns success when the values are equal. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L698)
- [`Check.notEqual`](./check-notequal.md): Returns success when the values are not equal. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L705)

