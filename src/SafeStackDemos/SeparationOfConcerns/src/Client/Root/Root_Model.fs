module Root_Model

type Model = {
    Clock: Clock_Model.Model;
    ToDoApp: ToDo_Model.Model;
}

type Msg =
    | ClockMsg of Clock_Model.Msg
    | ToDoMsg of ToDo_Model.Msg
