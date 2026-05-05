---
title: validate
description: API reference for Builders.validate
---

# validate

The accumulating `validate { }` computation expression.


```fsharp
let validate
```


## Remarks

<para>
Use this builder when you want to collect all validation failures instead of stopping
at the first one.
</para>
<para>
It is intended for forms, configuration checks, and other input-heavy boundaries where
the user benefits from seeing every problem at once.
</para>


## Information

- **Module**: `Builders`
- **Source**: [source](https://github.com/adz/FsFlow/blob/main/src/FsFlow/Flow.fs#L1508)

## Examples

```fsharp
let validatedUser =
    validate {
        let! name = Check.notBlank input.Name
        let! age = Check.okIf (input.Age > 0) "Age must be positive"
        return { Name = name; Age = age }
    }
```

