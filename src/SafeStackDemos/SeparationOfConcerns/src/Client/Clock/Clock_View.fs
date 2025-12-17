module Clock_View

open Feliz.DaisyUI
open Clock_Model
open AdtWidgets

let view (model: Clock_Model.Model) (dispatch: Clock_Model.Msg -> unit)  =
    printfn "DEBUG - Clock_View.view"
    AdtWidget.adtContainer (sprintf "%A" model.currentTime) [
        Daisy.cardActions [
            AdtWidget.button "Stop" (fun _ -> Stop |> dispatch)
            AdtWidget.button "Start" (fun _ -> Start |> dispatch)
        ]
    ]
        