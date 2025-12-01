module ToDo_Model

open SAFE
open Shared

type Model = {
    Todos: RemoteData<Todo list>
    Input: string
}

type Msg =
    | SetInput of string
    | LoadTodos of ApiCall<unit, Todo list>
    | SaveTodo of ApiCall<string, Todo list>
