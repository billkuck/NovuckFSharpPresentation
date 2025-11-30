module App

open Elmish
open Elmish.React
open ToDo_Update

open Fable.Core.JsInterop

importSideEffects "./index.css"

#if DEBUG
open Elmish.HMR
#endif

Program.mkProgram ToDo_Update.init ToDo_Update.update ToDo_View.view
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"

|> Program.run