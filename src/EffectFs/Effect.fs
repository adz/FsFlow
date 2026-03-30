namespace EffectFs

open System
open System.Threading
open System.Threading.Tasks

type Effect<'env, 'error, 'value> =
    private
    | Effect of ('env -> CancellationToken -> Async<Result<'value, 'error>>)

[<RequireQualifiedAccess>]
type LogLevel =
    | Trace
    | Debug
    | Information
    | Warning
    | Error
    | Critical

type LogEntry =
    { Level: LogLevel
      Message: string
      TimestampUtc: DateTimeOffset }

type RetryPolicy<'error> =
    { MaxAttempts: int
      Delay: int -> TimeSpan
      ShouldRetry: 'error -> bool }

[<RequireQualifiedAccess>]
module RetryPolicy =
    let noDelay (maxAttempts: int) : RetryPolicy<'error> =
        { MaxAttempts = maxAttempts
          Delay = fun _ -> TimeSpan.Zero
          ShouldRetry = fun _ -> true }

[<RequireQualifiedAccess>]
module Effect =
    let run
        (environment: 'env)
        (cancellationToken: CancellationToken)
        (Effect operation: Effect<'env, 'error, 'value>)
        : Async<Result<'value, 'error>> =
        operation environment cancellationToken

    let succeed (value: 'value) : Effect<'env, 'error, 'value> =
        Effect(fun _ _ -> async.Return(Ok value))

    let fail (error: 'error) : Effect<'env, 'error, 'value> =
        Effect(fun _ _ -> async.Return(Error error))

    let fromResult (result: Result<'value, 'error>) : Effect<'env, 'error, 'value> =
        Effect(fun _ _ -> async.Return result)

    let ofResult (result: Result<'value, 'error>) : Effect<'env, 'error, 'value> =
        fromResult result

    let fromAsync (operation: Async<'value>) : Effect<'env, 'error, 'value> =
        Effect(fun _ _ ->
            async {
                let! value = operation
                return Ok value
            })

    let ofAsync (operation: Async<'value>) : Effect<'env, 'error, 'value> =
        fromAsync operation

    let fromAsyncResult (operation: Async<Result<'value, 'error>>) : Effect<'env, 'error, 'value> =
        Effect(fun _ _ -> operation)

    let fromTaskResult
        (factory: CancellationToken -> Task<Result<'value, 'error>>)
        : Effect<'env, 'error, 'value> =
        Effect(fun _ cancellationToken ->
            async {
                return! factory cancellationToken |> Async.AwaitTask
            })

    let fromTask (factory: CancellationToken -> Task<'value>) : Effect<'env, 'error, 'value> =
        Effect(fun _ cancellationToken ->
            async {
                let! value = factory cancellationToken |> Async.AwaitTask
                return Ok value
            })

    let ofTask (factory: CancellationToken -> Task<'value>) : Effect<'env, 'error, 'value> =
        fromTask factory

    let fromTaskValue (task: Task<'value>) : Effect<'env, 'error, 'value> =
        fromTask (fun _ -> task)

    let fromTaskResultValue (task: Task<Result<'value, 'error>>) : Effect<'env, 'error, 'value> =
        fromTaskResult (fun _ -> task)

    let fromTaskUnit (task: Task) : Effect<'env, 'error, unit> =
        Effect(fun _ _ ->
            async {
                do! task |> Async.AwaitTask
                return Ok ()
            })

    let ask<'env, 'error> : Effect<'env, 'error, 'env> =
        Effect(fun environment _ -> async.Return(Ok environment))

    let environment<'env, 'error> : Effect<'env, 'error, 'env> =
        ask<'env, 'error>

    let read (projection: 'env -> 'value) : Effect<'env, 'error, 'value> =
        Effect(fun environment _ -> async.Return(Ok(projection environment)))

    let cancellationToken<'env, 'error> : Effect<'env, 'error, CancellationToken> =
        Effect(fun _ token -> async.Return(Ok token))

    let map
        (mapper: 'value -> 'next)
        (effect: Effect<'env, 'error, 'value>)
        : Effect<'env, 'error, 'next> =
        Effect(fun environment cancellationToken ->
            async {
                let! result = run environment cancellationToken effect
                return Result.map mapper result
            })

    let bind
        (binder: 'value -> Effect<'env, 'error, 'next>)
        (effect: Effect<'env, 'error, 'value>)
        : Effect<'env, 'error, 'next> =
        Effect(fun environment cancellationToken ->
            async {
                let! result = run environment cancellationToken effect

                match result with
                | Ok value -> return! run environment cancellationToken (binder value)
                | Error error -> return Error error
            })

    let tap
        (binder: 'value -> Effect<'env, 'error, unit>)
        (effect: Effect<'env, 'error, 'value>)
        : Effect<'env, 'error, 'value> =
        bind
            (fun value ->
                binder value
                |> map (fun () -> value))
            effect

    let mapError
        (mapper: 'error -> 'nextError)
        (effect: Effect<'env, 'error, 'value>)
        : Effect<'env, 'nextError, 'value> =
        Effect(fun environment cancellationToken ->
            async {
                let! result = run environment cancellationToken effect
                return Result.mapError mapper result
            })

    let catch
        (handler: exn -> 'error)
        (effect: Effect<'env, 'error, 'value>)
        : Effect<'env, 'error, 'value> =
        Effect(fun environment cancellationToken ->
            async {
                try
                    return! run environment cancellationToken effect
                with error ->
                    return Error(handler error)
            })

    let catchCancellation
        (handler: OperationCanceledException -> 'error)
        (effect: Effect<'env, 'error, 'value>)
        : Effect<'env, 'error, 'value> =
        Effect(fun environment cancellationToken ->
            async {
                try
                    return! run environment cancellationToken effect
                with :? OperationCanceledException as error ->
                    return Error(handler error)
            })

    let provide (environment: 'env) (effect: Effect<'env, 'error, 'value>) : Effect<unit, 'error, 'value> =
        Effect(fun () cancellationToken -> run environment cancellationToken effect)

    let withEnvironment
        (mapping: 'outerEnvironment -> 'innerEnvironment)
        (effect: Effect<'innerEnvironment, 'error, 'value>)
        : Effect<'outerEnvironment, 'error, 'value> =
        Effect(fun environment cancellationToken ->
            environment
            |> mapping
            |> fun innerEnvironment -> run innerEnvironment cancellationToken effect)

    let sleep (delay: TimeSpan) : Effect<'env, 'error, unit> =
        Effect(fun _ cancellationToken ->
            async {
                do! Task.Delay(delay, cancellationToken) |> Async.AwaitTask
                return Ok ()
            })

    let ensureNotCanceled (canceledError: 'error) : Effect<'env, 'error, unit> =
        Effect(fun _ cancellationToken ->
            async {
                if cancellationToken.IsCancellationRequested then
                    return Error canceledError
                else
                    return Ok ()
            })

    let log
        (writer: 'env -> LogEntry -> unit)
        (level: LogLevel)
        (message: string)
        : Effect<'env, 'error, unit> =
        Effect(fun environment _ ->
            async {
                writer
                    environment
                    { Level = level
                      Message = message
                      TimestampUtc = DateTimeOffset.UtcNow }

                return Ok ()
            })

    let logWith
        (writer: 'env -> LogEntry -> unit)
        (level: LogLevel)
        (messageFactory: 'env -> string)
        : Effect<'env, 'error, unit> =
        Effect(fun environment _ ->
            async {
                writer
                    environment
                    { Level = level
                      Message = messageFactory environment
                      TimestampUtc = DateTimeOffset.UtcNow }

                return Ok ()
            })

    let tryFinally
        (compensation: unit -> unit)
        (effect: Effect<'env, 'error, 'value>)
        : Effect<'env, 'error, 'value> =
        Effect(fun environment cancellationToken ->
            async {
                try
                    return! run environment cancellationToken effect
                finally
                    compensation ()
            })

    let bracket
        (acquire: Effect<'env, 'error, 'resource>)
        (release: 'resource -> unit)
        (useResource: 'resource -> Effect<'env, 'error, 'value>)
        : Effect<'env, 'error, 'value> =
        bind
            (fun resource ->
                useResource resource
                |> tryFinally (fun () -> release resource))
            acquire

    let bracketAsync
        (acquire: Effect<'env, 'error, 'resource>)
        (release: 'resource -> CancellationToken -> Task)
        (useResource: 'resource -> Effect<'env, 'error, 'value>)
        : Effect<'env, 'error, 'value> =
        bind
            (fun resource ->
                Effect(fun environment cancellationToken ->
                    async {
                        let! result =
                            run environment cancellationToken (useResource resource)
                            |> Async.Catch

                        do! release resource cancellationToken |> Async.AwaitTask

                        match result with
                        | Choice1Of2 value -> return value
                        | Choice2Of2 error -> return raise error
                    }))
            acquire

    let usingAsync
        (resource: 'resource)
        (useResource: 'resource -> Effect<'env, 'error, 'value>)
        : Effect<'env, 'error, 'value> when 'resource :> IAsyncDisposable =
        bracketAsync
            (succeed resource)
            (fun acquired _ -> acquired.DisposeAsync().AsTask())
            useResource

    let timeout
        (after: TimeSpan)
        (timeoutError: 'error)
        (effect: Effect<'env, 'error, 'value>)
        : Effect<'env, 'error, 'value> =
        Effect(fun environment cancellationToken ->
            async {
                try
                    let! child =
                        Async.StartChild(
                            run environment cancellationToken effect,
                            millisecondsTimeout = int after.TotalMilliseconds
                        )

                    return! child
                with :? TimeoutException ->
                    return Error timeoutError
            })

    let retry
        (policy: RetryPolicy<'error>)
        (effect: Effect<'env, 'error, 'value>)
        : Effect<'env, 'error, 'value> =
        let rec loop attempt =
            Effect(fun environment cancellationToken ->
                async {
                    let! result = run environment cancellationToken effect

                    match result with
                    | Ok value -> return Ok value
                    | Error error when attempt < policy.MaxAttempts && policy.ShouldRetry error ->
                        let delay = policy.Delay attempt

                        if delay > TimeSpan.Zero then
                            do! Task.Delay(delay, cancellationToken) |> Async.AwaitTask

                        return! run environment cancellationToken (loop (attempt + 1))
                    | Error error ->
                        return Error error
                })

        if policy.MaxAttempts < 1 then
            invalidArg (nameof policy.MaxAttempts) "RetryPolicy.MaxAttempts must be at least 1."

        loop 1

    let delay (factory: unit -> Effect<'env, 'error, 'value>) : Effect<'env, 'error, 'value> =
        Effect(fun environment cancellationToken ->
            run environment cancellationToken (factory ()))

    let execute (environment: 'env) (effect: Effect<'env, 'error, 'value>) : Async<Result<'value, 'error>> =
        run environment CancellationToken.None effect

    let executeWithCancellation
        (environment: 'env)
        (cancellationToken: CancellationToken)
        (effect: Effect<'env, 'error, 'value>)
        : Async<Result<'value, 'error>> =
        run environment cancellationToken effect

    let toAsyncResult (environment: 'env) (effect: Effect<'env, 'error, 'value>) : Async<Result<'value, 'error>> =
        execute environment effect

type EffectBuilder() =
    member _.Return(value: 'value) : Effect<'env, 'error, 'value> =
        Effect.succeed value

    member _.ReturnFrom(effect: Effect<'env, 'error, 'value>) : Effect<'env, 'error, 'value> =
        effect

    member _.ReturnFrom(result: Result<'value, 'error>) : Effect<'env, 'error, 'value> =
        Effect.fromResult result

    member _.ReturnFrom(operation: Async<'value>) : Effect<'env, 'error, 'value> =
        Effect.fromAsync operation

    member _.ReturnFrom(operation: Async<Result<'value, 'error>>) : Effect<'env, 'error, 'value> =
        Effect.fromAsyncResult operation

    member _.ReturnFrom(task: Task<Result<'value, 'error>>) : Effect<'env, 'error, 'value> =
        Effect.fromTaskResultValue task

    member _.ReturnFrom(task: Task<'value>) : Effect<'env, 'error, 'value> =
        Effect.fromTaskValue task

    member _.ReturnFrom(task: Task) : Effect<'env, 'error, unit> =
        Effect.fromTaskUnit task

    member _.Bind
        (
            effect: Effect<'env, 'error, 'value>,
            binder: 'value -> Effect<'env, 'error, 'next>
        ) : Effect<'env, 'error, 'next> =
        Effect.bind binder effect

    member _.Bind
        (
            result: Result<'value, 'error>,
            binder: 'value -> Effect<'env, 'error, 'next>
        ) : Effect<'env, 'error, 'next> =
        Effect.bind binder (Effect.fromResult result)

    member _.Bind
        (
            operation: Async<'value>,
            binder: 'value -> Effect<'env, 'error, 'next>
        ) : Effect<'env, 'error, 'next> =
        Effect.bind binder (Effect.fromAsync operation)

    member _.Bind
        (
            operation: Async<Result<'value, 'error>>,
            binder: 'value -> Effect<'env, 'error, 'next>
        ) : Effect<'env, 'error, 'next> =
        Effect.bind binder (Effect.fromAsyncResult operation)

    member _.Bind
        (
            task: Task<Result<'value, 'error>>,
            binder: 'value -> Effect<'env, 'error, 'next>
        ) : Effect<'env, 'error, 'next> =
        Effect.bind binder (Effect.fromTaskResultValue task)

    member _.Bind
        (
            task: Task<'value>,
            binder: 'value -> Effect<'env, 'error, 'next>
        ) : Effect<'env, 'error, 'next> =
        Effect.bind binder (Effect.fromTaskValue task)

    member _.Bind
        (
            task: Task,
            binder: unit -> Effect<'env, 'error, 'next>
        ) : Effect<'env, 'error, 'next> =
        Effect.bind binder (Effect.fromTaskUnit task)

    member _.Zero() : Effect<'env, 'error, unit> =
        Effect.succeed ()

    member _.Delay(factory: unit -> Effect<'env, 'error, 'value>) : Effect<'env, 'error, 'value> =
        Effect.delay factory

    member _.Combine
        (
            left: Effect<'env, 'error, unit>,
            right: Effect<'env, 'error, 'value>
        ) : Effect<'env, 'error, 'value> =
        Effect.bind (fun () -> right) left

    member _.TryWith
        (
            effect: Effect<'env, 'error, 'value>,
            handler: exn -> 'error
        ) : Effect<'env, 'error, 'value> =
        Effect.catch handler effect

    member _.TryFinally
        (
            effect: Effect<'env, 'error, 'value>,
            compensation: unit -> unit
        ) : Effect<'env, 'error, 'value> =
        Effect.tryFinally compensation effect

    member _.Using
        (
            resource: 'resource,
            binder: 'resource -> Effect<'env, 'error, 'value>
        ) : Effect<'env, 'error, 'value> when 'resource :> IDisposable =
        Effect.tryFinally
            (fun () ->
                if not (isNull (box resource)) then
                    resource.Dispose())
            (binder resource)

    member this.While
        (
            guard: unit -> bool,
            body: Effect<'env, 'error, unit>
        ) : Effect<'env, 'error, unit> =
        if guard () then
            this.Bind(body, fun () -> this.While(guard, body))
        else
            this.Zero()

    member this.For
        (
            sequence: seq<'value>,
            binder: 'value -> Effect<'env, 'error, unit>
        ) : Effect<'env, 'error, unit> =
        let values = Seq.toArray sequence
        let mutable index = 0

        this.While(
            (fun () -> index < values.Length),
            this.Delay(fun () ->
                let value = values[index]
                index <- index + 1
                binder value)
        )

[<AutoOpen>]
module EffectBuilderModule =
    let effect = EffectBuilder()
