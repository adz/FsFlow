namespace FsFlow

open System

/// <summary>
/// Pure validation helpers that return <see cref="T:System.Result`2" /> values with a unit error.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Validate =
    let okIf (cond: bool) : Result<unit, unit> =
        if cond then Ok () else Error ()

    let failIf (cond: bool) : Result<unit, unit> =
        if not cond then Ok () else Error ()

    let okIfSome (opt: 'a option) : Result<'a, unit> =
        match opt with
        | Some value -> Ok value
        | None -> Error ()

    let okIfNone (opt: 'a option) : Result<unit, unit> =
        match opt with
        | None -> Ok ()
        | Some _ -> Error ()

    let failIfSome (opt: 'a option) : Result<unit, unit> =
        match opt with
        | Some _ -> Error ()
        | None -> Ok ()

    let failIfNone (opt: 'a option) : Result<'a, unit> =
        match opt with
        | None -> Error ()
        | Some value -> Ok value

    let okIfValueSome (opt: 'a voption) : Result<'a, unit> =
        match opt with
        | ValueSome value -> Ok value
        | ValueNone -> Error ()

    let okIfValueNone (opt: 'a voption) : Result<unit, unit> =
        match opt with
        | ValueNone -> Ok ()
        | ValueSome _ -> Error ()

    let failIfValueSome (opt: 'a voption) : Result<unit, unit> =
        match opt with
        | ValueSome _ -> Error ()
        | ValueNone -> Ok ()

    let failIfValueNone (opt: 'a voption) : Result<'a, unit> =
        match opt with
        | ValueNone -> Error ()
        | ValueSome value -> Ok value

    let okIfNotNull (value: 'a when 'a : null) : Result<'a, unit> =
        if isNull value then Error () else Ok value

    let okIfNull (value: 'a when 'a : null) : Result<unit, unit> =
        if isNull value then Ok () else Error ()

    let failIfNotNull (value: 'a when 'a : null) : Result<unit, unit> =
        if isNull value then Error () else Ok ()

    let failIfNull (value: 'a when 'a : null) : Result<'a, unit> =
        if isNull value then Ok value else Error ()

    let okIfNotEmpty (coll: seq<'a>) : Result<seq<'a>, unit> =
        if Seq.isEmpty coll then Error () else Ok coll

    let okIfEmpty (coll: seq<'a>) : Result<unit, unit> =
        if Seq.isEmpty coll then Ok () else Error ()

    let failIfNotEmpty (coll: seq<'a>) : Result<unit, unit> =
        if Seq.isEmpty coll then Ok () else Error ()

    let failIfEmpty (coll: seq<'a>) : Result<seq<'a>, unit> =
        if Seq.isEmpty coll then Error () else Ok coll

    let okIfEqual (expected: 'a) (actual: 'a) : Result<unit, unit> =
        if expected = actual then Ok () else Error ()

    let okIfNotEqual (expected: 'a) (actual: 'a) : Result<unit, unit> =
        if expected <> actual then Ok () else Error ()

    let failIfEqual (expected: 'a) (actual: 'a) : Result<unit, unit> =
        if expected = actual then Error () else Ok ()

    let failIfNotEqual (expected: 'a) (actual: 'a) : Result<unit, unit> =
        if expected <> actual then Error () else Ok ()

    let okIfNonEmptyStr (str: string) : Result<string, unit> =
        if String.IsNullOrEmpty str then Error () else Ok str

    let okIfEmptyStr (str: string) : Result<unit, unit> =
        if String.IsNullOrEmpty str then Ok () else Error ()

    let failIfNonEmptyStr (str: string) : Result<unit, unit> =
        if String.IsNullOrEmpty str then Ok () else Error ()

    let failIfEmptyStr (str: string) : Result<string, unit> =
        if String.IsNullOrEmpty str then Error () else Ok str

    let okIfNotBlank (str: string) : Result<string, unit> =
        if String.IsNullOrWhiteSpace str then Error () else Ok str

    let okIfBlank (str: string) : Result<unit, unit> =
        if String.IsNullOrWhiteSpace str then Ok () else Error ()

    let failIfNotBlank (str: string) : Result<unit, unit> =
        if String.IsNullOrWhiteSpace str then Ok () else Error ()

    let failIfBlank (str: string) : Result<string, unit> =
        if String.IsNullOrWhiteSpace str then Error () else Ok str

    let orElse (error: 'e) (result: Result<'value, unit>) : Result<'value, 'e> =
        Result.mapError (fun () -> error) result

    let orElseWith (errorFn: unit -> 'e) (result: Result<'value, unit>) : Result<'value, 'e> =
        Result.mapError (fun () -> errorFn ()) result
