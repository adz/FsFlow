---
title: Check
description: Source-documented pure predicate helpers for FsFlow.
---

# Check

This page shows the source-documented `Check` surface: the unit-failure result type and the reusable predicate helpers.

## Core type

- type `Check`: A reusable predicate result that carries a unit failure placeholder until the caller maps it into a domain-specific error. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L249)

## Module functions

- module `Check`: Pure predicate helpers that return `Result` values with a unit error, plus the bridge functions that turn those checks into application errors. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L257)
- `Check.fromPredicate`: Builds a check from a predicate while preserving the successful value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L259)
- `Check.not`: Returns success when the supplied check fails. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L266)
- `Check.and`: Returns success when both checks succeed. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L272)
- `Check.or`: Returns success when either check succeeds. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L281)
- `Check.all`: Returns success when every check in the sequence succeeds. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L290)
- `Check.any`: Returns success when at least one check in the sequence succeeds. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L306)
- `Check.okIf`: Returns success when the condition is true. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L322)
- `Check.failIf`: Returns success when the condition is false. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L326)
- `Check.okIfSome`: Returns the value when the option is `Some`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L330)
- `Check.okIfNone`: Returns success when the option is `None`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L336)
- `Check.failIfSome`: Returns success when the option is `None`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L342)
- `Check.failIfNone`: Returns the value when the option is `Some`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L348)
- `Check.okIfValueSome`: Returns the value when the value option is `ValueSome`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L354)
- `Check.okIfValueNone`: Returns success when the value option is `ValueNone`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L360)
- `Check.failIfValueSome`: Returns success when the value option is `ValueNone`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L366)
- `Check.failIfValueNone`: Returns the value when the value option is `ValueSome`. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L372)
- `Check.okIfNotNull`: Returns the value when it is not null. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L378)
- `Check.okIfNull`: Returns success when the value is null. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L382)
- `Check.failIfNotNull`: Returns success when the value is null. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L386)
- `Check.failIfNull`: Returns the value when it is null. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L390)
- `Check.okIfNotEmpty`: Returns the sequence when it is not empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L394)
- `Check.okIfEmpty`: Returns success when the sequence is empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L398)
- `Check.failIfNotEmpty`: Returns success when the sequence is empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L402)
- `Check.failIfEmpty`: Returns the sequence when it is not empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L406)
- `Check.okIfEqual`: Returns success when the values are equal. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L410)
- `Check.okIfNotEqual`: Returns success when the values are not equal. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L414)
- `Check.failIfEqual`: Returns success when the values are equal. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L418)
- `Check.failIfNotEqual`: Returns success when the values are not equal. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L422)
- `Check.okIfNonEmptyStr`: Returns the string when it is not null or empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L426)
- `Check.okIfEmptyStr`: Returns success when the string is null or empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L430)
- `Check.failIfNonEmptyStr`: Returns success when the string is null or empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L434)
- `Check.failIfEmptyStr`: Returns the string when it is null or empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L438)
- `Check.okIfNotBlank`: Returns the string when it is not blank. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L442)
- `Check.notBlank`: Returns the string when it is not blank. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L446)
- `Check.okIfBlank`: Returns success when the string is blank. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L450)
- `Check.blank`: Returns success when the string is blank. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L454)
- `Check.failIfNotBlank`: Returns success when the string is blank. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L458)
- `Check.failIfBlank`: Returns the string when it is blank. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L462)
- `Check.orElse`: Maps a unit error into the supplied application error value. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L467)
- `Check.orElseWith`: Maps a unit error into an application error produced on demand. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L472)
- `Check.notNull`: Returns the value when it is not null. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L476)
- `Check.notEmpty`: Returns the sequence when it is not empty. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L480)
- `Check.equal`: Returns success when the values are equal. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L484)
- `Check.notEqual`: Returns success when the values are not equal. [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs#L488)

## Source

- [Validate.fs](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Validate.fs)
