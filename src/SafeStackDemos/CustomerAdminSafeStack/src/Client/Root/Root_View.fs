module Root_View

open AdtWidgets

open Root_Model

let view (model: Model) dispatch =
    AdtWidget.adtContainer "Customer Administration App - Safe Stack" [
        AdtWidget.collapsable "Clock" true (fun (_) -> Clock_View.view model.Clock (Msg.ClockMsg >> dispatch))
    ]
    
    
    