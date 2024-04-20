module Index

open Chess
open Chess.Api
open Elmish
open Fable.Remoting.Client
open Feliz.style
open Shared

type ChessModel = {
    GameState: GameState
    ErrorMsg: string option
}

[<RequireQualifiedAccess>]
type ChessMsg =
    | MakeMoveRequest of string
    | GotGameState of Result<GameState, string>

let chessApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IChessApi>

let init () =
    let model = {
        GameState = GameState.init
        ErrorMsg = None
    }

    let cmd = Cmd.OfAsync.perform chessApi.start () ChessMsg.GotGameState
    model, cmd

let update (msg: ChessMsg) (model: ChessModel) =
    match msg with
    | ChessMsg.GotGameState(Ok gameStateResult) ->
        let newModel = {
            model with
                GameState = gameStateResult
                ErrorMsg = None
        }

        newModel, Cmd.none
    | ChessMsg.GotGameState(Error err) ->
        let newModel = { model with ErrorMsg = Some err }
        newModel, Cmd.none
    | ChessMsg.MakeMoveRequest s ->
        let newModel = { model with ErrorMsg = None }
        let cmd = Cmd.OfAsync.perform chessApi.move s ChessMsg.GotGameState
        newModel, cmd

open Feliz

let view (model: ChessModel) dispatch =
    Html.section [
        let errorMsg = model.ErrorMsg |> Option.defaultValue "Ok"
        Html.p $"%A{errorMsg}"

        let printField (i: int) (pieceOpt: ChessPiece option) =
            let bgColor =
                match i % 2 with
                | 0 -> "green"
                | _ -> "black"

            Html.div [
                prop.style [
                    style.width (length.px 150)
                    style.height (length.px 150)
                    style.backgroundColor bgColor
                    style.fontSize (length.em 6)
                ]
                prop.children [ Html.span (PrintConsole.printPieceOpt pieceOpt) ]
            ]

        let printRow (j: int, row: ChessPiece option array) =
            Html.div [
                prop.style [
                    style.display.grid
                    style.gridTemplateColumns [ 150; 150; 150; 150; 150; 150; 150; 150 ]
                ]
                prop.children (row |> Array.indexed |> Array.map (fun (i, pc) -> printField (i + j) pc))
            ]

        Html.div [
            prop.className "chessboard"
            prop.children (model.GameState.Board |> Array.indexed |> Array.map printRow)
        ]

    ]