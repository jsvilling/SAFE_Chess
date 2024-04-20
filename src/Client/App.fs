module App

open Chess.Api
open Chess.Ui
open Elmish
open Elmish.React

open Fable.Core.JsInterop
open Fable.Remoting.Client
open Shared

importSideEffects "./index.css"

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram ChessBoardModel.init ChessBoardModel.update ChessBoardView.view
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactSynchronous "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run