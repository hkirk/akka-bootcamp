open System
open System.Threading
open Akka.Actor
open Akka.Event
open Akka.FSharp
open AkkaWordCounter.Counter
open AkkaWordCounter.Hello
open AkkaWordCounter.Messages
open AkkaWordCounter.Parser

let myActorSystem = System.create "LocalSystem" (Configuration.load())

myActorSystem.Log.Info "Hello from the actor system"

let hello = spawn myActorSystem "helloActor" (actorOf2 helloActor) 

hello <! "Hello, world"

// async {
//     let! whatsUp = hello <? "What's up?"
//     printfn whatsUp
// } |> ignore

let counter = spawn myActorSystem "counterActor" counterActor
let parser = spawn myActorSystem "parserActor" (actorOf2 (parseActor counter))

let saveRef: System.Func<IActorRef, obj> = System.Func<IActorRef, obj>(fun (ref: IActorRef) -> (FetchCount ref: obj))

let completionPromise = counter.Ask<Map<string, int>>(saveRef, TimeSpan.FromMinutes(10), CancellationToken.None)

let text = """
This is a test of the Akka.NET Word Counter.
I would go
"""

parser <! (ProcessDocument text)

task {
    let! counts = completionPromise
    counts |> Map.iter (fun k v -> myActorSystem.Log.Info $"{k}: {v} instances")
} |> ignore
    
async {
    
    myActorSystem.WhenTerminated.Wait()
} |> ignore