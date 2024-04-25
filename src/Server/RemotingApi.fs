namespace Chess.Server


open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Giraffe

open Shared
open Chess
open Chess.Api

[<RequireQualifiedAccess>]
module RemotingApi =

    let private chessApi: IChessApi = {
        start = fun () -> async { return Ok GameState.init }
    }

    let endpoint: HttpHandler =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.fromValue chessApi
        |> Remoting.buildHttpHandler