module App

open Chess.Api
open Chess.Ui
open Elmish
open Elmish.React

open Fable.Core.JsInterop

open Elmish.Bridge

importSideEffects "./index.css"

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram ChessBoardModel.init ChessBoardModel.update ChessBoardView.view
|> Program.withBridge BridgeConfig.endpoint
#if DEBUG
|> Program.withConsoleTrace
|> Program.withReactSynchronous "elmish-app"
|> Program.withDebugger
#endif
|> Program.run