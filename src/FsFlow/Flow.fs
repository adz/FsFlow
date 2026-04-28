namespace FsFlow

open System

/// <summary>
/// Represents a cold synchronous workflow that depends on an environment,
/// can fail with a typed error, and can succeed with a value.
/// </summary>
/// <typeparam name="env">The type of the environment dependency.</typeparam>
/// <typeparam name="error">The type of the failure value.</typeparam>
/// <typeparam name="value">The type of the success value.</typeparam>
type Flow<'env, 'error, 'value> =
    private
    | Flow of ('env -> Result<'value, 'error>)

/// <summary>
/// Represents a cold async workflow that depends on an environment,
/// can fail with a typed error, and can succeed with a value.
/// </summary>
/// <typeparam name="env">The type of the environment dependency.</typeparam>
/// <typeparam name="error">The type of the failure value.</typeparam>
/// <typeparam name="value">The type of the success value.</typeparam>
type AsyncFlow<'env, 'error, 'value> =
    private
    | AsyncFlow of ('env -> Async<Result<'value, 'error>>)

/// <summary>
/// Log levels used by runtime logging helpers.
/// </summary>
[<RequireQualifiedAccess>]
type LogLevel =
    | Trace
    | Debug
    | Information
    | Warning
    | Error
    | Critical

/// <summary>
/// A log entry written through a runtime logger.
/// </summary>
type LogEntry =
    {
      Level: LogLevel
      Message: string
      TimestampUtc: DateTimeOffset
    }

/// <summary>
/// Defines how runtime retry helpers should repeat typed failures.
/// </summary>
type RetryPolicy<'error> =
    {
      MaxAttempts: int
      Delay: int -> TimeSpan
      ShouldRetry: 'error -> bool
    }

/// <summary>
/// Standard retry policies.
/// </summary>
[<RequireQualifiedAccess>]
module RetryPolicy =
    let noDelay (maxAttempts: int) : RetryPolicy<'error> =
        { MaxAttempts = maxAttempts
          Delay = fun _ -> TimeSpan.Zero
          ShouldRetry = fun _ -> true }

module private ResultFlow =
    let map
        (mapper: 'value -> 'next)
        (result: Result<'value, 'error>)
        : Result<'next, 'error> =
        Result.map mapper result

    let bind
        (binder: 'value -> Result<'next, 'error>)
        (result: Result<'value, 'error>)
        : Result<'next, 'error> =
        Result.bind binder result

    let mapError
        (mapper: 'error -> 'nextError)
        (result: Result<'value, 'error>)
        : Result<'value, 'nextError> =
        Result.mapError mapper result

module internal OptionFlow =
    let toUnitResult (value: 'value option) : Result<'value, unit> =
        match value with
        | Some innerValue -> Ok innerValue
        | None -> Error()

    let toUnitResultValueOption (value: 'value voption) : Result<'value, unit> =
        match value with
        | ValueSome innerValue -> Ok innerValue
        | ValueNone -> Error()

    let toResult (error: 'error) (value: 'value option) : Result<'value, 'error> =
        match value with
        | Some innerValue -> Ok innerValue
        | None -> Error error

    let toResultValueOption (error: 'error) (value: 'value voption) : Result<'value, 'error> =
        match value with
        | ValueSome innerValue -> Ok innerValue
        | ValueNone -> Error error

module internal InternalCombinatorCore =
    let mapWith
        (mapOutcome: (Result<'value, 'error> -> Result<'next, 'error>) -> 'operation -> 'nextOperation)
        (mapper: 'value -> 'next)
        (operation: 'context -> 'operation)
        : 'context -> 'nextOperation =
        fun context -> operation context |> mapOutcome (Result.map mapper)

    let bindWith
        (bindOutcome: 'operation -> ('value -> 'nextOperation) -> ('error -> 'nextOperation) -> 'nextOperation)
        (continueWith: 'context -> 'value -> 'nextOperation)
        (failWith: 'error -> 'nextOperation)
        (operation: 'context -> 'operation)
        : 'context -> 'nextOperation =
        fun context -> bindOutcome (operation context) (continueWith context) failWith

    let mapErrorWith
        (mapOutcome: (Result<'value, 'error> -> Result<'value, 'nextError>) -> 'operation -> 'nextOperation)
        (mapper: 'error -> 'nextError)
        (operation: 'context -> 'operation)
        : 'context -> 'nextOperation =
        fun context -> operation context |> mapOutcome (Result.mapError mapper)

    let localEnvWith
        (run: 'innerEnvironment -> 'flow -> 'operation)
        (mapping: 'outerEnvironment -> 'innerEnvironment)
        (flow: 'flow)
        : 'outerEnvironment -> 'operation =
        fun environment -> flow |> run (mapping environment)

    let delayWith
        (run: 'environment -> 'flow -> 'operation)
        (factory: unit -> 'flow)
        : 'environment -> 'operation =
        fun environment -> factory () |> run environment

/// <summary>
/// Core functions for creating, composing, and executing synchronous flows.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module Flow =
    let run (environment: 'env) (Flow operation: Flow<'env, 'error, 'value>) : Result<'value, 'error> =
        operation environment

    let succeed (value: 'value) : Flow<'env, 'error, 'value> =
        Flow(fun _ -> Ok value)

    let value (item: 'value) : Flow<'env, 'error, 'value> =
        succeed item

    let fail (error: 'error) : Flow<'env, 'error, 'value> =
        Flow(fun _ -> Error error)

    let fromResult (result: Result<'value, 'error>) : Flow<'env, 'error, 'value> =
        Flow(fun _ -> result)

    let fromOption (error: 'error) (value: 'value option) : Flow<'env, 'error, 'value> =
        value
        |> OptionFlow.toResult error
        |> fromResult

    let fromValueOption (error: 'error) (value: 'value voption) : Flow<'env, 'error, 'value> =
        value
        |> OptionFlow.toResultValueOption error
        |> fromResult

    let env<'env, 'error> : Flow<'env, 'error, 'env> =
        Flow(fun environment -> Ok environment)

    let read (projection: 'env -> 'value) : Flow<'env, 'error, 'value> =
        Flow(fun environment -> Ok(projection environment))

    let map
        (mapper: 'value -> 'next)
        (flow: Flow<'env, 'error, 'value>)
        : Flow<'env, 'error, 'next> =
        Flow(InternalCombinatorCore.mapWith (fun mapOutcome outcome -> mapOutcome outcome) mapper (fun environment -> run environment flow))

    let bind
        (binder: 'value -> Flow<'env, 'error, 'next>)
        (flow: Flow<'env, 'error, 'value>)
        : Flow<'env, 'error, 'next> =
        Flow(
            InternalCombinatorCore.bindWith
                (fun outcome onSuccess onError ->
                    match outcome with
                    | Ok value -> onSuccess value
                    | Error error -> onError error)
                (fun environment value -> binder value |> run environment)
                Error
                (fun environment -> run environment flow)
        )

    let tap
        (binder: 'value -> Flow<'env, 'error, unit>)
        (flow: Flow<'env, 'error, 'value>)
        : Flow<'env, 'error, 'value> =
        bind
            (fun value ->
                binder value
                |> map (fun () -> value))
            flow

    let tapError
        (binder: 'error -> Flow<'env, 'error, unit>)
        (flow: Flow<'env, 'error, 'value>)
        : Flow<'env, 'error, 'value> =
        Flow(fun environment ->
            match run environment flow with
            | Ok value -> Ok value
            | Error error ->
                match binder error |> run environment with
                | Ok () -> Error error
                | Error nextError -> Error nextError)

    let mapError
        (mapper: 'error -> 'nextError)
        (flow: Flow<'env, 'error, 'value>)
        : Flow<'env, 'nextError, 'value> =
        Flow(
            InternalCombinatorCore.mapErrorWith
                (fun mapOutcome outcome -> mapOutcome outcome)
                mapper
                (fun environment -> run environment flow)
        )

    let catch
        (handler: exn -> 'error)
        (flow: Flow<'env, 'error, 'value>)
        : Flow<'env, 'error, 'value> =
        Flow(fun environment ->
            try
                run environment flow
            with error ->
                Error(handler error))

    let orElse
        (fallback: Flow<'env, 'error, 'value>)
        (flow: Flow<'env, 'error, 'value>)
        : Flow<'env, 'error, 'value> =
        Flow(fun environment ->
            match run environment flow with
            | Ok value -> Ok value
            | Error _ -> run environment fallback)

    let zip
        (left: Flow<'env, 'error, 'left>)
        (right: Flow<'env, 'error, 'right>)
        : Flow<'env, 'error, 'left * 'right> =
        bind
            (fun leftValue ->
                right
                |> map (fun rightValue -> leftValue, rightValue))
            left

    let map2
        (mapper: 'left -> 'right -> 'value)
        (left: Flow<'env, 'error, 'left>)
        (right: Flow<'env, 'error, 'right>)
        : Flow<'env, 'error, 'value> =
        zip left right
        |> map (fun (leftValue, rightValue) -> mapper leftValue rightValue)

    let localEnv
        (mapping: 'outerEnvironment -> 'innerEnvironment)
        (flow: Flow<'innerEnvironment, 'error, 'value>)
        : Flow<'outerEnvironment, 'error, 'value> =
        Flow(InternalCombinatorCore.localEnvWith run mapping flow)

    let delay (factory: unit -> Flow<'env, 'error, 'value>) : Flow<'env, 'error, 'value> =
        Flow(InternalCombinatorCore.delayWith run factory)

    /// <summary>
    /// Transforms a sequence of values into a flow by applying a flow-producing function to each element.
    /// </summary>
    let traverse
        (mapping: 'value -> Flow<'env, 'error, 'next>)
        (values: seq<'value>)
        : Flow<'env, 'error, 'next list> =
        Flow(fun environment ->
            let results = ResizeArray()
            let mutable currentError = None
            use enumerator = values.GetEnumerator()

            while currentError.IsNone && enumerator.MoveNext() do
                match mapping enumerator.Current |> run environment with
                | Ok value -> results.Add value
                | Error error -> currentError <- Some error

            match currentError with
            | Some error -> Error error
            | None -> Ok(List.ofSeq results))

    /// <summary>
    /// Transforms a sequence of flows into a flow of a sequence.
    /// </summary>
    let sequence (flows: seq<Flow<'env, 'error, 'value>>) : Flow<'env, 'error, 'value list> =
        traverse id flows

/// <summary>
/// Core functions for creating, composing, and executing async flows.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module AsyncFlow =
    let run
        (environment: 'env)
        (AsyncFlow operation: AsyncFlow<'env, 'error, 'value>)
        : Async<Result<'value, 'error>> =
        operation environment

    let toAsync (environment: 'env) (flow: AsyncFlow<'env, 'error, 'value>) : Async<Result<'value, 'error>> =
        run environment flow

    let succeed (value: 'value) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(fun _ -> async.Return(Ok value))

    let fail (error: 'error) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(fun _ -> async.Return(Error error))

    let fromResult (result: Result<'value, 'error>) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(fun _ -> async.Return result)

    let fromOption (error: 'error) (value: 'value option) : AsyncFlow<'env, 'error, 'value> =
        value
        |> OptionFlow.toResult error
        |> fromResult

    let fromValueOption (error: 'error) (value: 'value voption) : AsyncFlow<'env, 'error, 'value> =
        value
        |> OptionFlow.toResultValueOption error
        |> fromResult

    let fromFlow (flow: Flow<'env, 'error, 'value>) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(fun environment -> async.Return(Flow.run environment flow))

    let fromAsync (operation: Async<'value>) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(fun _ ->
            async {
                let! value = operation
                return Ok value
            })

    let fromAsyncResult (operation: Async<Result<'value, 'error>>) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(fun _ -> operation)

    let env<'env, 'error> : AsyncFlow<'env, 'error, 'env> =
        AsyncFlow(fun environment -> async.Return(Ok environment))

    let read (projection: 'env -> 'value) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(fun environment -> async.Return(Ok(projection environment)))

    let map
        (mapper: 'value -> 'next)
        (flow: AsyncFlow<'env, 'error, 'value>)
        : AsyncFlow<'env, 'error, 'next> =
        AsyncFlow(
            InternalCombinatorCore.mapWith
                (fun mapOutcome operation ->
                    async {
                        let! result = operation
                        return mapOutcome result
                    })
                mapper
                (fun environment -> run environment flow)
        )

    let bind
        (binder: 'value -> AsyncFlow<'env, 'error, 'next>)
        (flow: AsyncFlow<'env, 'error, 'value>)
        : AsyncFlow<'env, 'error, 'next> =
        AsyncFlow(
            InternalCombinatorCore.bindWith
                (fun operation onSuccess onError ->
                    async {
                        let! result = operation

                        match result with
                        | Ok value -> return! onSuccess value
                        | Error error -> return! onError error
                    })
                (fun environment value -> binder value |> run environment)
                (Error >> async.Return)
                (fun environment -> run environment flow)
        )

    let tap
        (binder: 'value -> AsyncFlow<'env, 'error, unit>)
        (flow: AsyncFlow<'env, 'error, 'value>)
        : AsyncFlow<'env, 'error, 'value> =
        bind
            (fun value ->
                binder value
                |> map (fun () -> value))
            flow

    let tapError
        (binder: 'error -> AsyncFlow<'env, 'error, unit>)
        (flow: AsyncFlow<'env, 'error, 'value>)
        : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(fun environment ->
            async {
                let! result = run environment flow

                match result with
                | Ok value -> return Ok value
                | Error error ->
                    let! tapResult = binder error |> run environment

                    match tapResult with
                    | Ok () -> return Error error
                    | Error nextError -> return Error nextError
            })

    let mapError
        (mapper: 'error -> 'nextError)
        (flow: AsyncFlow<'env, 'error, 'value>)
        : AsyncFlow<'env, 'nextError, 'value> =
        AsyncFlow(
            InternalCombinatorCore.mapErrorWith
                (fun mapOutcome operation ->
                    async {
                        let! result = operation
                        return mapOutcome result
                    })
                mapper
                (fun environment -> run environment flow)
        )

    let catch
        (handler: exn -> 'error)
        (flow: AsyncFlow<'env, 'error, 'value>)
        : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(fun environment ->
            async {
                try
                    return! run environment flow
                with error ->
                    return Error(handler error)
            })

    let orElse
        (fallback: AsyncFlow<'env, 'error, 'value>)
        (flow: AsyncFlow<'env, 'error, 'value>)
        : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(fun environment ->
            async {
                let! result = run environment flow

                match result with
                | Ok value -> return Ok value
                | Error _ -> return! run environment fallback
            })

    let zip
        (left: AsyncFlow<'env, 'error, 'left>)
        (right: AsyncFlow<'env, 'error, 'right>)
        : AsyncFlow<'env, 'error, 'left * 'right> =
        bind
            (fun leftValue ->
                right
                |> map (fun rightValue -> leftValue, rightValue))
            left

    let map2
        (mapper: 'left -> 'right -> 'value)
        (left: AsyncFlow<'env, 'error, 'left>)
        (right: AsyncFlow<'env, 'error, 'right>)
        : AsyncFlow<'env, 'error, 'value> =
        zip left right
        |> map (fun (leftValue, rightValue) -> mapper leftValue rightValue)

    let localEnv
        (mapping: 'outerEnvironment -> 'innerEnvironment)
        (flow: AsyncFlow<'innerEnvironment, 'error, 'value>)
        : AsyncFlow<'outerEnvironment, 'error, 'value> =
        AsyncFlow(InternalCombinatorCore.localEnvWith run mapping flow)

    let delay (factory: unit -> AsyncFlow<'env, 'error, 'value>) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(InternalCombinatorCore.delayWith run factory)

    /// <summary>
    /// Transforms a sequence of values into an async flow by applying an async flow-producing function to each element.
    /// </summary>
    let traverse
        (mapping: 'value -> AsyncFlow<'env, 'error, 'next>)
        (values: seq<'value>)
        : AsyncFlow<'env, 'error, 'next list> =
        AsyncFlow(fun environment ->
            async {
                let results = ResizeArray()
                let mutable currentError = None
                use enumerator = values.GetEnumerator()

                while currentError.IsNone && enumerator.MoveNext() do
                    let! outcome = mapping enumerator.Current |> run environment

                    match outcome with
                    | Ok value -> results.Add value
                    | Error error -> currentError <- Some error

                match currentError with
                | Some error -> return Error error
                | None -> return Ok(List.ofSeq results)
            })

    /// <summary>
    /// Transforms a sequence of async flows into an async flow of a sequence.
    /// </summary>
    let sequence (flows: seq<AsyncFlow<'env, 'error, 'value>>) : AsyncFlow<'env, 'error, 'value list> =
        traverse id flows

    /// <summary>
    /// Runtime helpers for operational concerns like logging, timeout, retry, and cleanup.
    /// </summary>
    [<RequireQualifiedAccess>]
    module Runtime =
        open System.Threading
        open System.Threading.Tasks

        /// <summary>
        /// Reads the current cancellation token from the flow.
        /// </summary>
        /// <remarks>This observes the runtime token; it does not translate cancellation into a typed error by itself.</remarks>
        let cancellationToken<'env, 'error> : AsyncFlow<'env, 'error, CancellationToken> =
            AsyncFlow(fun _environment ->
                async {
                    let! cancellationToken = Async.CancellationToken
                    return Ok cancellationToken
                })

        /// <summary>
        /// Catches <see cref="OperationCanceledException"/> and converts it into a typed error.
        /// </summary>
        /// <param name="handler">The function to convert the exception.</param>
        /// <param name="flow">The source flow.</param>
        /// <remarks>This translates cancellation exceptions raised during execution. It does not pre-check the token.</remarks>
        let catchCancellation
            (handler: OperationCanceledException -> 'error)
            (flow: AsyncFlow<'env, 'error, 'value>)
            : AsyncFlow<'env, 'error, 'value> =
            AsyncFlow(fun environment ->
                async {
                    try
                        return! run environment flow
                    with :? OperationCanceledException as error ->
                        return Error(handler error)
                })

        /// <summary>
        /// Checks if cancellation has been requested and returns a typed error if it has.
        /// </summary>
        /// <param name="canceledError">The error to return if canceled.</param>
        /// <remarks>This observes the current token state and returns a typed error immediately instead of waiting for an exception.</remarks>
        let ensureNotCanceled<'env, 'error> (canceledError: 'error) : AsyncFlow<'env, 'error, unit> =
            AsyncFlow(fun _environment ->
                async {
                    let! cancellationToken = Async.CancellationToken

                    if cancellationToken.IsCancellationRequested then
                        return Error canceledError
                    else
                        return Ok ()
                })

        /// <summary>
        /// Suspends the flow for the specified duration, observing cancellation.
        /// </summary>
        /// <param name="delay">The duration to sleep.</param>
        /// <remarks>If the runtime token is canceled, the underlying task raises cancellation which can be translated with <see cref="catchCancellation"/>.</remarks>
        let sleep<'env, 'error> (delay: TimeSpan) : AsyncFlow<'env, 'error, unit> =
            AsyncFlow(fun _environment ->
                async {
                    let! cancellationToken = Async.CancellationToken
                    do! Task.Delay(delay, cancellationToken) |> Async.AwaitTask
                    return Ok ()
                })

        /// <summary>
        /// Writes a log entry using the writer provided by the environment.
        /// </summary>
        /// <param name="writer">The logging function extracted from the environment.</param>
        /// <param name="level">The log level.</param>
        /// <param name="message">The message.</param>
        let log
            (writer: 'env -> LogEntry -> unit)
            (level: LogLevel)
            (message: string)
            : AsyncFlow<'env, 'error, unit> =
            AsyncFlow(fun environment ->
                async {
                    writer
                        environment
                        { Level = level
                          Message = message
                          TimestampUtc = DateTimeOffset.UtcNow }

                    return Ok ()
                })

        /// <summary>
        /// Writes a log entry using a message produced from the environment.
        /// </summary>
        /// <param name="writer">The logging function extracted from the environment.</param>
        /// <param name="level">The log level.</param>
        /// <param name="messageFactory">The function to produce the message from the environment.</param>
        let logWith
            (writer: 'env -> LogEntry -> unit)
            (level: LogLevel)
            (messageFactory: 'env -> string)
            : AsyncFlow<'env, 'error, unit> =
            AsyncFlow(fun environment ->
                async {
                    writer
                        environment
                        { Level = level
                          Message = messageFactory environment
                          TimestampUtc = DateTimeOffset.UtcNow }

                    return Ok ()
                })

        /// <summary>
        /// Safely acquires a resource, uses it, and ensures it is released via a task-based action.
        /// </summary>
        /// <param name="acquire">The flow that acquires the resource.</param>
        /// <param name="release">The function that releases the resource.</param>
        /// <param name="useResource">The flow that uses the resource.</param>
        let useWithAcquireRelease
            (acquire: AsyncFlow<'env, 'error, 'resource>)
            (release: 'resource -> CancellationToken -> Task)
            (useResource: 'resource -> AsyncFlow<'env, 'error, 'value>)
            : AsyncFlow<'env, 'error, 'value> =
            bind
                (fun resource ->
                    AsyncFlow(fun environment ->
                        async {
                            let! cancellationToken = Async.CancellationToken
                            let! result = run environment (useResource resource) |> Async.Catch

                            do! release resource cancellationToken |> Async.AwaitTask

                            match result with
                            | Choice1Of2 (Ok value) -> return Ok value
                            | Choice1Of2 (Error error) -> return Error error
                            | Choice2Of2 error -> return raise error
                        }))
                acquire

        /// <summary>
        /// Wraps a flow with a timeout. If the flow does not complete within the specified duration, returns a typed error.
        /// </summary>
        /// <param name="after">The duration after which to timeout.</param>
        /// <param name="timeoutError">The error to return on timeout.</param>
        /// <param name="flow">The flow to wrap.</param>
        /// <remarks>This helper translates timeout into a typed error. It does not automatically cancel the underlying work on timeout.</remarks>
        let timeout
            (after: TimeSpan)
            (timeoutError: 'error)
            (flow: AsyncFlow<'env, 'error, 'value>)
            : AsyncFlow<'env, 'error, 'value> =
            AsyncFlow(fun environment ->
                async {
                    let! cancellationToken = Async.CancellationToken
                    let operation =
                        run environment flow
                        |> fun asyncOperation -> Async.StartAsTask(asyncOperation, cancellationToken = cancellationToken)

                    let timeoutTask = Task.Delay after
                    let! completed = Task.WhenAny([| operation :> Task; timeoutTask |]) |> Async.AwaitTask

                    if obj.ReferenceEquals(completed, timeoutTask) then
                        return Error timeoutError
                    else
                        return! operation |> Async.AwaitTask
                })

        /// <summary>
        /// Wraps a flow with a timeout. If the flow does not complete within the specified duration, returns a success value.
        /// </summary>
        let timeoutToOk
            (after: TimeSpan)
            (value: 'value)
            (flow: AsyncFlow<'env, 'error, 'value>)
            : AsyncFlow<'env, 'error, 'value> =
            AsyncFlow(fun environment ->
                async {
                    let! cancellationToken = Async.CancellationToken
                    let operation =
                        run environment flow
                        |> fun asyncOperation -> Async.StartAsTask(asyncOperation, cancellationToken = cancellationToken)

                    let timeoutTask = Task.Delay after
                    let! completed = Task.WhenAny([| operation :> Task; timeoutTask |]) |> Async.AwaitTask

                    if obj.ReferenceEquals(completed, timeoutTask) then
                        return Ok value
                    else
                        return! operation |> Async.AwaitTask
                })

        /// <summary>
        /// Transitions to a failure value on timeout.
        /// </summary>
        let timeoutToError
            (after: TimeSpan)
            (error: 'error)
            (flow: AsyncFlow<'env, 'error, 'value>)
            : AsyncFlow<'env, 'error, 'value> =
            timeout after error flow

        /// <summary>
        /// Transitions to a fallback workflow on timeout.
        /// </summary>
        let timeoutWith
            (after: TimeSpan)
            (fallback: unit -> AsyncFlow<'env, 'error, 'value>)
            (flow: AsyncFlow<'env, 'error, 'value>)
            : AsyncFlow<'env, 'error, 'value> =
            AsyncFlow(fun environment ->
                async {
                    let! cancellationToken = Async.CancellationToken
                    let operation =
                        run environment flow
                        |> fun asyncOperation -> Async.StartAsTask(asyncOperation, cancellationToken = cancellationToken)

                    let timeoutTask = Task.Delay after
                    let! completed = Task.WhenAny([| operation :> Task; timeoutTask |]) |> Async.AwaitTask

                    if obj.ReferenceEquals(completed, timeoutTask) then
                        return! run environment (fallback ())
                    else
                        return! operation |> Async.AwaitTask
                })

        /// <summary>
        /// Retries a flow according to the specified policy.
        /// </summary>
        /// <param name="policy">The retry policy.</param>
        /// <param name="flow">The flow to retry.</param>
        let retry
            (policy: RetryPolicy<'error>)
            (flow: AsyncFlow<'env, 'error, 'value>)
            : AsyncFlow<'env, 'error, 'value> =
            if policy.MaxAttempts < 1 then
                invalidArg (nameof policy.MaxAttempts) "RetryPolicy.MaxAttempts must be at least 1."

            let rec loop attempt =
                AsyncFlow(fun environment ->
                    async {
                        let! result = run environment flow

                        match result with
                        | Ok value -> return Ok value
                        | Error error when attempt < policy.MaxAttempts && policy.ShouldRetry error ->
                            let delay = policy.Delay attempt
                            let! cancellationToken = Async.CancellationToken

                            if delay > TimeSpan.Zero then
                                do! Task.Delay(delay, cancellationToken) |> Async.AwaitTask

                            return! run environment (loop (attempt + 1))
                        | Error error ->
                            return Error error
                    })

            loop 1

/// <summary>
/// Computation expression builder for synchronous <see cref="T:FsFlow.Flow`3" /> workflows.
/// </summary>
type FlowBuilder() =
    member _.Return(value: 'value) : Flow<'env, 'error, 'value> =
        Flow.succeed value

    member _.ReturnFrom(flow: Flow<'env, 'error, 'value>) : Flow<'env, 'error, 'value> =
        flow

    member _.ReturnFrom(result: Result<'value, 'error>) : Flow<'env, 'error, 'value> =
        Flow.fromResult result

    member _.ReturnFrom(option: 'value option) : Flow<'env, unit, 'value> =
        option
        |> OptionFlow.toUnitResult
        |> Flow.fromResult

    member _.ReturnFrom(option: 'value voption) : Flow<'env, unit, 'value> =
        option
        |> OptionFlow.toUnitResultValueOption
        |> Flow.fromResult

    member _.Zero() : Flow<'env, 'error, unit> =
        Flow.succeed ()

    member _.Bind
        (
            flow: Flow<'env, 'error, 'value>,
            binder: 'value -> Flow<'env, 'error, 'next>
        ) : Flow<'env, 'error, 'next> =
        Flow.bind binder flow

    member _.Bind
        (
            result: Result<'value, 'error>,
            binder: 'value -> Flow<'env, 'error, 'next>
        ) : Flow<'env, 'error, 'next> =
        result
        |> Flow.fromResult
        |> Flow.bind binder

    member _.Bind
        (
            option: 'value option,
            binder: 'value -> Flow<'env, unit, 'next>
        ) : Flow<'env, unit, 'next> =
        option
        |> OptionFlow.toUnitResult
        |> Flow.fromResult
        |> Flow.bind binder

    member _.Bind
        (
            option: 'value voption,
            binder: 'value -> Flow<'env, unit, 'next>
        ) : Flow<'env, unit, 'next> =
        option
        |> OptionFlow.toUnitResultValueOption
        |> Flow.fromResult
        |> Flow.bind binder

    member _.Delay(factory: unit -> Flow<'env, 'error, 'value>) : Flow<'env, 'error, 'value> =
        Flow.delay factory

    member _.Run(flow: Flow<'env, 'error, 'value>) : Flow<'env, 'error, 'value> =
        flow

    member _.Combine
        (
            first: Flow<'env, 'error, unit>,
            second: Flow<'env, 'error, 'value>
        ) : Flow<'env, 'error, 'value> =
        first
        |> Flow.bind (fun () -> second)

    member _.TryWith
        (
            flow: Flow<'env, 'error, 'value>,
            handler: exn -> Flow<'env, 'error, 'value>
        ) : Flow<'env, 'error, 'value> =
        Flow(fun environment ->
            try
                Flow.run environment flow
            with error ->
                Flow.run environment (handler error))

    member _.TryFinally(flow: Flow<'env, 'error, 'value>, compensation: unit -> unit) : Flow<'env, 'error, 'value> =
        Flow(fun environment ->
            try
                Flow.run environment flow
            finally
                compensation ())

    member this.Using
        (
            resource: 'resource,
            binder: 'resource -> Flow<'env, 'error, 'value>
        ) : Flow<'env, 'error, 'value>
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
            body: Flow<'env, 'error, unit>
        ) : Flow<'env, 'error, unit> =
        if guard () then
            this.Bind(body, fun () -> this.While(guard, body))
        else
            this.Zero()

    member this.For
        (
            sequence: seq<'value>,
            binder: 'value -> Flow<'env, 'error, unit>
        ) : Flow<'env, 'error, unit> =
        this.Using(
            sequence.GetEnumerator(),
            fun enumerator -> this.While(enumerator.MoveNext, this.Delay(fun () -> binder enumerator.Current))
        )

/// <summary>
/// Computation expression builder for async <see cref="T:FsFlow.AsyncFlow`3" /> workflows.
/// </summary>
type AsyncFlowBuilder() =
    member _.Return(value: 'value) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow.succeed value

    member _.ReturnFrom(flow: AsyncFlow<'env, 'error, 'value>) : AsyncFlow<'env, 'error, 'value> =
        flow

    member _.ReturnFrom(operation: Async<'value>) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow.fromAsync operation

    member _.ReturnFrom(operation: Async<Result<'value, 'error>>) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow.fromAsyncResult operation

    member _.ReturnFrom(flow: Flow<'env, 'error, 'value>) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow.fromFlow flow

    member _.ReturnFrom(result: Result<'value, 'error>) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow.fromResult result

    member _.ReturnFrom(option: 'value option) : AsyncFlow<'env, unit, 'value> =
        option
        |> OptionFlow.toUnitResult
        |> AsyncFlow.fromResult

    member _.ReturnFrom(option: 'value voption) : AsyncFlow<'env, unit, 'value> =
        option
        |> OptionFlow.toUnitResultValueOption
        |> AsyncFlow.fromResult

    member _.Zero() : AsyncFlow<'env, 'error, unit> =
        AsyncFlow.succeed ()

    member _.Bind
        (
            flow: AsyncFlow<'env, 'error, 'value>,
            binder: 'value -> AsyncFlow<'env, 'error, 'next>
        ) : AsyncFlow<'env, 'error, 'next> =
        AsyncFlow.bind binder flow

    member _.Bind
        (
            flow: Flow<'env, 'error, 'value>,
            binder: 'value -> AsyncFlow<'env, 'error, 'next>
        ) : AsyncFlow<'env, 'error, 'next> =
        flow
        |> AsyncFlow.fromFlow
        |> AsyncFlow.bind binder

    member _.Bind
        (
            operation: Async<'value>,
            binder: 'value -> AsyncFlow<'env, 'error, 'next>
        ) : AsyncFlow<'env, 'error, 'next> =
        operation
        |> AsyncFlow.fromAsync
        |> AsyncFlow.bind binder

    member _.Bind
        (
            operation: Async<Result<'value, 'error>>,
            binder: 'value -> AsyncFlow<'env, 'error, 'next>
        ) : AsyncFlow<'env, 'error, 'next> =
        operation
        |> AsyncFlow.fromAsyncResult
        |> AsyncFlow.bind binder

    member _.Bind
        (
            result: Result<'value, 'error>,
            binder: 'value -> AsyncFlow<'env, 'error, 'next>
        ) : AsyncFlow<'env, 'error, 'next> =
        result
        |> AsyncFlow.fromResult
        |> AsyncFlow.bind binder

    member _.Bind
        (
            option: 'value option,
            binder: 'value -> AsyncFlow<'env, unit, 'next>
        ) : AsyncFlow<'env, unit, 'next> =
        option
        |> OptionFlow.toUnitResult
        |> AsyncFlow.fromResult
        |> AsyncFlow.bind binder

    member _.Bind
        (
            option: 'value voption,
            binder: 'value -> AsyncFlow<'env, unit, 'next>
        ) : AsyncFlow<'env, unit, 'next> =
        option
        |> OptionFlow.toUnitResultValueOption
        |> AsyncFlow.fromResult
        |> AsyncFlow.bind binder

    member _.Delay(factory: unit -> AsyncFlow<'env, 'error, 'value>) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow.delay factory

    member _.Run(flow: AsyncFlow<'env, 'error, 'value>) : AsyncFlow<'env, 'error, 'value> =
        flow

    member _.Combine
        (
            first: AsyncFlow<'env, 'error, unit>,
            second: AsyncFlow<'env, 'error, 'value>
        ) : AsyncFlow<'env, 'error, 'value> =
        first
        |> AsyncFlow.bind (fun () -> second)

    member _.TryWith
        (
            flow: AsyncFlow<'env, 'error, 'value>,
            handler: exn -> AsyncFlow<'env, 'error, 'value>
        ) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(fun environment ->
            async {
                try
                    return! AsyncFlow.run environment flow
                with error ->
                    return! AsyncFlow.run environment (handler error)
            })

    member _.TryFinally
        (
            flow: AsyncFlow<'env, 'error, 'value>,
            compensation: unit -> unit
        ) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(fun environment ->
            async {
                try
                    return! AsyncFlow.run environment flow
                finally
                    compensation ()
            })

    member this.Using
        (
            resource: 'resource,
            binder: 'resource -> AsyncFlow<'env, 'error, 'value>
        ) : AsyncFlow<'env, 'error, 'value>
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
            body: AsyncFlow<'env, 'error, unit>
        ) : AsyncFlow<'env, 'error, unit> =
        if guard () then
            this.Bind(body, fun () -> this.While(guard, body))
        else
            this.Zero()

    member this.For
        (
            sequence: seq<'value>,
            binder: 'value -> AsyncFlow<'env, 'error, unit>
        ) : AsyncFlow<'env, 'error, unit> =
        this.Using(
            sequence.GetEnumerator(),
            fun enumerator -> this.While(enumerator.MoveNext, this.Delay(fun () -> binder enumerator.Current))
        )

[<AutoOpen>]
module Builders =
    /// <summary>
    /// The sync-only <c>flow { }</c> computation expression.
    /// </summary>
    let flow = FlowBuilder()

    /// <summary>
    /// The core <c>asyncFlow { }</c> computation expression.
    /// </summary>
    let asyncFlow = AsyncFlowBuilder()
