module Clock_Behavior

open Clock_Model
open Elmish
open System

let waitSomeSeconds (Seconds updateFrequencyInSeconds) =
    fun () ->
        async {
            do! Async.Sleep (updateFrequencyInSeconds * 1000)
            return DateTime.Now
        }

let init () =
    {
        running = false;
        currentTime = DateTime.MinValue
        updateFrequencyInSeconds = 1 |> Seconds
    }, Cmd.ofMsg Start

let update
    (msg: Clock_Model.Msg)
    (model: Clock_Model.Model)
    : Clock_Model.Model * Cmd<Clock_Model.Msg> =

    match msg with
    | Start -> 
        { model with running = true; currentTime = DateTime.Now;  },
        Cmd.OfAsync.perform (waitSomeSeconds model.updateFrequencyInSeconds) () Expired

    | Stop ->
        { model with running = false },
        Cmd.none

    | Expired current ->
        { model with currentTime = current },
        if model.running
            then (Cmd.ofMsg Start)
            else Cmd.none
