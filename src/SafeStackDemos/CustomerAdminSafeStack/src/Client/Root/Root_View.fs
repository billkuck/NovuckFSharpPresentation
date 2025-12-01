module Root_View

open AdtWidgets

open Root_Model

let view (model: Model) dispatch =
    AdtWidget.adtContainer "Developer Dashboard" [
        AdtWidget.collapsable "Clock" false (fun (_) -> Clock_View.view model.Clock (Msg.ClockMsg >> dispatch))
    ]
    
    
    