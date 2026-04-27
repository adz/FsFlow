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

    let env<'env, 'error> : Flow<'env, 'error, 'env> =
        Flow(fun environment -> Ok environment)

    let read (projection: 'env -> 'value) : Flow<'env, 'error, 'value> =
        Flow(fun environment -> Ok(projection environment))

    let map
        (mapper: 'value -> 'next)
        (flow: Flow<'env, 'error, 'value>)
        : Flow<'env, 'error, 'next> =
        Flow(fun environment -> flow |> run environment |> ResultFlow.map mapper)

    let bind
        (binder: 'value -> Flow<'env, 'error, 'next>)
        (flow: Flow<'env, 'error, 'value>)
        : Flow<'env, 'error, 'next> =
        Flow(fun environment ->
            flow
            |> run environment
            |> ResultFlow.bind (fun value -> binder value |> run environment))

    let tap
        (binder: 'value -> Flow<'env, 'error, unit>)
        (flow: Flow<'env, 'error, 'value>)
        : Flow<'env, 'error, 'value> =
        bind
            (fun value ->
                binder value
                |> map (fun () -> value))
            flow

    let mapError
        (mapper: 'error -> 'nextError)
        (flow: Flow<'env, 'error, 'value>)
        : Flow<'env, 'nextError, 'value> =
        Flow(fun environment -> flow |> run environment |> ResultFlow.mapError mapper)

    let catch
        (handler: exn -> 'error)
        (flow: Flow<'env, 'error, 'value>)
        : Flow<'env, 'error, 'value> =
        Flow(fun environment ->
            try
                run environment flow
            with error ->
                Error(handler error))

    let localEnv
        (mapping: 'outerEnvironment -> 'innerEnvironment)
        (flow: Flow<'innerEnvironment, 'error, 'value>)
        : Flow<'outerEnvironment, 'error, 'value> =
        Flow(fun environment -> flow |> run (mapping environment))

    let delay (factory: unit -> Flow<'env, 'error, 'value>) : Flow<'env, 'error, 'value> =
        Flow(fun environment -> factory () |> run environment)

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
        AsyncFlow(fun environment ->
            async {
                let! result = run environment flow
                return ResultFlow.map mapper result
            })

    let bind
        (binder: 'value -> AsyncFlow<'env, 'error, 'next>)
        (flow: AsyncFlow<'env, 'error, 'value>)
        : AsyncFlow<'env, 'error, 'next> =
        AsyncFlow(fun environment ->
            async {
                let! result = run environment flow

                match result with
                | Ok value -> return! binder value |> run environment
                | Error error -> return Error error
            })

    let tap
        (binder: 'value -> AsyncFlow<'env, 'error, unit>)
        (flow: AsyncFlow<'env, 'error, 'value>)
        : AsyncFlow<'env, 'error, 'value> =
        bind
            (fun value ->
                binder value
                |> map (fun () -> value))
            flow

    let mapError
        (mapper: 'error -> 'nextError)
        (flow: AsyncFlow<'env, 'error, 'value>)
        : AsyncFlow<'env, 'nextError, 'value> =
        AsyncFlow(fun environment ->
            async {
                let! result = run environment flow
                return ResultFlow.mapError mapper result
            })

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

    let localEnv
        (mapping: 'outerEnvironment -> 'innerEnvironment)
        (flow: AsyncFlow<'innerEnvironment, 'error, 'value>)
        : AsyncFlow<'outerEnvironment, 'error, 'value> =
        AsyncFlow(fun environment -> flow |> run (mapping environment))

    let delay (factory: unit -> AsyncFlow<'env, 'error, 'value>) : AsyncFlow<'env, 'error, 'value> =
        AsyncFlow(fun environment -> factory () |> run environment)
