open System
open System.Threading
open System.Threading.Tasks
open FsFlow
open FsFlow.Net

type AppEnv =
    { Prefix: string
      Name: string
      LoadSuffix: ColdTask<string> }

let greetingFlow : Flow<AppEnv, string, string> =
    Flow.read (fun env -> $"{env.Prefix} {env.Name}") // Flow<AppEnv, string, string>

// Keep the explicit lift form here so the example shows the conversion before the shorter CE version.
let greetingAsyncFlowManual : AsyncFlow<AppEnv, string, string> =
    asyncFlow {
        let! greeting = greetingFlow |> AsyncFlow.fromFlow // AsyncFlow<AppEnv, string, string>
        return greeting.ToUpperInvariant()
    }

let greetingAsyncFlow : AsyncFlow<AppEnv, string, string> =
    asyncFlow {
        let! greeting = greetingFlow // AsyncFlow<AppEnv, string, string>
        return greeting.ToUpperInvariant()
    }

// Keep the explicit lift form here so the task interop path stays visible before the auto-boundary version.
let greetingTaskFlowManual : TaskFlow<AppEnv, string, string> =
    taskFlow {
        let! env = TaskFlow.env // TaskFlow<AppEnv, string, AppEnv>
        let! greeting = greetingFlow |> TaskFlow.fromFlow // TaskFlow<AppEnv, string, string>
        let! suffix = env.LoadSuffix |> TaskFlow.fromTask // TaskFlow<AppEnv, string, string>
        return $"{greeting}{suffix}"
    }

let greetingTaskFlow : TaskFlow<AppEnv, string, string> =
    taskFlow {
        let! env = TaskFlow.env // TaskFlow<AppEnv, string, AppEnv>
        let! greeting = greetingFlow // TaskFlow<AppEnv, string, string>
        let! suffix = env.LoadSuffix // TaskFlow<AppEnv, string, string>
        return $"{greeting}{suffix}"
    }

[<EntryPoint>]
let main _ =
    let env =
        { Prefix = "Hello"
          Name = "Ada"
          LoadSuffix = ColdTask(fun _ -> Task.FromResult "!") }

    let syncResult =
        greetingFlow
        |> Flow.run env

    let asyncResult =
        greetingAsyncFlow
        |> AsyncFlow.run env
        |> Async.RunSynchronously

    let taskResult =
        greetingTaskFlow
        |> TaskFlow.run env CancellationToken.None
        |> fun task -> task.GetAwaiter().GetResult()

    printfn "Flow: %A" syncResult
    printfn "AsyncFlow: %A" asyncResult
    printfn "TaskFlow: %A" taskResult
    0
