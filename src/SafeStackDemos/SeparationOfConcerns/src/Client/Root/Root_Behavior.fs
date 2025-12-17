module Root_Behavior

open Elmish
open Root_Model

let init () : Model * Cmd<Msg> =
    let (customerAdminModel, customerAdminInitialMsg) = CustomerAdmin_Behavior.init()
    let (clockModel, clockInitialMsg) = Clock_Behavior.init();

    let initialModel = {
                CustomerAdmin = customerAdminModel;
                Clock = clockModel;
            }

    let initialCmd = Cmd.batch [
            Cmd.map Root_Model.CustomerAdminMsg customerAdminInitialMsg
            Cmd.map Root_Model.ClockMsg clockInitialMsg
        ] 
    initialModel, initialCmd

let update (message: Msg) (model: Model) : Model * Cmd<Msg> =
    match message with

    | CustomerAdminMsg  (message) ->
        let childModel, childMessage = CustomerAdmin_Behavior.update message model.CustomerAdmin
        { model with CustomerAdmin = childModel}, Cmd.map CustomerAdminMsg childMessage

    | ClockMsg  (message) ->
        let childModel, childMessage = Clock_Behavior.update message model.Clock
        { model with Clock = childModel}, Cmd.map ClockMsg childMessage

