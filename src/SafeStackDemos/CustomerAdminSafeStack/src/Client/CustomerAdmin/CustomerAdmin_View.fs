module CustomerAdmin_View

open SAFE
open Shared
open Feliz
open CustomerAdmin_Model
open AdtWidgets

let view model dispatch =
    printfn "DEBUG - CustomerAdmin_View.view"
    AdtWidget.adtContainer "This is a test" [
    ]
