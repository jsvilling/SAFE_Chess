namespace Chess.Server

open Chess
open Giraffe
open Elmish.Bridge

open Chess.Api

type ServerModel = {
    GameState: GameState
}


[<RequireQualifiedAccess>]
module ServerModel =
    let init = {
        GameState = GameState.init ()
    }

[<RequireQualifiedAccess>]
module WebSocketApi =

    let private init _ _ =
        ServerModel.init, Elmish.Cmd.Empty

    let private update (clientDispatch: Elmish.Dispatch<ChessBoardMsg>) (msg: ServerMsg) (model: ServerModel) =

        let newGameState =
            match msg with
            | ServerMsg.ResetGame -> GameState.init ()
            | ServerMsg.MakeMove move -> GameState.update model.GameState move

        clientDispatch (ChessBoardMsg.GotGameState (Ok newGameState))

        let newModel = { model with GameState = newGameState }

        newModel, Elmish.Cmd.Empty

    let endpoint: HttpHandler =
        Bridge.mkServer BridgeConfig.endpoint init update
        |> Bridge.run Giraffe.server