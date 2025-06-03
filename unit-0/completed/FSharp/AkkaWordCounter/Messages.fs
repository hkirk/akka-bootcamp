module AkkaWordCounter.Messages

open Akka.Actor

type DocumentCommands =
    | ProcessDocument of string
   
type CounterCommands =
    | CountTokens of string list
    // parser reached the end of the document
    | ExpectNoMoreTokens
    
type CounterQueries =
    // Send this actor a notification once counting is complete
    | FetchCount of IActorRef