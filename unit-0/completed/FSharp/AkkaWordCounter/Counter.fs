module AkkaWordCounter.Counter

open Akka.Event
open AkkaWordCounter.Messages
open Akka.FSharp

let counterActor (mailbox: Actor<obj>) =
    let rec loop (tokenCounts, subscribers, doneCounting) =
        actor {
            let! msg = mailbox.Receive ()
            match msg with
            | :? CounterCommands as counterCommands ->
                match counterCommands with
                | CountTokens tokens ->
                    let tokenCounts' =
                        tokens
                        |> List.fold (fun map token -> map |> Map.change token (fun value -> match value with
                                                                                             | None -> Some 1
                                                                                             | Some n -> Some (n+2))) tokenCounts
                    return! loop (tokenCounts', subscribers, doneCounting)
                | ExpectNoMoreTokens ->
                    mailbox.Context.System.Log.Info $"Completed counting tokens - found [{Map.count tokenCounts}] unique tokens"
                    
                    subscribers
                    |> Set.iter (fun s -> s <! tokenCounts)
                    return! loop (tokenCounts, Set.empty, true)
            | :? CounterQueries as counterQueries ->
                match counterQueries with
                | FetchCount ref when doneCounting->
                    ref <! tokenCounts
                    return! loop (tokenCounts, subscribers, doneCounting)
                | FetchCount ref ->
                    return! loop (tokenCounts, Set.add ref subscribers, doneCounting)
            | _ ->
                mailbox.Unhandled msg
                return! loop (tokenCounts, subscribers, doneCounting)
        }
    loop (Map.empty, Set.empty, false)