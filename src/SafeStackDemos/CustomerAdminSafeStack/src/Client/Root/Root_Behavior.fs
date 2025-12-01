module Root_Behavior

open Elmish
open Root_Model

let init () : Model * Cmd<Msg> =
    let (clockModel, clockInitialMsg) = Clock_Behavior.init();
    let (customerAdminModel, customerAdminMsg) = CustomerAdmin_Update.init();

    let initialModel = {
                Clock = clockModel;
                CustomerAdmin = customerAdminModel;
            }

    let initialCmd = Cmd.batch [
            Cmd.map Root_Model.ClockMsg clockInitialMsg
            Cmd.map Root_Model.CustomerAdminMsg customerAdminMsg
        ] 
    initialModel, initialCmd

let update (message: Msg) (model: Model) : Model * Cmd<Msg> =
    match message with

    | ClockMsg  (message) ->
        let childModel, childMessage = Clock_Behavior.update message model.Clock
        { model with Clock = childModel}, Cmd.map ClockMsg childMessage

    | CustomerAdminMsg  (message) ->
        let childModel, childMessage = CustomerAdmin_Update.update message model.CustomerAdmin
        { model with CustomerAdmin = childModel}, Cmd.map CustomerAdminMsg childMessage

