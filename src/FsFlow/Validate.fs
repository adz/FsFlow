namespace FsFlow

open System

/// <summary>
/// Pure validation helpers that return <see cref="T:System.Result`2" /> values with a unit error,
/// plus the bridge functions that turn those checks into application errors.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Validate =
    /// <summary>Returns success when the condition is true.</summary>
    let okIf (cond: bool) : Result<unit, unit> =
        if cond then Ok () else Error ()

    /// <summary>Returns success when the condition is false.</summary>
    let failIf (cond: bool) : Result<unit, unit> =
        if not cond then Ok () else Error ()

    /// <summary>Returns the value when the option is <c>Some</c>.</summary>
    let okIfSome (opt: 'a option) : Result<'a, unit> =
        match opt with
        | Some value -> Ok value
        | None -> Error ()

    /// <summary>Returns success when the option is <c>None</c>.</summary>
    let okIfNone (opt: 'a option) : Result<unit, unit> =
        match opt with
        | None -> Ok ()
        | Some _ -> Error ()

    /// <summary>Returns success when the option is <c>None</c>.</summary>
    let failIfSome (opt: 'a option) : Result<unit, unit> =
        match opt with
        | Some _ -> Error ()
        | None -> Ok ()

    /// <summary>Returns the value when the option is <c>Some</c>.</summary>
    let failIfNone (opt: 'a option) : Result<'a, unit> =
        match opt with
        | None -> Error ()
        | Some value -> Ok value

    /// <summary>Returns the value when the value option is <c>ValueSome</c>.</summary>
    let okIfValueSome (opt: 'a voption) : Result<'a, unit> =
        match opt with
        | ValueSome value -> Ok value
        | ValueNone -> Error ()

    /// <summary>Returns success when the value option is <c>ValueNone</c>.</summary>
    let okIfValueNone (opt: 'a voption) : Result<unit, unit> =
        match opt with
        | ValueNone -> Ok ()
        | ValueSome _ -> Error ()

    /// <summary>Returns success when the value option is <c>ValueNone</c>.</summary>
    let failIfValueSome (opt: 'a voption) : Result<unit, unit> =
        match opt with
        | ValueSome _ -> Error ()
        | ValueNone -> Ok ()

    /// <summary>Returns the value when the value option is <c>ValueSome</c>.</summary>
    let failIfValueNone (opt: 'a voption) : Result<'a, unit> =
        match opt with
        | ValueNone -> Error ()
        | ValueSome value -> Ok value

    /// <summary>Returns the value when it is not null.</summary>
    let okIfNotNull (value: 'a when 'a : null) : Result<'a, unit> =
        if isNull value then Error () else Ok value

    /// <summary>Returns success when the value is null.</summary>
    let okIfNull (value: 'a when 'a : null) : Result<unit, unit> =
        if isNull value then Ok () else Error ()

    /// <summary>Returns success when the value is null.</summary>
    let failIfNotNull (value: 'a when 'a : null) : Result<unit, unit> =
        if isNull value then Error () else Ok ()

    /// <summary>Returns the value when it is null.</summary>
    let failIfNull (value: 'a when 'a : null) : Result<'a, unit> =
        if isNull value then Ok value else Error ()

    /// <summary>Returns the sequence when it is not empty.</summary>
    let okIfNotEmpty (coll: seq<'a>) : Result<seq<'a>, unit> =
        if Seq.isEmpty coll then Error () else Ok coll

    /// <summary>Returns success when the sequence is empty.</summary>
    let okIfEmpty (coll: seq<'a>) : Result<unit, unit> =
        if Seq.isEmpty coll then Ok () else Error ()

    /// <summary>Returns success when the sequence is empty.</summary>
    let failIfNotEmpty (coll: seq<'a>) : Result<unit, unit> =
        if Seq.isEmpty coll then Ok () else Error ()

    /// <summary>Returns the sequence when it is not empty.</summary>
    let failIfEmpty (coll: seq<'a>) : Result<seq<'a>, unit> =
        if Seq.isEmpty coll then Error () else Ok coll

    /// <summary>Returns success when the values are equal.</summary>
    let okIfEqual (expected: 'a) (actual: 'a) : Result<unit, unit> =
        if expected = actual then Ok () else Error ()

    /// <summary>Returns success when the values are not equal.</summary>
    let okIfNotEqual (expected: 'a) (actual: 'a) : Result<unit, unit> =
        if expected <> actual then Ok () else Error ()

    /// <summary>Returns success when the values are equal.</summary>
    let failIfEqual (expected: 'a) (actual: 'a) : Result<unit, unit> =
        if expected = actual then Error () else Ok ()

    /// <summary>Returns success when the values are not equal.</summary>
    let failIfNotEqual (expected: 'a) (actual: 'a) : Result<unit, unit> =
        if expected <> actual then Error () else Ok ()

    /// <summary>Returns the string when it is not null or empty.</summary>
    let okIfNonEmptyStr (str: string) : Result<string, unit> =
        if String.IsNullOrEmpty str then Error () else Ok str

    /// <summary>Returns success when the string is null or empty.</summary>
    let okIfEmptyStr (str: string) : Result<unit, unit> =
        if String.IsNullOrEmpty str then Ok () else Error ()

    /// <summary>Returns success when the string is null or empty.</summary>
    let failIfNonEmptyStr (str: string) : Result<unit, unit> =
        if String.IsNullOrEmpty str then Ok () else Error ()

    /// <summary>Returns the string when it is null or empty.</summary>
    let failIfEmptyStr (str: string) : Result<string, unit> =
        if String.IsNullOrEmpty str then Error () else Ok str

    /// <summary>Returns the string when it is not blank.</summary>
    let okIfNotBlank (str: string) : Result<string, unit> =
        if String.IsNullOrWhiteSpace str then Error () else Ok str

    /// <summary>Returns success when the string is blank.</summary>
    let okIfBlank (str: string) : Result<unit, unit> =
        if String.IsNullOrWhiteSpace str then Ok () else Error ()

    /// <summary>Returns success when the string is blank.</summary>
    let failIfNotBlank (str: string) : Result<unit, unit> =
        if String.IsNullOrWhiteSpace str then Ok () else Error ()

    /// <summary>Returns the string when it is blank.</summary>
    let failIfBlank (str: string) : Result<string, unit> =
        if String.IsNullOrWhiteSpace str then Error () else Ok str

    /// <summary>Maps a unit error into the supplied application error value.</summary>
    let orElse (error: 'e) (result: Result<'value, unit>) : Result<'value, 'e> =
        Result.mapError (fun () -> error) result

    /// <summary>Maps a unit error into an application error produced on demand.</summary>
    let orElseWith (errorFn: unit -> 'e) (result: Result<'value, unit>) : Result<'value, 'e> =
        Result.mapError (fun () -> errorFn ()) result
