module Clock_View

open Feliz.DaisyUI
open Clock_Model
open NkWidgets

let view (model: Clock_Model.Model) (dispatch: Clock_Model.Msg -> unit)  =
    printfn "DEBUG - Clock_View.view"
    NkWidget.nkContainer (sprintf "%A" model.currentTime) [
        Daisy.cardActions [
            NkWidget.button "Stop" (fun _ -> Stop |> dispatch)
            NkWidget.button "Start" (fun _ -> Start |> dispatch)
        ]
    ]
        