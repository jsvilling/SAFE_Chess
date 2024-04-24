namespace Chess.Server

open Giraffe
open Elmish.Bridge

open Chess.Api

type ServerModel = {
    State: int
}

[<RequireQualifiedAccess>]
module ServerModel =
    let init = {
        State = 0
    }

[<RequireQualifiedAccess>]
module WebSocketApi =

    let private init _ _ =
        ServerModel.init, Elmish.Cmd.Empty

    let private update _ _ model =
        let newModel = { model with State = model.State + 1 }
        newModel, Elmish.Cmd.Empty

    let endpoint: HttpHandler =
        Bridge.mkServer BridgeConfig.endpoint init update
        |> Bridge.run Giraffe.server