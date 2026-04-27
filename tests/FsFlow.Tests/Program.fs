namespace FsFlow.Tests

open System
open System.Threading
open System.Threading.Tasks
open FsFlow
open FsFlow.Net
open Swensen.Unquote
open Xunit

module Tests =
    let private publicInstanceMethodNames (targetType: Type) =
        targetType.GetMethods()
        |> Array.filter (fun methodInfo -> methodInfo.IsPublic && not methodInfo.IsSpecialName)
        |> Array.map _.Name
        |> Array.distinct
        |> Array.sort

    let private flowBuilderBindAndReturnFromArgumentNames () =
        typeof<FlowBuilder>.GetMethods()
        |> Array.filter (fun methodInfo ->
            methodInfo.IsPublic
            && not methodInfo.IsSpecialName
            && (methodInfo.Name = "Bind" || methodInfo.Name = "ReturnFrom"))
        |> Array.collect (fun methodInfo -> methodInfo.GetParameters())
        |> Array.map (fun parameterInfo -> parameterInfo.ParameterType.Name)
        |> Array.distinct
        |> Array.sort

    [<Fact>]
    let ``Flow is sync result only`` () =
        let workflow : Flow<int, string, int> =
            Flow.env
            |> Flow.bind (fun value -> Flow.succeed(value * 2))

        test <@ Flow.run 21 workflow = Ok 42 @>

    [<Fact>]
    let ``Flow delay reruns from scratch`` () =
        let runs = ref 0

        let workflow : Flow<unit, string, int> =
            Flow.delay(fun () ->
                runs.Value <- runs.Value + 1
                Flow.succeed runs.Value)

        test <@ Flow.run () workflow = Ok 1 @>
        test <@ Flow.run () workflow = Ok 2 @>

    [<Fact>]
    let ``AsyncFlow runs as Async result`` () =
        let workflow : AsyncFlow<int, string, int> =
            AsyncFlow.read id
            |> AsyncFlow.bind (fun value ->
                AsyncFlow.fromAsync(async { return value * 2 }))

        let result =
            workflow
            |> AsyncFlow.run 21
            |> Async.RunSynchronously

        test <@ result = Ok 42 @>

    [<Fact>]
    let ``AsyncFlow can lift Flow`` () =
        let syncWorkflow : Flow<string, string, int> =
            Flow.read String.length

        let asyncWorkflow : AsyncFlow<string, string, int> =
            syncWorkflow
            |> AsyncFlow.fromFlow
            |> AsyncFlow.map ((+) 1)

        let result =
            asyncWorkflow
            |> AsyncFlow.toAsync "effect"
            |> Async.RunSynchronously

        test <@ result = Ok 7 @>

    [<Fact>]
    let ``TaskFlow observes the runtime cancellation token`` () =
        let seen = ref CancellationToken.None
        use cts = new CancellationTokenSource()

        let workflow : TaskFlow<unit, string, int> =
            TaskFlow.fromTask(fun cancellationToken ->
                seen.Value <- cancellationToken
                Task.FromResult 42)

        let result =
            workflow
            |> TaskFlow.run () cts.Token
            |> fun task -> task.GetAwaiter().GetResult()

        test <@ result = Ok 42 @>
        test <@ seen.Value = cts.Token @>

    [<Fact>]
    let ``TaskFlow can lift AsyncFlow`` () =
        let asyncWorkflow : AsyncFlow<int, string, int> =
            AsyncFlow.read id
            |> AsyncFlow.map (fun value -> value + 2)

        let taskWorkflow : TaskFlow<int, string, int> =
            asyncWorkflow
            |> TaskFlow.fromAsyncFlow
            |> TaskFlow.map (fun value -> value * 10)

        let result =
            taskWorkflow
            |> TaskFlow.toTask 4 CancellationToken.None
            |> fun task -> task.GetAwaiter().GetResult()

        test <@ result = Ok 60 @>

    [<Fact>]
    let ``TaskFlow delay reruns from scratch`` () =
        let runs = ref 0

        let workflow : TaskFlow<unit, string, int> =
            TaskFlow.delay(fun () ->
                runs.Value <- runs.Value + 1
                TaskFlow.succeed runs.Value)

        let runOnce () =
            workflow
            |> TaskFlow.run () CancellationToken.None
            |> fun task -> task.GetAwaiter().GetResult()

        test <@ runOnce () = Ok 1 @>
        test <@ runOnce () = Ok 2 @>

    [<Fact>]
    let ``flow computation expression is sync only`` () =
        let workflow : Flow<int, string, int> =
            flow {
                let! env = Flow.env
                let! doubled = Ok(env * 2)
                return doubled
            }

        let publicMethods = publicInstanceMethodNames typeof<FlowBuilder>
        let argumentTypeNames = flowBuilderBindAndReturnFromArgumentNames ()

        test <@ Flow.run 21 workflow = Ok 42 @>
        test <@ publicMethods |> Array.contains "Bind" @>
        test <@ publicMethods |> Array.contains "ReturnFrom" @>
        test <@ argumentTypeNames = [| "FSharpFunc`2"; "FSharpResult`2"; "Flow`3" |] @>

    [<Fact>]
    let ``asyncFlow lives in FsFlow and composes sync flows`` () =
        let workflow : AsyncFlow<int, string, int> =
            asyncFlow {
                let! env = Flow.env
                let! baseValue = AsyncFlow.succeed(env + 1)
                return baseValue * 2
            }

        let result =
            workflow
            |> AsyncFlow.run 20
            |> Async.RunSynchronously

        test <@ typeof<AsyncFlowBuilder>.Namespace = "FsFlow" @>
        test <@ result = Ok 42 @>

    [<Fact>]
    let ``taskFlow lives in FsFlow.Net and composes async flows`` () =
        let workflow : TaskFlow<int, string, int> =
            taskFlow {
                let! env = Flow.env
                let! baseValue = AsyncFlow.succeed(env + 1)
                return baseValue * 2
            }

        let result =
            workflow
            |> TaskFlow.run 20 CancellationToken.None
            |> fun task -> task.GetAwaiter().GetResult()

        test <@ typeof<TaskFlowBuilder>.Namespace = "FsFlow.Net" @>
        test <@ result = Ok 42 @>

module Program =
    [<EntryPoint>]
    let main _ = 0
