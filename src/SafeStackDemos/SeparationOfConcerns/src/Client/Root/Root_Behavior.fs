module Root_Behavior

open Elmish
open Root_Model

let init () : Model * Cmd<Msg> =
    let (clockModel, clockInitialMsg) = Clock_Behavior.init();
    let (toDoModel, toDoInitialMsg) = ToDo_Update.init();

    let initialModel = {
                Clock = clockModel;
                ToDoApp = toDoModel;
            }

    let initialCmd = Cmd.batch [
            Cmd.map Root_Model.ClockMsg clockInitialMsg
            Cmd.map Root_Model.ToDoMsg toDoInitialMsg
        ] 
    initialModel, initialCmd

let update (message: Msg) (model: Model) : Model * Cmd<Msg> =
    match message with

    | ClockMsg  (message) ->
        let childModel, childMessage = Clock_Behavior.update message model.Clock
        { model with Clock = childModel}, Cmd.map ClockMsg childMessage

    | ToDoMsg  (message) ->
        let childModel, childMessage = ToDo_Update.update message model.ToDoApp
        { model with ToDoApp = childModel}, Cmd.map ToDoMsg childMessage

