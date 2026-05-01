---
title: Runnable Examples
description: Application-shaped examples that are executed during docs generation and mirrored back into the site.
---

# Runnable Examples

This page shows the examples that are executed during the docs build, so the public docs stay tied to real code and observed output.

The examples below are built from the repository projects, run with the current source, and then written back into this page.

The code blocks keep the important API calls on the same lines as the values they bind, with trailing comments where that makes the signature easier to read.
The examples prefer the normal direct-bind style inside computation expressions, so the docs reflect the recommended day-to-day usage.

## Request Boundary Example

This example shows a request boundary that pulls a user from a database-like environment, threads a trace id through the request context, and reuses the same validation shape across Flow, AsyncFlow, and TaskFlow.

Run it:

```bash
dotnet run --project examples/FsFlow.Examples/FsFlow.Examples.fsproj --nologo
```

Source:

- [Program.fs](https://github.com/adz/FsFlow/blob/main/examples/FsFlow.Examples/Program.fs)

Source code:

```fsharp
open System
open System.Threading
open System.Threading.Tasks
open FsFlow
open FsFlow.Net
open FsFlow.Validate

type User =
    { Id: int
      Name: string }

type AppDb =
    { FindUser: int -> User option }

type RequestEnv =
    { TraceId: Guid
      Prefix: string
      Db: AppDb
      LoadSuffix: ColdTask<string> }

let validateName (name: string) : Result<string, string> =
    okIfNotBlank name
    |> orElse "name is required"

let loadUser : Flow<RequestEnv, string, User> =
    flow {
        let! db = Flow.read _.Db // Flow<RequestEnv, string, AppDb>
        let! user = db.FindUser 42 |> Flow.fromOption "user not found" // Flow<RequestEnv, string, User>
        return user
    }

let renderTrace : AsyncFlow<RequestEnv, string, string> =
    asyncFlow {
        let! env = AsyncFlow.env // AsyncFlow<RequestEnv, string, RequestEnv>
        let! user = loadUser // AsyncFlow<RequestEnv, string, User>
        let! validName = validateName user.Name // AsyncFlow<RequestEnv, string, string>
        return $"{env.Prefix} [{env.TraceId}] {validName}"
    }

let publishResponse : TaskFlow<RequestEnv, string, string> =
    taskFlow {
        let! env = TaskFlow.env // TaskFlow<RequestEnv, string, RequestEnv>
        let! user = loadUser // TaskFlow<RequestEnv, string, User>
        let! suffix = env.LoadSuffix // TaskFlow<RequestEnv, string, string>
        return $"{env.Prefix} [{env.TraceId}] {user.Name}{suffix}"
    }

[<EntryPoint>]
let main _ =
    let environment =
        { TraceId = Guid.Parse "11111111-1111-1111-1111-111111111111"
          Prefix = "Hello"
          Db =
            { FindUser =
                function
                | 42 -> Some { Id = 42; Name = "Ada" }
                | _ -> None }
          LoadSuffix = ColdTask(fun _ -> Task.FromResult "!") }

    let syncResult =
        loadUser
        |> Flow.run environment

    let asyncResult =
        renderTrace
        |> AsyncFlow.run environment
        |> Async.RunSynchronously

    let taskResult =
        publishResponse
        |> TaskFlow.run environment CancellationToken.None
        |> fun task -> task.GetAwaiter().GetResult()

    printfn "Flow result: %A" syncResult
    printfn "AsyncFlow result: %A" asyncResult
    printfn "TaskFlow result: %A" taskResult
    0

```

Observed output:

```text
Flow result: Ok { Id = 42
     Name = "Ada" }
AsyncFlow result: Ok "Hello [11111111-1111-1111-1111-111111111111] Ada"
TaskFlow result: Ok "Hello [11111111-1111-1111-1111-111111111111] Ada!"
```

