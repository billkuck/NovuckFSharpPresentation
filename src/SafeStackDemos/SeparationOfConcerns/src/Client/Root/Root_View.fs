module Root_View

open NkWidgets

open Root_Model

let view (model: Model) dispatch =


    NkWidget.nkContainer "Customer Administration App - Safe Stack" [
        NkWidget.collapsable "Customer Admin" true (fun (_) -> CustomerAdmin_View.view model.CustomerAdmin (Msg.CustomerAdminMsg >> dispatch))
        NkWidget.collapsable "Clock" true (fun (_) -> Clock_View.view model.Clock (Msg.ClockMsg >> dispatch))
    ]
    
    
    