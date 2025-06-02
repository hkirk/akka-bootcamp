module AkkaWordCounter.Hello

open Akka.Event
open Akka.FSharp

let helloActor (mailbox: Actor<_>) (msg: obj) =
    match msg with
    | :? string as msg ->
        mailbox.Context.System.Log.Info $"Received messsage: {msg}"
        mailbox.Sender () <! ($"{msg} reply")
    | _ -> mailbox.Unhandled msg