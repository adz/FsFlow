module FsFlow.Benchmarks

open System
open System.Diagnostics
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open FsFlow.Net

type CandidateValueTaskFlow<'env, 'error, 'value> =
    private
    | CandidateValueTaskFlow of ('env -> CancellationToken -> ValueTask<Result<'value, 'error>>)

[<RequireQualifiedAccess>]
module CandidateValueTaskFlow =
    let run
        (environment: 'env)
        (cancellationToken: CancellationToken)
        (CandidateValueTaskFlow operation: CandidateValueTaskFlow<'env, 'error, 'value>)
        : ValueTask<Result<'value, 'error>> =
        operation environment cancellationToken

    let succeed (value: 'value) : CandidateValueTaskFlow<'env, 'error, 'value> =
        CandidateValueTaskFlow(fun _ _ -> ValueTask<Result<'value, 'error>>(Ok value))

    let map
        (mapper: 'value -> 'next)
        (flow: CandidateValueTaskFlow<'env, 'error, 'value>)
        : CandidateValueTaskFlow<'env, 'error, 'next> =
        CandidateValueTaskFlow(fun environment cancellationToken ->
            let operation = run environment cancellationToken flow

            if operation.IsCompletedSuccessfully then
                match operation.Result with
                | Ok value -> ValueTask<Result<'next, 'error>>(Ok(mapper value))
                | Error error -> ValueTask<Result<'next, 'error>>(Error error)
            else
                ValueTask<Result<'next, 'error>>(
                    task {
                        let! result = operation.AsTask()

                        return
                            match result with
                            | Ok value -> Ok(mapper value)
                            | Error error -> Error error
                    }
                ))

    let bind
        (binder: 'value -> CandidateValueTaskFlow<'env, 'error, 'next>)
        (flow: CandidateValueTaskFlow<'env, 'error, 'value>)
        : CandidateValueTaskFlow<'env, 'error, 'next> =
        CandidateValueTaskFlow(fun environment cancellationToken ->
            let operation = run environment cancellationToken flow

            if operation.IsCompletedSuccessfully then
                match operation.Result with
                | Ok value -> binder value |> run environment cancellationToken
                | Error error -> ValueTask<Result<'next, 'error>>(Error error)
            else
                ValueTask<Result<'next, 'error>>(
                    task {
                        let! result = operation.AsTask()

                        match result with
                        | Ok value ->
                            return! binder value |> run environment cancellationToken |> _.AsTask()
                        | Error error -> return Error error
                    }
                ))

type CandidateValueTaskFlowBuilder() =
    member _.Return(value: 'value) : CandidateValueTaskFlow<'env, 'error, 'value> =
        CandidateValueTaskFlow.succeed value

    member _.ReturnFrom(flow: CandidateValueTaskFlow<'env, 'error, 'value>) : CandidateValueTaskFlow<'env, 'error, 'value> =
        flow

    member _.Bind
        (
            flow: CandidateValueTaskFlow<'env, 'error, 'value>,
            binder: 'value -> CandidateValueTaskFlow<'env, 'error, 'next>
        ) : CandidateValueTaskFlow<'env, 'error, 'next> =
        CandidateValueTaskFlow.bind binder flow

let candidateValueTaskFlow = CandidateValueTaskFlowBuilder()

type Measurement =
    { Name: string
      Iterations: int
      MedianNanosecondsPerOperation: float
      MedianBytesPerOperation: float }

[<MethodImpl(MethodImplOptions.NoInlining)>]
let private consumeResult (result: Result<int, string>) =
    match result with
    | Ok value -> value
    | Error error -> error.Length

[<MethodImpl(MethodImplOptions.NoInlining)>]
let private runTaskLoop
    (iterations: int)
    (workflow: TaskFlow<int, string, int>)
    : int =
    let mutable total = 0

    for index in 1 .. iterations do
        total <-
            total
            + (workflow
               |> TaskFlow.run index CancellationToken.None
               |> fun task -> task.GetAwaiter().GetResult()
               |> consumeResult)

    total

[<MethodImpl(MethodImplOptions.NoInlining)>]
let private runValueTaskLoop
    (iterations: int)
    (workflow: CandidateValueTaskFlow<int, string, int>)
    : int =
    let mutable total = 0

    for index in 1 .. iterations do
        total <-
            total
            + (workflow
               |> CandidateValueTaskFlow.run index CancellationToken.None
               |> fun valueTask -> valueTask.GetAwaiter().GetResult()
               |> consumeResult)

    total

let private median (values: float array) =
    let sorted = Array.copy values
    Array.sortInPlace sorted

    if sorted.Length % 2 = 1 then
        sorted[sorted.Length / 2]
    else
        let upperIndex = sorted.Length / 2
        (sorted[upperIndex - 1] + sorted[upperIndex]) / 2.0

let private collectMeasurements
    (name: string)
    (iterations: int)
    (warmup: unit -> int)
    (measure: unit -> int)
    : Measurement =
    let mutable sink = 0

    for _ in 1 .. 2 do
        sink <- sink + warmup ()

    GC.KeepAlive sink

    let elapsedTicks = Array.zeroCreate<float> 5
    let allocatedBytes = Array.zeroCreate<float> 5

    for repeat in 0 .. 4 do
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()

        let beforeBytes = GC.GetAllocatedBytesForCurrentThread()
        let stopwatch = Stopwatch.StartNew()
        sink <- sink + measure ()
        stopwatch.Stop()
        let afterBytes = GC.GetAllocatedBytesForCurrentThread()

        elapsedTicks[repeat] <- float stopwatch.ElapsedTicks
        allocatedBytes[repeat] <- float (afterBytes - beforeBytes)

    GC.KeepAlive sink

    { Name = name
      Iterations = iterations
      MedianNanosecondsPerOperation =
        median elapsedTicks
        * 1_000_000_000.0
        / float Stopwatch.Frequency
        / float iterations
      MedianBytesPerOperation = median allocatedBytes / float iterations }

[<EntryPoint>]
let main _ =
    let iterations = 2_000_000

    let taskSucceed =
        TaskFlow.succeed 42

    let valueTaskSucceed =
        CandidateValueTaskFlow.succeed 42

    let taskMapChain =
        TaskFlow.succeed 1
        |> TaskFlow.map ((+) 1)
        |> TaskFlow.map ((*) 2)
        |> TaskFlow.map ((+) 3)
        |> TaskFlow.map ((*) 4)

    let valueTaskMapChain =
        CandidateValueTaskFlow.succeed 1
        |> CandidateValueTaskFlow.map ((+) 1)
        |> CandidateValueTaskFlow.map ((*) 2)
        |> CandidateValueTaskFlow.map ((+) 3)
        |> CandidateValueTaskFlow.map ((*) 4)

    let taskBindChain =
        TaskFlow.succeed 1
        |> TaskFlow.bind (fun value -> TaskFlow.succeed(value + 1))
        |> TaskFlow.bind (fun value -> TaskFlow.succeed(value * 2))
        |> TaskFlow.bind (fun value -> TaskFlow.succeed(value + 3))
        |> TaskFlow.bind (fun value -> TaskFlow.succeed(value * 4))

    let valueTaskBindChain =
        CandidateValueTaskFlow.succeed 1
        |> CandidateValueTaskFlow.bind (fun value -> CandidateValueTaskFlow.succeed(value + 1))
        |> CandidateValueTaskFlow.bind (fun value -> CandidateValueTaskFlow.succeed(value * 2))
        |> CandidateValueTaskFlow.bind (fun value -> CandidateValueTaskFlow.succeed(value + 3))
        |> CandidateValueTaskFlow.bind (fun value -> CandidateValueTaskFlow.succeed(value * 4))

    let taskComputationExpression =
        taskFlow {
            let! start = TaskFlow.succeed 1
            let! next = TaskFlow.succeed(start + 1)
            let! third = TaskFlow.succeed(next * 2)
            return third + 5
        }

    let valueTaskComputationExpression =
        candidateValueTaskFlow {
            let! start = CandidateValueTaskFlow.succeed 1
            let! next = CandidateValueTaskFlow.succeed(start + 1)
            let! third = CandidateValueTaskFlow.succeed(next * 2)
            return third + 5
        }

    let measurements =
        [| collectMeasurements
               "TaskFlow run/succeed"
               iterations
               (fun () -> runTaskLoop 200_000 taskSucceed)
               (fun () -> runTaskLoop iterations taskSucceed)
           collectMeasurements
               "Candidate ValueTaskFlow run/succeed"
               iterations
               (fun () -> runValueTaskLoop 200_000 valueTaskSucceed)
               (fun () -> runValueTaskLoop iterations valueTaskSucceed)
           collectMeasurements
               "TaskFlow map chain"
               iterations
               (fun () -> runTaskLoop 200_000 taskMapChain)
               (fun () -> runTaskLoop iterations taskMapChain)
           collectMeasurements
               "Candidate ValueTaskFlow map chain"
               iterations
               (fun () -> runValueTaskLoop 200_000 valueTaskMapChain)
               (fun () -> runValueTaskLoop iterations valueTaskMapChain)
           collectMeasurements
               "TaskFlow bind chain"
               iterations
               (fun () -> runTaskLoop 200_000 taskBindChain)
               (fun () -> runTaskLoop iterations taskBindChain)
           collectMeasurements
               "Candidate ValueTaskFlow bind chain"
               iterations
               (fun () -> runValueTaskLoop 200_000 valueTaskBindChain)
               (fun () -> runValueTaskLoop iterations valueTaskBindChain)
           collectMeasurements
               "TaskFlow CE chain"
               iterations
               (fun () -> runTaskLoop 200_000 taskComputationExpression)
               (fun () -> runTaskLoop iterations taskComputationExpression)
           collectMeasurements
               "Candidate ValueTaskFlow CE chain"
               iterations
               (fun () -> runValueTaskLoop 200_000 valueTaskComputationExpression)
               (fun () -> runValueTaskLoop iterations valueTaskComputationExpression) |]

    printfn "FsFlow task-backbone benchmark"
    printfn "Runtime: %s" System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription
    printfn "Iterations per measurement: %d" iterations
    printfn ""
    printfn "%-34s %14s %12s" "Scenario" "ns/op" "B/op"

    for measurement in measurements do
        printfn
            "%-34s %14.2f %12.2f"
            measurement.Name
            measurement.MedianNanosecondsPerOperation
            measurement.MedianBytesPerOperation

    0
