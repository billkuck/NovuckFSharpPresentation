module ToDoServer

open SAFE
open Saturn
open Shared

module Storage =
    let todos =
        ResizeArray [
            Todo.create "Create new SAFE project"
            Todo.create "Write your app"
            Todo.create "Ship it!!!"
        ]

    let addTodo todo =
        if Todo.isValid todo.Description then
            todos.Add todo
            Ok()
        else
            Error "Invalid todo"

let todosApi ctx = {
    getTodos = fun () -> async { return Storage.todos |> List.ofSeq }
    addTodo =
        fun todo -> async {
            return
                match Storage.addTodo todo with
                | Ok() -> Storage.todos |> List.ofSeq
                | Error e -> failwith e
        }
}
