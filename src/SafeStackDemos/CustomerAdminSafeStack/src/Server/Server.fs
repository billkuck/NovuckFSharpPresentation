module Server

open SAFE
open Saturn
open Shared

//open ToDoServer
open CustomerAdminServer

//let webApp = Api.make todosApi
let webApp = Api.make customerAdminApi

let app = application {
    use_router webApp
    memory_cache
    use_static "public"
    use_gzip
}

[<EntryPoint>]
let main _ =
    run app
    0