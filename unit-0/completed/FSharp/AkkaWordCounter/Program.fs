open Akka.Actor
open Akka.Event
open Akka.FSharp

let myActorSystem = System.create "LocalSystem" (Configuration.load())

myActorSystem.Log.Info "Helle from the actor system"

myActorSystem.WhenTerminated.Wait ()