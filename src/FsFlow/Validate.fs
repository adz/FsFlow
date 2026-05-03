namespace FsFlow

open System

/// <summary>Location markers used to describe where a diagnostic belongs in a validation graph.</summary>
[<RequireQualifiedAccess>]
type PathSegment =
    /// <summary>A string-based key, usually for map or record fields.</summary>
    | Key of string
    /// <summary>A zero-based integer index, usually for lists or arrays.</summary>
    | Index of int
    /// <summary>A descriptive name for a property or field.</summary>
    | Name of string

/// <summary>A path through a validation graph, represented as a list of <see cref="T:FsFlow.PathSegment" />.</summary>
type Path = PathSegment list

/// <summary>A single failure item attached to a path in a validation graph.</summary>
type Diagnostic<'error> =
    {
        /// <summary>The path to the source of the error.</summary>
        Path: Path
        /// <summary>The application-specific error value of type <c>'error</c>.</summary>
        Error: 'error
    }

/// <summary>
/// A mergeable validation graph that carries local diagnostics and nested child branches.
/// </summary>
/// <remarks>
/// This structure allows for representing errors in hierarchical data models. 
/// Use <see cref="T:FsFlow.Diagnostics.flatten" /> to convert it into a linear list.
/// </remarks>
type Diagnostics<'error> =
    {
        /// <summary>Diagnostics that occurred exactly at this node in the graph.</summary>
        Local: Diagnostic<'error> list
        /// <summary>Nested diagnostic branches, keyed by <see cref="T:FsFlow.PathSegment" />.</summary>
        Children: Map<PathSegment, Diagnostics<'error>>
    }

/// <summary>
/// An accumulating validation result that keeps the structured diagnostics graph visible.
/// </summary>
/// <remarks>
/// Unlike <see cref="T:Microsoft.FSharp.Core.FSharpResult`2" />, this type is designed for applicative
/// composition using <c>and!</c> in the <c>validate { }</c> builder, which merges errors instead of
/// short-circuiting.
/// </remarks>
type Validation<'value, 'error> = private Validation of Result<'value, Diagnostics<'error>>

/// <summary>
/// Helpers for building, merging, and flattening validation diagnostics graphs.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Diagnostics =
    /// <summary>Creates an empty diagnostics graph with no errors.</summary>
    /// <returns>An empty <see cref="T:FsFlow.Diagnostics`1" />.</returns>
    let empty<'error> : Diagnostics<'error> =
        {
            Local = []
            Children = Map.empty
        }

    /// <summary>Creates a diagnostics graph containing exactly one diagnostic item at the root.</summary>
    /// <param name="diagnostic">The <see cref="T:FsFlow.Diagnostic`1" /> to wrap.</param>
    /// <returns>A <see cref="T:FsFlow.Diagnostics`1" /> with a single error.</returns>
    let singleton (diagnostic: Diagnostic<'error>) : Diagnostics<'error> =
        {
            Local = [ diagnostic ]
            Children = Map.empty
        }

    /// <summary>Recursively merges two diagnostics graphs, combining shared branches and local errors.</summary>
    /// <remarks>
    /// This is the core operation for applicative validation. It ensures that errors from sibling
    /// fields are collected together into a single structured graph.
    /// </remarks>
    /// <param name="left">The first graph of type <see cref="T:FsFlow.Diagnostics`1" />.</param>
    /// <param name="right">The second graph of type <see cref="T:FsFlow.Diagnostics`1" />.</param>
    /// <returns>A new <see cref="T:FsFlow.Diagnostics`1" /> containing the union of both inputs.</returns>
    let rec merge (left: Diagnostics<'error>) (right: Diagnostics<'error>) : Diagnostics<'error> =
        let addBranch children key branch =
            match Map.tryFind key children with
            | Some existing -> Map.add key (merge existing branch) children
            | None -> Map.add key branch children

        {
            Local = left.Local @ right.Local
            Children = Map.fold addBranch left.Children right.Children
        }

    /// <summary>Flattens the structured diagnostics graph into a linear list of diagnostics.</summary>
    /// <remarks>
    /// During flattening, child paths are correctly prefixed with their parent segments,
    /// ensuring each <see cref="T:FsFlow.Diagnostic`1" /> in the resulting list has a full absolute path.
    /// </remarks>
    /// <param name="graph">The <see cref="T:FsFlow.Diagnostics`1" /> to flatten.</param>
    /// <returns>A list of type <see cref="T:FsFlow.Diagnostic`1" /> list.</returns>
    let flatten (graph: Diagnostics<'error>) : Diagnostic<'error> list =
        let rec flattenWithPrefix (prefix: Path) (node: Diagnostics<'error>) =
            let local =
                node.Local
                |> List.map (fun diagnostic -> { diagnostic with Path = prefix @ diagnostic.Path })

            let children =
                node.Children
                |> Map.toList
                |> List.collect (fun (segment, child) -> flattenWithPrefix (prefix @ [ segment ]) child)

            local @ children

        flattenWithPrefix [] graph

/// <summary>
/// Helpers for fail-fast <see cref="T:System.Result`2" /> workflows and the bridge from
/// placeholder unit failures into application errors.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Result =
    /// <summary>Maps the successful value of a result.</summary>
    /// <param name="mapper">A function of type <c>'value -> 'next</c>.</param>
    /// <param name="result">The source <see cref="T:System.Result`2" />.</param>
    /// <returns>A new <see cref="T:System.Result`2" /> with the mapped value.</returns>
    let map
        (mapper: 'value -> 'next)
        (result: Result<'value, 'error>)
        : Result<'next, 'error> =
        match result with
        | Ok value -> Ok(mapper value)
        | Error error -> Error error

    /// <summary>Sequences a result-producing continuation after a successful value.</summary>
    /// <param name="binder">A function of type <c>'value -> Result&lt;'next, 'error&gt;</c>.</param>
    /// <param name="result">The source <see cref="T:System.Result`2" />.</param>
    /// <returns>The result of the binder or the original error.</returns>
    let bind
        (binder: 'value -> Result<'next, 'error>)
        (result: Result<'value, 'error>)
        : Result<'next, 'error> =
        match result with
        | Ok value -> binder value
        | Error error -> Error error

    /// <summary>Maps the failure value of a result.</summary>
    /// <param name="mapper">A function of type <c>'error -> 'nextError</c>.</param>
    /// <param name="result">The source <see cref="T:System.Result`2" />.</param>
    /// <returns>A new <see cref="T:System.Result`2" /> with the mapped error.</returns>
    let mapError
        (mapper: 'error -> 'nextError)
        (result: Result<'value, 'error>)
        : Result<'value, 'nextError> =
        match result with
        | Ok value -> Ok value
        | Error error -> Error(mapper error)

    /// <summary>Replaces the unit failure from a predicate result with the supplied error.</summary>
    /// <param name="error">The error of type <c>'nextError</c> to return on failure.</param>
    /// <param name="result">The source result of type <see cref="T:Microsoft.FSharp.Core.FSharpResult`2" />.</param>
    /// <returns>A result with the new error type.</returns>
    let mapErrorTo
        (error: 'nextError)
        (result: Result<'value, unit>)
        : Result<'value, 'nextError> =
        mapError (fun () -> error) result

    /// <summary>Runs a sequence of results until the first failure or the end of the sequence.</summary>
    /// <param name="results">A sequence of type <c>seq&lt;Result&lt;'value, 'error&gt;&gt;</c>.</param>
    /// <returns>A result containing the list of values or the first error encountered.</returns>
    let sequence (results: seq<Result<'value, 'error>>) : Result<'value list, 'error> =
        let mutable values = []
        let mutable failure = None

        use enumerator = results.GetEnumerator()

        while failure.IsNone && enumerator.MoveNext() do
            match enumerator.Current with
            | Ok value -> values <- value :: values
            | Error error -> failure <- Some error

        match failure with
        | Some error -> Error error
        | None -> Ok(List.rev values)

    /// <summary>Maps a sequence with a fail-fast result-producing function.</summary>
    /// <param name="mapper">A function of type <c>'source -> Result&lt;'value, 'error&gt;</c>.</param>
    /// <param name="values">The source sequence.</param>
    /// <returns>A result containing the list of successful mapped values.</returns>
    let traverse
        (mapper: 'source -> Result<'value, 'error>)
        (values: seq<'source>)
        : Result<'value list, 'error> =
        let mutable collected = []
        let mutable failure = None

        use enumerator = values.GetEnumerator()

        while failure.IsNone && enumerator.MoveNext() do
            match mapper enumerator.Current with
            | Ok value -> collected <- value :: collected
            | Error error -> failure <- Some error

        match failure with
        | Some error -> Error error
        | None -> Ok(List.rev collected)

/// <summary>
/// Helpers for accumulating validation results with mergeable diagnostics.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Validation =
    let private unwrap (Validation result) = result

    /// <summary>Converts a <see cref="T:FsFlow.Validation`2" /> into a standard <see cref="T:System.Result`2" />.</summary>
    /// <param name="validation">The validation to convert.</param>
    /// <returns>A result containing either the success value or the full diagnostics graph.</returns>
    let toResult (validation: Validation<'value, 'error>) : Result<'value, Diagnostics<'error>> =
        unwrap validation

    /// <summary>Creates a successful validation result.</summary>
    /// <param name="value">The success value of type <c>'value</c>.</param>
    /// <returns>A successful <see cref="T:FsFlow.Validation`2" />.</returns>
    let succeed (value: 'value) : Validation<'value, 'error> =
        Validation (Ok value)

    /// <summary>Creates a failing validation result with the provided diagnostics.</summary>
    /// <param name="diagnostics">The <see cref="T:FsFlow.Diagnostics`1" /> graph.</param>
    /// <returns>A failing <see cref="T:FsFlow.Validation`2" />.</returns>
    let fail (diagnostics: Diagnostics<'error>) : Validation<'value, 'error> =
        Validation (Error diagnostics)

    /// <summary>Lifts a standard <see cref="T:System.Result`2" /> into the <see cref="T:FsFlow.Validation`2" /> context.</summary>
    /// <remarks>
    /// If the result is an error, it is wrapped in a root-level <see cref="T:FsFlow.Diagnostics`1" /> graph.
    /// </remarks>
    /// <param name="result">The result to lift.</param>
    /// <returns>A <see cref="T:FsFlow.Validation`2" /> mirroring the result.</returns>
    let fromResult (result: Result<'value, 'error>) : Validation<'value, 'error> =
        match result with
        | Ok value -> succeed value
        | Error error -> fail (Diagnostics.singleton { Path = []; Error = error })

    /// <summary>Maps the successful value of a validation.</summary>
    /// <param name="mapper">A function of type <c>'value -> 'next</c>.</param>
    /// <param name="validation">The source <see cref="T:FsFlow.Validation`2" />.</param>
    /// <returns>A validation with the transformed success value.</returns>
    let map
        (mapper: 'value -> 'next)
        (validation: Validation<'value, 'error>)
        : Validation<'next, 'error> =
        validation |> unwrap |> Result.map mapper |> Validation

    /// <summary>Sequences a validation-producing continuation.</summary>
    /// <remarks>
    /// This is the monadic "bind" for validation. Note that this operation short-circuits
    /// and does not accumulate errors from the binder if the source has already failed.
    /// For accumulation, use <see cref="map2" /> or the applicative <c>and!</c> syntax.
    /// </remarks>
    /// <param name="binder">A function of type <c>'value -> Validation&lt;'next, 'error&gt;</c>.</param>
    /// <param name="validation">The source validation.</param>
    /// <returns>The result of the binder or the original diagnostics.</returns>
    let bind
        (binder: 'value -> Validation<'next, 'error>)
        (validation: Validation<'value, 'error>)
        : Validation<'next, 'error> =
        match unwrap validation with
        | Ok value -> binder value
        | Error diagnostics -> fail diagnostics

    /// <summary>Maps the error type of a validation graph.</summary>
    /// <param name="mapper">A function of type <c>'error -> 'nextError</c>.</param>
    /// <param name="validation">The source <see cref="T:FsFlow.Validation`2" />.</param>
    /// <returns>A validation with transformed error values.</returns>
    let mapError
        (mapper: 'error -> 'nextError)
        (validation: Validation<'value, 'error>)
        : Validation<'value, 'nextError> =
        let rec mapDiagnostics (graph: Diagnostics<'error>) : Diagnostics<'nextError> =
            {
                Local =
                    graph.Local
                    |> List.map (fun diagnostic ->
                        {
                            Path = diagnostic.Path
                            Error = mapper diagnostic.Error
                        })
                Children =
                    graph.Children
                    |> Map.map (fun _ child -> mapDiagnostics child)
            }

        validation |> unwrap |> Result.mapError mapDiagnostics |> Validation

    /// <summary>Combines two validations, accumulating errors if both fail.</summary>
    /// <remarks>
    /// This is the core applicative operation. If both <paramref name="left" /> and 
    /// <paramref name="right" /> fail, their diagnostics graphs are merged.
    /// </remarks>
    /// <param name="mapper">A function of type <c>'left -> 'right -> 'value</c>.</param>
    /// <param name="left">The first validation.</param>
    /// <param name="right">The second validation.</param>
    /// <returns>A validation with the combined result.</returns>
    let map2
        (mapper: 'left -> 'right -> 'value)
        (left: Validation<'left, 'error>)
        (right: Validation<'right, 'error>)
        : Validation<'value, 'error> =
        Validation(
            match unwrap left, unwrap right with
            | Ok leftValue, Ok rightValue -> Ok(mapper leftValue rightValue)
            | Error leftDiagnostics, Ok _ -> Error leftDiagnostics
            | Ok _, Error rightDiagnostics -> Error rightDiagnostics
            | Error leftDiagnostics, Error rightDiagnostics -> Error(Diagnostics.merge leftDiagnostics rightDiagnostics)
        )

    /// <summary>Applies a validation-wrapped function to a validation-wrapped value.</summary>
    /// <param name="validation">The validation containing the function.</param>
    /// <param name="value">The validation containing the value.</param>
    /// <returns>The result of applying the function to the value, with accumulated errors.</returns>
    let apply
        (validation: Validation<'value -> 'next, 'error>)
        (value: Validation<'value, 'error>)
        : Validation<'next, 'error> =
        map2 (fun mapper input -> mapper input) validation value

    /// <summary>Collects a sequence of validations into a single validation of a list.</summary>
    /// <remarks>
    /// This operation is applicative: it will collect errors from ALL items in the sequence.
    /// </remarks>
    /// <param name="validations">A sequence of type <c>seq&lt;Validation&lt;'value, 'error&gt;&gt;</c>.</param>
    /// <returns>A validation containing the list of values or accumulated diagnostics.</returns>
    let collect (validations: seq<Validation<'value, 'error>>) : Validation<'value list, 'error> =
        let folder
            (state: Validation<'value list, 'error>)
            (validation: Validation<'value, 'error>) =
            map2 (fun values value -> values @ [ value ]) state validation

        Seq.fold folder (succeed []) validations

    /// <summary>Transforms a sequence of validations into a validation of a list.</summary>
    /// <param name="validations">The input sequence.</param>
    /// <returns>A validation containing the list of values.</returns>
    let sequence (validations: seq<Validation<'value, 'error>>) : Validation<'value list, 'error> =
        collect validations

    /// <summary>Merges two validations into a validation of a tuple.</summary>
    /// <param name="left">The first validation.</param>
    /// <param name="right">The second validation.</param>
    /// <returns>A validation containing a tuple of the results.</returns>
    let merge (left: Validation<'value, 'error>) (right: Validation<'next, 'error>) : Validation<'value * 'next, 'error> =
        map2 (fun leftValue rightValue -> leftValue, rightValue) left right

/// <summary>
/// A reusable predicate result that carries a unit failure placeholder until the caller
/// maps it into a domain-specific error.
/// </summary>
/// <remarks>
/// Use the <see cref="T:FsFlow.Check" /> module helpers to create and compose checks.
/// </remarks>
type Check<'value> = Result<'value, unit>

/// <summary>
/// Pure predicate helpers that return <see cref="T:System.Result`2" /> values with a unit error,
/// plus the bridge functions that turn those checks into application errors.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Check =
    /// <summary>Builds a check from a predicate while preserving the successful value.</summary>
    /// <param name="predicate">A function of type <c>'value -> bool</c> to test the value.</param>
    /// <param name="value">The value of type <c>'value</c> to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the value if the predicate succeeds.</returns>
    let fromPredicate (predicate: 'value -> bool) (value: 'value) : Check<'value> =
        if predicate value then
            Ok value
        else
            Error ()

    /// <summary>Returns success when the supplied check fails.</summary>
    /// <remarks>
    /// This is a logical "not" operation for checks. Note that it discards the success value
    /// and returns <see cref="T:Microsoft.FSharp.Core.Unit" /> on success.
    /// </remarks>
    /// <param name="check">The source <see cref="T:FsFlow.Check`1" /> to invert.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the input fails.</returns>
    let not (check: Check<'value>) : Check<unit> =
        match check with
        | Ok _ -> Error ()
        | Error () -> Ok ()

    /// <summary>Returns success when both checks succeed.</summary>
    /// <remarks>
    /// This is a logical "and" operation. It short-circuits: if <paramref name="left" /> fails,
    /// <paramref name="right" /> is not evaluated.
    /// </remarks>
    /// <param name="left">The first check.</param>
    /// <param name="right">The second check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds only if both inputs succeed.</returns>
    let ``and`` (left: Check<'left>) (right: Check<'right>) : Check<unit> =
        match left with
        | Error () -> Error ()
        | Ok _ ->
            match right with
            | Ok _ -> Ok ()
            | Error () -> Error ()

    /// <summary>Returns success when either check succeeds.</summary>
    /// <remarks>
    /// This is a logical "or" operation. It short-circuits: if <paramref name="left" /> succeeds,
    /// <paramref name="right" /> is not evaluated.
    /// </remarks>
    /// <param name="left">The first check.</param>
    /// <param name="right">The second check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if either input succeeds.</returns>
    let ``or`` (left: Check<'left>) (right: Check<'right>) : Check<unit> =
        match left with
        | Ok _ -> Ok ()
        | Error () ->
            match right with
            | Ok _ -> Ok ()
            | Error () -> Error ()

    /// <summary>Returns success when every check in the sequence succeeds.</summary>
    /// <remarks>
    /// Sequentially evaluates each check in the <paramref name="checks" /> sequence.
    /// Stops at the first failure.
    /// </remarks>
    /// <param name="checks">A sequence of checks.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds only if all inputs succeed.</returns>
    let all (checks: seq<Check<'value>>) : Check<unit> =
        use enumerator = checks.GetEnumerator()

        let mutable result = Ok ()
        let mutable continueLoop = true

        while continueLoop && enumerator.MoveNext() do
            match enumerator.Current with
            | Ok _ -> ()
            | Error () ->
                result <- Error ()
                continueLoop <- false

        result

    /// <summary>Returns success when at least one check in the sequence succeeds.</summary>
    /// <remarks>
    /// Sequentially evaluates each check in the <paramref name="checks" /> sequence.
    /// Stops at the first success.
    /// </remarks>
    /// <param name="checks">A sequence of checks.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if any input succeeds.</returns>
    let any (checks: seq<Check<'value>>) : Check<unit> =
        use enumerator = checks.GetEnumerator()

        let mutable result = Error ()
        let mutable continueLoop = true

        while continueLoop && enumerator.MoveNext() do
            match enumerator.Current with
            | Ok _ ->
                result <- Ok ()
                continueLoop <- false
            | Error () -> ()

        result

    /// <summary>Returns success when the condition is true.</summary>
    /// <param name="cond">The boolean condition to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if <paramref name="cond" /> is true.</returns>
    let okIf (cond: bool) : Check<unit> =
        if cond then Ok () else Error ()

    /// <summary>Returns success when the condition is false.</summary>
    /// <param name="cond">The boolean condition to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if <paramref name="cond" /> is false.</returns>
    let failIf (cond: bool) : Check<unit> =
        if Operators.not cond then Ok () else Error ()

    /// <summary>Returns the value when the option is <c>Some</c>.</summary>
    /// <param name="opt">The <see cref="T:Microsoft.FSharp.Core.FSharpOption`1" /> to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the value if present.</returns>
    let okIfSome (opt: 'a option) : Check<'a> =
        match opt with
        | Some value -> Ok value
        | None -> Error ()

    /// <summary>Returns success when the option is <c>None</c>.</summary>
    /// <param name="opt">The option to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the option is <see cref="T:Microsoft.FSharp.Core.FSharpOption`1.None" />.</returns>
    let okIfNone (opt: 'a option) : Check<unit> =
        match opt with
        | None -> Ok ()
        | Some _ -> Error ()

    /// <summary>Returns success when the option is <c>None</c>.</summary>
    /// <param name="opt">The option to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the option is <see cref="T:Microsoft.FSharp.Core.FSharpOption`1.None" />.</returns>
    let failIfSome (opt: 'a option) : Check<unit> =
        match opt with
        | Some _ -> Error ()
        | None -> Ok ()

    /// <summary>Returns the value when the option is <c>Some</c>.</summary>
    /// <param name="opt">The option to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the value if present.</returns>
    let failIfNone (opt: 'a option) : Check<'a> =
        match opt with
        | None -> Error ()
        | Some value -> Ok value

    /// <summary>Returns the value when the value option is <c>ValueSome</c>.</summary>
    /// <param name="opt">The <see cref="T:Microsoft.FSharp.Core.FSharpValueOption`1" /> to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the value if present.</returns>
    let okIfValueSome (opt: 'a voption) : Check<'a> =
        match opt with
        | ValueSome value -> Ok value
        | ValueNone -> Error ()

    /// <summary>Returns success when the value option is <c>ValueNone</c>.</summary>
    /// <param name="opt">The value option to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the value option is <see cref="T:Microsoft.FSharp.Core.FSharpValueOption`1.ValueNone" />.</returns>
    let okIfValueNone (opt: 'a voption) : Check<unit> =
        match opt with
        | ValueNone -> Ok ()
        | ValueSome _ -> Error ()

    /// <summary>Returns success when the value option is <c>ValueNone</c>.</summary>
    /// <param name="opt">The value option to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the value option is <see cref="T:Microsoft.FSharp.Core.FSharpValueOption`1.ValueNone" />.</returns>
    let failIfValueSome (opt: 'a voption) : Check<unit> =
        match opt with
        | ValueSome _ -> Error ()
        | ValueNone -> Ok ()

    /// <summary>Returns the value when the value option is <c>ValueSome</c>.</summary>
    /// <param name="opt">The value option to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the value if present.</returns>
    let failIfValueNone (opt: 'a voption) : Check<'a> =
        match opt with
        | ValueNone -> Error ()
        | ValueSome value -> Ok value

    /// <summary>Returns the value when it is not null.</summary>
    /// <param name="value">The value of type <c>'a</c> to check for null.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the non-null value.</returns>
    let okIfNotNull (value: 'a when 'a : null) : Check<'a> =
        if isNull value then Error () else Ok value

    /// <summary>Returns success when the value is null.</summary>
    /// <param name="value">The value to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the value is null.</returns>
    let okIfNull (value: 'a when 'a : null) : Check<unit> =
        if isNull value then Ok () else Error ()

    /// <summary>Returns success when the value is null.</summary>
    /// <param name="value">The value to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the value is null.</returns>
    let failIfNotNull (value: 'a when 'a : null) : Check<unit> =
        if isNull value then Error () else Ok ()

    /// <summary>Returns the value when it is null.</summary>
    /// <param name="value">The value to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the null value.</returns>
    let failIfNull (value: 'a when 'a : null) : Check<'a> =
        if isNull value then Ok value else Error ()

    /// <summary>Returns the sequence when it is not empty.</summary>
    /// <param name="coll">The sequence of type <c>seq&lt;'a&gt;</c> to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the non-empty sequence.</returns>
    let okIfNotEmpty (coll: seq<'a>) : Check<seq<'a>> =
        if Seq.isEmpty coll then Error () else Ok coll

    /// <summary>Returns success when the sequence is empty.</summary>
    /// <param name="coll">The sequence to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the sequence is empty.</returns>
    let okIfEmpty (coll: seq<'a>) : Check<unit> =
        if Seq.isEmpty coll then Ok () else Error ()

    /// <summary>Returns success when the sequence is empty.</summary>
    /// <param name="coll">The sequence to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the sequence is empty.</returns>
    let failIfNotEmpty (coll: seq<'a>) : Check<unit> =
        if Seq.isEmpty coll then Ok () else Error ()

    /// <summary>Returns the sequence when it is not empty.</summary>
    /// <param name="coll">The sequence to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the non-empty sequence.</returns>
    let failIfEmpty (coll: seq<'a>) : Check<seq<'a>> =
        if Seq.isEmpty coll then Error () else Ok coll

    /// <summary>Returns success when the values are equal.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="actual">The actual value.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the values are equal.</returns>
    let okIfEqual (expected: 'a) (actual: 'a) : Check<unit> =
        if expected = actual then Ok () else Error ()

    /// <summary>Returns success when the values are not equal.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="actual">The actual value.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the values differ.</returns>
    let okIfNotEqual (expected: 'a) (actual: 'a) : Check<unit> =
        if expected <> actual then Ok () else Error ()

    /// <summary>Returns success when the values are equal.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="actual">The actual value.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the values are equal.</returns>
    let failIfEqual (expected: 'a) (actual: 'a) : Check<unit> =
        if expected = actual then Error () else Ok ()

    /// <summary>Returns success when the values are not equal.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="actual">The actual value.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the values differ.</returns>
    let failIfNotEqual (expected: 'a) (actual: 'a) : Check<unit> =
        if expected <> actual then Error () else Ok ()

    /// <summary>Returns the string when it is not null or empty.</summary>
    /// <param name="str">The string to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the non-empty string.</returns>
    let okIfNonEmptyStr (str: string) : Check<string> =
        if String.IsNullOrEmpty str then Error () else Ok str

    /// <summary>Returns success when the string is null or empty.</summary>
    /// <param name="str">The string to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the string is null or empty.</returns>
    let okIfEmptyStr (str: string) : Check<unit> =
        if String.IsNullOrEmpty str then Ok () else Error ()

    /// <summary>Returns success when the string is null or empty.</summary>
    /// <param name="str">The string to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the string is null or empty.</returns>
    let failIfNonEmptyStr (str: string) : Check<unit> =
        if String.IsNullOrEmpty str then Ok () else Error ()

    /// <summary>Returns the string when it is null or empty.</summary>
    /// <param name="str">The string to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the empty or null string.</returns>
    let failIfEmptyStr (str: string) : Check<string> =
        if String.IsNullOrEmpty str then Error () else Ok str

    /// <summary>Returns the string when it is not blank.</summary>
    /// <param name="str">The string to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the non-blank string.</returns>
    let okIfNotBlank (str: string) : Check<string> =
        if String.IsNullOrWhiteSpace str then Error () else Ok str

    /// <summary>Returns the string when it is not blank.</summary>
    /// <param name="str">The string to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the non-blank string.</returns>
    let notBlank (str: string) : Check<string> =
        fromPredicate (fun value -> Operators.not (String.IsNullOrWhiteSpace value)) str

    /// <summary>Returns success when the string is blank.</summary>
    /// <param name="str">The string to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the string is null, empty, or whitespace.</returns>
    let okIfBlank (str: string) : Check<unit> =
        if String.IsNullOrWhiteSpace str then Ok () else Error ()

    /// <summary>Returns success when the string is blank.</summary>
    /// <param name="str">The string to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the string is blank.</returns>
    let blank (str: string) : Check<unit> =
        okIfBlank str

    /// <summary>Returns success when the string is blank.</summary>
    /// <param name="str">The string to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the string is blank.</returns>
    let failIfNotBlank (str: string) : Check<unit> =
        if String.IsNullOrWhiteSpace str then Ok () else Error ()

    /// <summary>Returns the string when it is blank.</summary>
    /// <param name="str">The string to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the blank string.</returns>
    let failIfBlank (str: string) : Check<string> =
        if String.IsNullOrWhiteSpace str then Error () else Ok str

    /// <summary>Returns the value when it is not null.</summary>
    /// <param name="value">The value to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the non-null value.</returns>
    let notNull (value: 'a when 'a : null) : Check<'a> =
        fromPredicate (fun inner -> Operators.not (isNull inner)) value

    /// <summary>Returns the sequence when it is not empty.</summary>
    /// <param name="coll">The sequence to check.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> containing the non-empty sequence.</returns>
    let notEmpty (coll: seq<'a>) : Check<seq<'a>> =
        fromPredicate (fun inner -> Operators.not (Seq.isEmpty inner)) coll

    /// <summary>Returns success when the values are equal.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="actual">The actual value.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the values are equal.</returns>
    let equal (expected: 'a) (actual: 'a) : Check<unit> =
        okIfEqual expected actual

    /// <summary>Returns success when the values are not equal.</summary>
    /// <param name="expected">The expected value.</param>
    /// <param name="actual">The actual value.</param>
    /// <returns>A <see cref="T:FsFlow.Check`1" /> that succeeds if the values differ.</returns>
    let notEqual (expected: 'a) (actual: 'a) : Check<unit> =
        okIfNotEqual expected actual

    /// <summary>Maps a unit error into the supplied application error value.</summary>
    /// <remarks>
    /// This is the primary bridge from pure checks to domain-specific results.
    /// </remarks>
    /// <param name="error">The domain error of type <c>'error</c> to return on failure.</param>
    /// <param name="result">The source <see cref="T:FsFlow.Check`1" />.</param>
    /// <returns>A <see cref="T:System.Result`2" /> with the provided error value.</returns>
    [<Obsolete("Use Result.mapErrorTo instead.")>]
    let orElse (error: 'error) (result: Check<'value>) : Result<'value, 'error> =
        match result with
        | Ok value -> Ok value
        | Error () -> Error error

    /// <summary>Maps a unit error into an application error produced on demand.</summary>
    /// <param name="errorFn">A function of type <c>unit -> 'error</c> to produce the error.</param>
    /// <param name="result">The source <see cref="T:FsFlow.Check`1" />.</param>
    /// <returns>A <see cref="T:System.Result`2" /> with the produced error value.</returns>
    [<Obsolete("Use Result.mapError instead.")>]
    let orElseWith (errorFn: unit -> 'error) (result: Check<'value>) : Result<'value, 'error> =
        match result with
        | Ok value -> Ok value
        | Error () -> Error(errorFn ())

/// <summary>
/// Computation expression builder for fail-fast <see cref="T:System.Result`2" /> workflows.
/// </summary>
/// <exclude/>
type ResultBuilder() =
    member _.Return(value: 'value) : Result<'value, 'error> =
        Ok value

    member _.ReturnFrom(result: Result<'value, 'error>) : Result<'value, 'error> =
        result

    member _.Zero() : Result<unit, 'error> =
        Ok ()

    member _.Bind
        (
            result: Result<'value, 'error>,
            binder: 'value -> Result<'next, 'error>
        ) : Result<'next, 'error> =
        Result.bind binder result

    member _.Delay(factory: unit -> Result<'value, 'error>) : Result<'value, 'error> =
        factory ()

    member _.Run(result: Result<'value, 'error>) : Result<'value, 'error> =
        result

    member _.Combine
        (
            first: Result<unit, 'error>,
            second: Result<'value, 'error>
        ) : Result<'value, 'error> =
        Result.bind (fun () -> second) first

    member _.TryWith
        (
            result: Result<'value, 'error>,
            handler: exn -> Result<'value, 'error>
        ) : Result<'value, 'error> =
        try
            result
        with error ->
            handler error

    member _.TryFinally(result: Result<'value, 'error>, compensation: unit -> unit) : Result<'value, 'error> =
        try
            result
        finally
            compensation ()

    member this.Using
        (
            resource: 'resource,
            binder: 'resource -> Result<'value, 'error>
        ) : Result<'value, 'error>
        when 'resource :> IDisposable =
        this.TryFinally(
            binder resource,
            fun () ->
                if not (isNull (box resource)) then
                    resource.Dispose()
        )

    member this.While
        (
            guard: unit -> bool,
            body: Result<unit, 'error>
        ) : Result<unit, 'error> =
        if guard () then
            this.Bind(body, fun () -> this.While(guard, body))
        else
            this.Zero()

    member this.For
        (
            sequence: seq<'value>,
            binder: 'value -> Result<unit, 'error>
        ) : Result<unit, 'error> =
        this.Using(
            sequence.GetEnumerator(),
            fun enumerator -> this.While(enumerator.MoveNext, this.Delay(fun () -> binder enumerator.Current))
        )

/// <summary>
/// Computation expression builder for accumulating <see cref="T:FsFlow.Validation`2" /> workflows.
/// </summary>
/// <exclude/>
type ValidateBuilder() =
    member _.Return(value: 'value) : Validation<'value, 'error> =
        Validation.succeed value

    member _.ReturnFrom(validation: Validation<'value, 'error>) : Validation<'value, 'error> =
        validation

    member _.ReturnFrom(result: Result<'value, 'error>) : Validation<'value, 'error> =
        Validation.fromResult result

    member _.Zero() : Validation<unit, 'error> =
        Validation.succeed ()

    member _.Bind
        (
            validation: Validation<'value, 'error>,
            binder: 'value -> Validation<'next, 'error>
        ) : Validation<'next, 'error> =
        Validation.bind binder validation

    member _.Bind
        (
            result: Result<'value, 'error>,
            binder: 'value -> Validation<'next, 'error>
        ) : Validation<'next, 'error> =
        result
        |> Validation.fromResult
        |> Validation.bind binder

    member _.Delay(factory: unit -> Validation<'value, 'error>) : Validation<'value, 'error> =
        factory ()

    member _.Run(validation: Validation<'value, 'error>) : Validation<'value, 'error> =
        validation

    member _.Combine
        (
            first: Validation<unit, 'error>,
            second: Validation<'value, 'error>
        ) : Validation<'value, 'error> =
        Validation.bind (fun () -> second) first

    member _.MergeSources
        (
            left: Validation<'left, 'error>,
            right: Validation<'right, 'error>
        ) : Validation<'left * 'right, 'error> =
        Validation.map2 (fun leftValue rightValue -> leftValue, rightValue) left right

    member _.TryWith
        (
            validation: Validation<'value, 'error>,
            handler: exn -> Validation<'value, 'error>
        ) : Validation<'value, 'error> =
        try
            validation
        with error ->
            handler error

    member _.TryFinally(validation: Validation<'value, 'error>, compensation: unit -> unit) : Validation<'value, 'error> =
        try
            validation
        finally
            compensation ()

    member this.Using
        (
            resource: 'resource,
            binder: 'resource -> Validation<'value, 'error>
        ) : Validation<'value, 'error>
        when 'resource :> IDisposable =
        this.TryFinally(
            binder resource,
            fun () ->
                if not (isNull (box resource)) then
                    resource.Dispose()
        )

    member this.While
        (
            guard: unit -> bool,
            body: Validation<unit, 'error>
        ) : Validation<unit, 'error> =
        if guard () then
            this.Bind(body, fun () -> this.While(guard, body))
        else
            this.Zero()

    member this.For
        (
            sequence: seq<'value>,
            binder: 'value -> Validation<unit, 'error>
        ) : Validation<unit, 'error> =
        this.Using(
            sequence.GetEnumerator(),
            fun enumerator -> this.While(enumerator.MoveNext, this.Delay(fun () -> binder enumerator.Current))
        )
