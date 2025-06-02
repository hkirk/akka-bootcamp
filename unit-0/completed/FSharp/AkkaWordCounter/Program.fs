open Akka.Actor
open Akka.Event
open Akka.FSharp
open AkkaWordCounter.Hello

let myActorSystem = System.create "LocalSystem" (Configuration.load())

myActorSystem.Log.Info "Helle from the actor system"

let hello = spawn myActorSystem "helloActor" (actorOf2 helloActor) 

hello <! "Hello, world"

async {
    let! whatsUp = hello <? "What's up?"
    printfn whatsUp

} |> ignore

myActorSystem.WhenTerminated.Wait ()