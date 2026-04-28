namespace FsFlow.Net

open System
open System.Threading
open System.Threading.Tasks
open FsFlow

/// <summary>
/// Represents a cold task-based workflow that depends on an environment,
/// observes a runtime cancellation token, can fail with a typed error,
/// and can succeed with a value.
/// </summary>
/// <typeparam name="env">The type of the environment dependency.</typeparam>
/// <typeparam name="error">The type of the failure value.</typeparam>
/// <typeparam name="value">The type of the success value.</typeparam>
type TaskFlow<'env, 'error, 'value> =
    private
    | TaskFlow of ('env -> CancellationToken -> Task<Result<'value, 'error>>)

/// <summary>
/// Represents delayed task work that can observe a runtime cancellation token
/// when it is started.
/// </summary>
/// <typeparam name="value">The type of the produced task value.</typeparam>
type ColdTask<'value> =
    | ColdTask of (CancellationToken -> Task<'value>)

/// <summary>
/// Core functions for creating and executing cold tasks.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module ColdTask =
    let create (operation: CancellationToken -> Task<'value>) : ColdTask<'value> =
        ColdTask operation

    let fromTaskFactory (factory: unit -> Task<'value>) : ColdTask<'value> =
        create (fun _ -> factory ())

    let fromTask (startedTask: Task<'value>) : ColdTask<'value> =
        fromTaskFactory (fun () -> startedTask)

    let fromValueTaskFactory
        (factory: CancellationToken -> ValueTask<'value>)
        : ColdTask<'value> =
        create (fun cancellationToken -> factory cancellationToken |> _.AsTask())

    let fromValueTaskFactoryWithoutCancellation
        (factory: unit -> ValueTask<'value>)
        : ColdTask<'value> =
        create (fun _ -> factory () |> _.AsTask())

    let fromValueTask (startedValueTask: ValueTask<'value>) : ColdTask<'value> =
        let startedTask = startedValueTask.AsTask()
        fromTask startedTask

    let run (cancellationToken: CancellationToken) (ColdTask operation: ColdTask<'value>) : Task<'value> =
        operation cancellationToken

/// <summary>
/// Core functions for creating, composing, and executing task flows.
/// </summary>
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<RequireQualifiedAccess>]
module TaskFlow =
    let run
        (environment: 'env)
        (cancellationToken: CancellationToken)
        (TaskFlow operation: TaskFlow<'env, 'error, 'value>)
        : Task<Result<'value, 'error>> =
        operation environment cancellationToken

    let toTask
        (environment: 'env)
        (cancellationToken: CancellationToken)
        (flow: TaskFlow<'env, 'error, 'value>)
        : Task<Result<'value, 'error>> =
        run environment cancellationToken flow

    let succeed (value: 'value) : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun _ _ -> Task.FromResult(Ok value))

    let fail (error: 'error) : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun _ _ -> Task.FromResult(Error error))

    let fromResult (result: Result<'value, 'error>) : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun _ _ -> Task.FromResult result)

    let fromOption (error: 'error) (value: 'value option) : TaskFlow<'env, 'error, 'value> =
        match value with
        | Some innerValue -> succeed innerValue
        | None -> fail error

    let fromValueOption (error: 'error) (value: 'value voption) : TaskFlow<'env, 'error, 'value> =
        match value with
        | ValueSome innerValue -> succeed innerValue
        | ValueNone -> fail error

    let fromFlow (flow: Flow<'env, 'error, 'value>) : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun environment _ -> Task.FromResult(Flow.run environment flow))

    let fromAsyncFlow (flow: AsyncFlow<'env, 'error, 'value>) : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun environment cancellationToken ->
            AsyncFlow.run environment flow
            |> fun operation -> Async.StartAsTask(operation, cancellationToken = cancellationToken))

    let fromTask (coldTask: ColdTask<'value>) : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun _ cancellationToken ->
            task {
                let! value = ColdTask.run cancellationToken coldTask
                return Ok value
            })

    let fromTaskResult
        (coldTask: ColdTask<Result<'value, 'error>>)
        : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun _ cancellationToken -> ColdTask.run cancellationToken coldTask)

    let env<'env, 'error> : TaskFlow<'env, 'error, 'env> =
        TaskFlow(fun environment _ -> Task.FromResult(Ok environment))

    let read (projection: 'env -> 'value) : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun environment _ -> Task.FromResult(Ok(projection environment)))

    let map
        (mapper: 'value -> 'next)
        (flow: TaskFlow<'env, 'error, 'value>)
        : TaskFlow<'env, 'error, 'next> =
        TaskFlow(fun environment cancellationToken ->
            InternalCombinatorCore.mapWith
                (fun mapOutcome operation ->
                    task {
                        let! result = operation
                        return mapOutcome result
                    })
                mapper
                (fun (environment, cancellationToken) -> run environment cancellationToken flow)
                (environment, cancellationToken))

    let bind
        (binder: 'value -> TaskFlow<'env, 'error, 'next>)
        (flow: TaskFlow<'env, 'error, 'value>)
        : TaskFlow<'env, 'error, 'next> =
        TaskFlow(fun environment cancellationToken ->
            InternalCombinatorCore.bindWith
                (fun operation onSuccess onError ->
                    task {
                        let! result = operation

                        match result with
                        | Ok value -> return! onSuccess value
                        | Error error -> return! onError error
                    })
                (fun (environment, cancellationToken) value -> binder value |> run environment cancellationToken)
                (Error >> Task.FromResult)
                (fun (environment, cancellationToken) -> run environment cancellationToken flow)
                (environment, cancellationToken))

    let tap
        (binder: 'value -> TaskFlow<'env, 'error, unit>)
        (flow: TaskFlow<'env, 'error, 'value>)
        : TaskFlow<'env, 'error, 'value> =
        bind
            (fun value ->
                binder value
                |> map (fun () -> value))
            flow

    let tapError
        (binder: 'error -> TaskFlow<'env, 'error, unit>)
        (flow: TaskFlow<'env, 'error, 'value>)
        : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun environment cancellationToken ->
            task {
                let! result = run environment cancellationToken flow

                match result with
                | Ok value -> return Ok value
                | Error error ->
                    let! tapResult = binder error |> run environment cancellationToken

                    match tapResult with
                    | Ok () -> return Error error
                    | Error nextError -> return Error nextError
            })

    let mapError
        (mapper: 'error -> 'nextError)
        (flow: TaskFlow<'env, 'error, 'value>)
        : TaskFlow<'env, 'nextError, 'value> =
        TaskFlow(fun environment cancellationToken ->
            InternalCombinatorCore.mapErrorWith
                (fun mapOutcome operation ->
                    task {
                        let! result = operation
                        return mapOutcome result
                    })
                mapper
                (fun (environment, cancellationToken) -> run environment cancellationToken flow)
                (environment, cancellationToken))

    let catch
        (handler: exn -> 'error)
        (flow: TaskFlow<'env, 'error, 'value>)
        : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun environment cancellationToken ->
            task {
                try
                    return! run environment cancellationToken flow
                with error ->
                    return Error(handler error)
            })

    let orElse
        (fallback: TaskFlow<'env, 'error, 'value>)
        (flow: TaskFlow<'env, 'error, 'value>)
        : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun environment cancellationToken ->
            task {
                let! result = run environment cancellationToken flow

                match result with
                | Ok value -> return Ok value
                | Error _ -> return! run environment cancellationToken fallback
            })

    let zip
        (left: TaskFlow<'env, 'error, 'left>)
        (right: TaskFlow<'env, 'error, 'right>)
        : TaskFlow<'env, 'error, 'left * 'right> =
        bind
            (fun leftValue ->
                right
                |> map (fun rightValue -> leftValue, rightValue))
            left

    let map2
        (mapper: 'left -> 'right -> 'value)
        (left: TaskFlow<'env, 'error, 'left>)
        (right: TaskFlow<'env, 'error, 'right>)
        : TaskFlow<'env, 'error, 'value> =
        zip left right
        |> map (fun (leftValue, rightValue) -> mapper leftValue rightValue)

    let localEnv
        (mapping: 'outerEnvironment -> 'innerEnvironment)
        (flow: TaskFlow<'innerEnvironment, 'error, 'value>)
        : TaskFlow<'outerEnvironment, 'error, 'value> =
        TaskFlow(fun environment cancellationToken ->
            InternalCombinatorCore.localEnvWith
                (fun (environment, cancellationToken) innerFlow -> run environment cancellationToken innerFlow)
                (fun (environment, cancellationToken) -> mapping environment, cancellationToken)
                flow
                (environment, cancellationToken))

    let delay (factory: unit -> TaskFlow<'env, 'error, 'value>) : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun environment cancellationToken ->
            InternalCombinatorCore.delayWith
                (fun (environment, cancellationToken) delayedFlow -> run environment cancellationToken delayedFlow)
                factory
                (environment, cancellationToken))

    /// <summary>
    /// Transforms a sequence of values into a task flow by applying a task flow-producing function to each element.
    /// </summary>
    let traverse
        (mapping: 'value -> TaskFlow<'env, 'error, 'next>)
        (values: seq<'value>)
        : TaskFlow<'env, 'error, 'next list> =
        TaskFlow(fun environment cancellationToken ->
            task {
                let results = ResizeArray()
                let mutable currentError = None
                use enumerator = values.GetEnumerator()

                while currentError.IsNone && enumerator.MoveNext() do
                    let! outcome = mapping enumerator.Current |> run environment cancellationToken

                    match outcome with
                    | Ok value -> results.Add value
                    | Error error -> currentError <- Some error

                match currentError with
                | Some error -> return Error error
                | None -> return Ok(List.ofSeq results)
            })

    /// <summary>
    /// Transforms a sequence of task flows into a task flow of a sequence.
    /// </summary>
    let sequence (flows: seq<TaskFlow<'env, 'error, 'value>>) : TaskFlow<'env, 'error, 'value list> =
        traverse id flows

    /// <summary>
    /// Task-native runtime helpers for operational concerns like logging, timeout, retry, and scoped cleanup.
    /// </summary>
    [<RequireQualifiedAccess>]
    module Runtime =
        let cancellationToken<'env, 'error> : TaskFlow<'env, 'error, CancellationToken> =
            TaskFlow(fun _environment cancellationToken -> Task.FromResult(Ok cancellationToken))

        let catchCancellation
            (handler: OperationCanceledException -> 'error)
            (flow: TaskFlow<'env, 'error, 'value>)
            : TaskFlow<'env, 'error, 'value> =
            TaskFlow(fun environment cancellationToken ->
                task {
                    try
                        return! run environment cancellationToken flow
                    with :? OperationCanceledException as error ->
                        return Error(handler error)
                })

        let ensureNotCanceled<'env, 'error> (canceledError: 'error) : TaskFlow<'env, 'error, unit> =
            TaskFlow(fun _environment cancellationToken ->
                if cancellationToken.IsCancellationRequested then
                    Task.FromResult(Error canceledError)
                else
                    Task.FromResult(Ok ()))

        let sleep<'env, 'error> (delay: TimeSpan) : TaskFlow<'env, 'error, unit> =
            TaskFlow(fun _environment cancellationToken ->
                task {
                    do! Task.Delay(delay, cancellationToken)
                    return Ok ()
                })

        let log
            (writer: 'env -> LogEntry -> unit)
            (level: LogLevel)
            (message: string)
            : TaskFlow<'env, 'error, unit> =
            TaskFlow(fun environment _ ->
                writer
                    environment
                    { Level = level
                      Message = message
                      TimestampUtc = DateTimeOffset.UtcNow }

                Task.FromResult(Ok ()))

        let logWith
            (writer: 'env -> LogEntry -> unit)
            (level: LogLevel)
            (messageFactory: 'env -> string)
            : TaskFlow<'env, 'error, unit> =
            TaskFlow(fun environment _ ->
                writer
                    environment
                    { Level = level
                      Message = messageFactory environment
                      TimestampUtc = DateTimeOffset.UtcNow }

                Task.FromResult(Ok ()))

        let useWithAcquireRelease
            (acquire: TaskFlow<'env, 'error, 'resource>)
            (release: 'resource -> CancellationToken -> Task)
            (useResource: 'resource -> TaskFlow<'env, 'error, 'value>)
            : TaskFlow<'env, 'error, 'value> =
            bind
                (fun resource ->
                    TaskFlow(fun environment cancellationToken ->
                        async {
                            let! result =
                                run environment cancellationToken (useResource resource)
                                |> Async.AwaitTask
                                |> Async.Catch

                            do! release resource cancellationToken |> Async.AwaitTask

                            match result with
                            | Choice1Of2 (Ok value) -> return Ok value
                            | Choice1Of2 (Error error) -> return Error error
                            | Choice2Of2 error -> return raise error
                        }
                        |> fun computation -> Async.StartAsTask(computation, cancellationToken = cancellationToken)))
                acquire

        let timeout
            (after: TimeSpan)
            (timeoutError: 'error)
            (flow: TaskFlow<'env, 'error, 'value>)
            : TaskFlow<'env, 'error, 'value> =
            TaskFlow(fun environment cancellationToken ->
                task {
                    let operation = run environment cancellationToken flow
                    let timeoutTask = Task.Delay after
                    let! completed = Task.WhenAny([| operation :> Task; timeoutTask |])

                    if obj.ReferenceEquals(completed, timeoutTask) then
                        return Error timeoutError
                    else
                        return! operation
                })

        /// <summary>
        /// Wraps a flow with a timeout. If the flow does not complete within the specified duration, returns a success value.
        /// </summary>
        let timeoutToOk
            (after: TimeSpan)
            (value: 'value)
            (flow: TaskFlow<'env, 'error, 'value>)
            : TaskFlow<'env, 'error, 'value> =
            TaskFlow(fun environment cancellationToken ->
                task {
                    let operation = run environment cancellationToken flow
                    let timeoutTask = Task.Delay after
                    let! completed = Task.WhenAny([| operation :> Task; timeoutTask |])

                    if obj.ReferenceEquals(completed, timeoutTask) then
                        return Ok value
                    else
                        return! operation
                })

        /// <summary>
        /// Transitions to a failure value on timeout.
        /// </summary>
        let timeoutToError
            (after: TimeSpan)
            (error: 'error)
            (flow: TaskFlow<'env, 'error, 'value>)
            : TaskFlow<'env, 'error, 'value> =
            timeout after error flow

        /// <summary>
        /// Transitions to a fallback workflow on timeout.
        /// </summary>
        let timeoutWith
            (after: TimeSpan)
            (fallback: unit -> TaskFlow<'env, 'error, 'value>)
            (flow: TaskFlow<'env, 'error, 'value>)
            : TaskFlow<'env, 'error, 'value> =
            TaskFlow(fun environment cancellationToken ->
                task {
                    let operation = run environment cancellationToken flow
                    let timeoutTask = Task.Delay after
                    let! completed = Task.WhenAny([| operation :> Task; timeoutTask |])

                    if obj.ReferenceEquals(completed, timeoutTask) then
                        return! run environment cancellationToken (fallback ())
                    else
                        return! operation
                })

        let retry
            (policy: RetryPolicy<'error>)
            (flow: TaskFlow<'env, 'error, 'value>)
            : TaskFlow<'env, 'error, 'value> =
            if policy.MaxAttempts < 1 then
                invalidArg (nameof policy.MaxAttempts) "RetryPolicy.MaxAttempts must be at least 1."

            let rec loop attempt =
                TaskFlow(fun environment cancellationToken ->
                    task {
                        let! result = run environment cancellationToken flow

                        match result with
                        | Ok value -> return Ok value
                        | Error error when attempt < policy.MaxAttempts && policy.ShouldRetry error ->
                            let delay = policy.Delay attempt

                            if delay > TimeSpan.Zero then
                                do! Task.Delay(delay, cancellationToken)

                            return! run environment cancellationToken (loop (attempt + 1))
                        | Error error ->
                            return Error error
                    })

            loop 1

/// <summary>
/// Computation expression builder for task-based <see cref="T:FsFlow.Net.TaskFlow`3" /> workflows.
/// </summary>
type TaskFlowBuilder() =
    member _.Return(value: 'value) : TaskFlow<'env, 'error, 'value> =
        TaskFlow.succeed value

    member _.ReturnFrom(flow: TaskFlow<'env, 'error, 'value>) : TaskFlow<'env, 'error, 'value> =
        flow

    member _.ReturnFrom(operation: Async<'value>) : TaskFlow<'env, 'error, 'value> =
        operation
        |> AsyncFlow.fromAsync
        |> TaskFlow.fromAsyncFlow

    member _.ReturnFrom(operation: Task<Result<'value, 'error>>) : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun _ _ -> operation)

    member _.ReturnFrom(operation: ValueTask<Result<'value, 'error>>) : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun _ _ -> operation.AsTask())

    member _.ReturnFrom(operation: ColdTask<Result<'value, 'error>>) : TaskFlow<'env, 'error, 'value> =
        TaskFlow.fromTaskResult operation

    member _.ReturnFrom(operation: Async<Result<'value, 'error>>) : TaskFlow<'env, 'error, 'value> =
        operation
        |> AsyncFlow.fromAsyncResult
        |> TaskFlow.fromAsyncFlow

    member _.ReturnFrom(flow: AsyncFlow<'env, 'error, 'value>) : TaskFlow<'env, 'error, 'value> =
        TaskFlow.fromAsyncFlow flow

    member _.ReturnFrom(flow: Flow<'env, 'error, 'value>) : TaskFlow<'env, 'error, 'value> =
        TaskFlow.fromFlow flow

    member _.ReturnFrom(result: Result<'value, 'error>) : TaskFlow<'env, 'error, 'value> =
        TaskFlow.fromResult result

    member _.ReturnFrom(option: 'value option) : TaskFlow<'env, unit, 'value> =
        match option with
        | Some value -> TaskFlow.succeed value
        | None -> TaskFlow.fail ()

    member _.ReturnFrom(option: 'value voption) : TaskFlow<'env, unit, 'value> =
        match option with
        | ValueSome value -> TaskFlow.succeed value
        | ValueNone -> TaskFlow.fail ()

    member _.Zero() : TaskFlow<'env, 'error, unit> =
        TaskFlow.succeed ()

    member _.Bind
        (
            flow: TaskFlow<'env, 'error, 'value>,
            binder: 'value -> TaskFlow<'env, 'error, 'next>
        ) : TaskFlow<'env, 'error, 'next> =
        TaskFlow.bind binder flow

    member _.Bind
        (
            flow: AsyncFlow<'env, 'error, 'value>,
            binder: 'value -> TaskFlow<'env, 'error, 'next>
        ) : TaskFlow<'env, 'error, 'next> =
        flow
        |> TaskFlow.fromAsyncFlow
        |> TaskFlow.bind binder

    member _.Bind
        (
            operation: Async<'value>,
            binder: 'value -> TaskFlow<'env, 'error, 'next>
        ) : TaskFlow<'env, 'error, 'next> =
        operation
        |> AsyncFlow.fromAsync
        |> TaskFlow.fromAsyncFlow
        |> TaskFlow.bind binder

    member _.Bind
        (
            operation: Async<Result<'value, 'error>>,
            binder: 'value -> TaskFlow<'env, 'error, 'next>
        ) : TaskFlow<'env, 'error, 'next> =
        operation
        |> AsyncFlow.fromAsyncResult
        |> TaskFlow.fromAsyncFlow
        |> TaskFlow.bind binder

    member _.Bind
        (
            operation: Task<Result<'value, 'error>>,
            binder: 'value -> TaskFlow<'env, 'error, 'next>
        ) : TaskFlow<'env, 'error, 'next> =
        TaskFlow(fun _ _ -> operation)
        |> TaskFlow.bind binder

    member _.Bind
        (
            operation: ValueTask<Result<'value, 'error>>,
            binder: 'value -> TaskFlow<'env, 'error, 'next>
        ) : TaskFlow<'env, 'error, 'next> =
        TaskFlow(fun _ _ -> operation.AsTask())
        |> TaskFlow.bind binder

    member _.Bind
        (
            operation: ColdTask<Result<'value, 'error>>,
            binder: 'value -> TaskFlow<'env, 'error, 'next>
        ) : TaskFlow<'env, 'error, 'next> =
        operation
        |> TaskFlow.fromTaskResult
        |> TaskFlow.bind binder

    member _.Bind
        (
            flow: Flow<'env, 'error, 'value>,
            binder: 'value -> TaskFlow<'env, 'error, 'next>
        ) : TaskFlow<'env, 'error, 'next> =
        flow
        |> TaskFlow.fromFlow
        |> TaskFlow.bind binder

    member _.Bind
        (
            result: Result<'value, 'error>,
            binder: 'value -> TaskFlow<'env, 'error, 'next>
        ) : TaskFlow<'env, 'error, 'next> =
        result
        |> TaskFlow.fromResult
        |> TaskFlow.bind binder

    member _.Bind
        (
            option: 'value option,
            binder: 'value -> TaskFlow<'env, unit, 'next>
        ) : TaskFlow<'env, unit, 'next> =
        match option with
        | Some value -> binder value
        | None -> TaskFlow.fail ()

    member _.Bind
        (
            option: 'value voption,
            binder: 'value -> TaskFlow<'env, unit, 'next>
        ) : TaskFlow<'env, unit, 'next> =
        match option with
        | ValueSome value -> binder value
        | ValueNone -> TaskFlow.fail ()

    member _.Delay(factory: unit -> TaskFlow<'env, 'error, 'value>) : TaskFlow<'env, 'error, 'value> =
        TaskFlow.delay factory

    member _.Run(flow: TaskFlow<'env, 'error, 'value>) : TaskFlow<'env, 'error, 'value> =
        flow

    member _.Combine
        (
            first: TaskFlow<'env, 'error, unit>,
            second: TaskFlow<'env, 'error, 'value>
        ) : TaskFlow<'env, 'error, 'value> =
        first
        |> TaskFlow.bind (fun () -> second)

    member _.TryWith
        (
            flow: TaskFlow<'env, 'error, 'value>,
            handler: exn -> TaskFlow<'env, 'error, 'value>
        ) : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun environment cancellationToken ->
            task {
                try
                    return! TaskFlow.run environment cancellationToken flow
                with error ->
                    return! TaskFlow.run environment cancellationToken (handler error)
            })

    member _.TryFinally
        (
            flow: TaskFlow<'env, 'error, 'value>,
            compensation: unit -> unit
        ) : TaskFlow<'env, 'error, 'value> =
        TaskFlow(fun environment cancellationToken ->
            task {
                try
                    return! TaskFlow.run environment cancellationToken flow
                finally
                    compensation ()
            })

    member this.Using
        (
            resource: 'resource,
            binder: 'resource -> TaskFlow<'env, 'error, 'value>
        ) : TaskFlow<'env, 'error, 'value>
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
            body: TaskFlow<'env, 'error, unit>
        ) : TaskFlow<'env, 'error, unit> =
        if guard () then
            this.Bind(body, fun () -> this.While(guard, body))
        else
            this.Zero()

    member this.For
        (
            sequence: seq<'value>,
            binder: 'value -> TaskFlow<'env, 'error, unit>
        ) : TaskFlow<'env, 'error, unit> =
        this.Using(
            sequence.GetEnumerator(),
            fun enumerator -> this.While(enumerator.MoveNext, this.Delay(fun () -> binder enumerator.Current))
        )

[<AutoOpen>]
module TaskFlowBuilderExtensions =
    type TaskFlowBuilder with
        member this.ReturnFrom(operation: ValueTask) : TaskFlow<'env, 'error, unit> =
            operation.AsTask()
            |> this.ReturnFrom

        member this.ReturnFrom(operation: ValueTask<'value>) : TaskFlow<'env, 'error, 'value> =
            operation.AsTask()
            |> this.ReturnFrom

        member _.ReturnFrom(operation: Task) : TaskFlow<'env, 'error, unit> =
            TaskFlow(fun _ _ ->
                task {
                    do! operation
                    return Ok()
                })

        member _.ReturnFrom(operation: Task<'value>) : TaskFlow<'env, 'error, 'value> =
            TaskFlow(fun _ _ ->
                task {
                    let! value = operation
                    return Ok value
                })

        member this.Bind
            (
                operation: Task,
                binder: unit -> TaskFlow<'env, 'error, 'next>
            ) : TaskFlow<'env, 'error, 'next> =
            operation
            |> this.ReturnFrom
            |> TaskFlow.bind binder

        member this.Bind
            (
                operation: Task<'value>,
                binder: 'value -> TaskFlow<'env, 'error, 'next>
            ) : TaskFlow<'env, 'error, 'next> =
            operation
            |> this.ReturnFrom
            |> TaskFlow.bind binder

        member this.Bind
            (
                operation: ValueTask,
                binder: unit -> TaskFlow<'env, 'error, 'next>
            ) : TaskFlow<'env, 'error, 'next> =
            operation
            |> this.ReturnFrom
            |> TaskFlow.bind binder

        member this.Bind
            (
                operation: ValueTask<'value>,
                binder: 'value -> TaskFlow<'env, 'error, 'next>
            ) : TaskFlow<'env, 'error, 'next> =
            operation
            |> this.ReturnFrom
            |> TaskFlow.bind binder

        member _.ReturnFrom(operation: ColdTask<'value>) : TaskFlow<'env, 'error, 'value> =
            TaskFlow.fromTask operation

        member this.Bind
            (
                operation: ColdTask<'value>,
                binder: 'value -> TaskFlow<'env, 'error, 'next>
            ) : TaskFlow<'env, 'error, 'next> =
            operation
            |> this.ReturnFrom
            |> TaskFlow.bind binder

type DotNetAsyncFlowBuilder() =
    inherit AsyncFlowBuilder()

    member _.ReturnFrom(operation: Task<Result<'value, 'error>>) : AsyncFlow<'env, 'error, 'value> =
        operation
        |> Async.AwaitTask
        |> AsyncFlow.fromAsyncResult

    member _.ReturnFrom(operation: ValueTask<Result<'value, 'error>>) : AsyncFlow<'env, 'error, 'value> =
        operation.AsTask()
        |> Async.AwaitTask
        |> AsyncFlow.fromAsyncResult

    member _.ReturnFrom(operation: ColdTask<Result<'value, 'error>>) : AsyncFlow<'env, 'error, 'value> =
        async {
            let! cancellationToken = Async.CancellationToken
            return! ColdTask.run cancellationToken operation |> Async.AwaitTask
        }
        |> AsyncFlow.fromAsyncResult

    member _.Bind
        (
            operation: Task<Result<'value, 'error>>,
            binder: 'value -> AsyncFlow<'env, 'error, 'next>
        ) : AsyncFlow<'env, 'error, 'next> =
        operation
        |> Async.AwaitTask
        |> AsyncFlow.fromAsyncResult
        |> AsyncFlow.bind binder

    member _.Bind
        (
            operation: ValueTask<Result<'value, 'error>>,
            binder: 'value -> AsyncFlow<'env, 'error, 'next>
        ) : AsyncFlow<'env, 'error, 'next> =
        operation.AsTask()
        |> Async.AwaitTask
        |> AsyncFlow.fromAsyncResult
        |> AsyncFlow.bind binder

    member this.Bind
        (
            operation: ColdTask<Result<'value, 'error>>,
            binder: 'value -> AsyncFlow<'env, 'error, 'next>
        ) : AsyncFlow<'env, 'error, 'next> =
        async {
            let! cancellationToken = Async.CancellationToken
            return! ColdTask.run cancellationToken operation |> Async.AwaitTask
        }
        |> AsyncFlow.fromAsyncResult
        |> AsyncFlow.bind binder

[<AutoOpen>]
module AsyncFlowBuilderExtensions =
    type AsyncFlowBuilder with
        member this.ReturnFrom(operation: ValueTask) : AsyncFlow<'env, 'error, unit> =
            operation.AsTask()
            |> this.ReturnFrom

        member this.ReturnFrom(operation: ValueTask<'value>) : AsyncFlow<'env, 'error, 'value> =
            operation.AsTask()
            |> this.ReturnFrom

        member _.ReturnFrom(operation: Task) : AsyncFlow<'env, 'error, unit> =
            operation
            |> Async.AwaitTask
            |> AsyncFlow.fromAsync

        member _.ReturnFrom(operation: Task<'value>) : AsyncFlow<'env, 'error, 'value> =
            operation
            |> Async.AwaitTask
            |> AsyncFlow.fromAsync

        member this.Bind
            (
                operation: Task,
                binder: unit -> AsyncFlow<'env, 'error, 'next>
            ) : AsyncFlow<'env, 'error, 'next> =
            operation
            |> this.ReturnFrom
            |> AsyncFlow.bind binder

        member this.Bind
            (
                operation: Task<'value>,
                binder: 'value -> AsyncFlow<'env, 'error, 'next>
            ) : AsyncFlow<'env, 'error, 'next> =
            operation
            |> this.ReturnFrom
            |> AsyncFlow.bind binder

        member this.Bind
            (
                operation: ValueTask,
                binder: unit -> AsyncFlow<'env, 'error, 'next>
            ) : AsyncFlow<'env, 'error, 'next> =
            operation
            |> this.ReturnFrom
            |> AsyncFlow.bind binder

        member this.Bind
            (
                operation: ValueTask<'value>,
                binder: 'value -> AsyncFlow<'env, 'error, 'next>
            ) : AsyncFlow<'env, 'error, 'next> =
            operation
            |> this.ReturnFrom
            |> AsyncFlow.bind binder

        member _.ReturnFrom(operation: ColdTask<'value>) : AsyncFlow<'env, 'error, 'value> =
            async {
                let! cancellationToken = Async.CancellationToken
                let! value = ColdTask.run cancellationToken operation |> Async.AwaitTask
                return value
            }
            |> AsyncFlow.fromAsync

        member this.Bind
            (
                operation: ColdTask<'value>,
                binder: 'value -> AsyncFlow<'env, 'error, 'next>
            ) : AsyncFlow<'env, 'error, 'next> =
            operation
            |> this.ReturnFrom
            |> AsyncFlow.bind binder

[<AutoOpen>]
module Builders =
    /// <summary>
    /// The .NET-extended <c>asyncFlow { }</c> computation expression.
    /// </summary>
    let asyncFlow = DotNetAsyncFlowBuilder()

    /// <summary>
    /// The .NET <c>taskFlow { }</c> computation expression.
    /// </summary>
    let taskFlow = TaskFlowBuilder()
