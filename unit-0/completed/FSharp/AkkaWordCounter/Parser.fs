module AkkaWordCounter.Parser

open Akka.Actor
open Akka.FSharp
open AkkaWordCounter.Messages

let tokenBatchSize = 10

let parseActor (countActor: ICanTell) (mailbox: Actor<DocumentCommands>)  msg=
    match msg with
    | ProcessDocument str ->
        str.Split(" ")
        |> Array.chunkBySize 10
        |> Array.map (fun array -> List.ofArray array)
        |> Array.iter (fun chunk -> countActor <! (CountTokens chunk))
        countActor <! ExpectNoMoreTokens
    | _ ->
        mailbox.Unhandled msg