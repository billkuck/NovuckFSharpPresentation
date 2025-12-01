module App

open Elmish

open Fable.Core.JsInterop

importSideEffects "./index.css"

open Elmish.HMR

Program.mkProgram Root_Behavior.init Root_Behavior.update Root_View.view
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"

|> Program.run