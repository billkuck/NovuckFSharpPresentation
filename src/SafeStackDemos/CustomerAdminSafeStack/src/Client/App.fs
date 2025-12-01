module App

open Elmish
open Elmish.React
open ToDo_Update
open CustomerAdmin_View
open Root_View

open Fable.Core.JsInterop

importSideEffects "./index.css"

#if DEBUG
open Elmish.HMR
#endif

Program.mkProgram Root_Behavior.init Root_Behavior.update Root_View.view
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"

|> Program.run