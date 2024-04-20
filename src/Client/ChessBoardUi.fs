namespace Chess.Ui

open Chess
open Chess.Api
open Elmish
open Fable.Remoting.Client
open Feliz
open Shared

[<RequireQualifiedAccess>]
type ChessBoardMsg =
    | MakeMoveRequest of string
    | GotGameState of Result<GameState, string>
    | ChessSquareMsg of ChessSquareMsg

type ChessBoardModel = {
    GameState: GameState
    SquareModels: ChessSquareModel array array
    ErrorMsg: string option
    SelectedSquare: (int * int) option
    ValidMoves: (int * int) list option
}

[<RequireQualifiedAccess>]
module ChessBoardModel =

    let chessApi =
        Remoting.createApi ()
        |> Remoting.withRouteBuilder Route.builder
        |> Remoting.buildProxy<IChessApi>

    let buildSquareModels gameState =
        let mkSquares (i: int, row: ChessPiece option array) =
            row |> Array.indexed |> Array.map (fun (j, pc) -> ChessSquareModel.init i j pc)

        let squares = gameState.Board  |> Array.indexed |> Array.map mkSquares
        squares


    let init () =
        let model =
            let gameState = GameState.init
            let squares = buildSquareModels gameState

            let initModel = {
                GameState = gameState
                SquareModels = squares
                ErrorMsg = None
                SelectedSquare = None
                ValidMoves = None
            }
            initModel

        let cmd = Cmd.OfAsync.perform chessApi.start () ChessBoardMsg.GotGameState
        model, cmd

    let private updateSquares model (squareMsg) =
        let newSquares =
            model.SquareModels
            |> Array.map (fun row -> row |> Array.map (ChessSquareModel.update squareMsg))
        newSquares

    let update (msg: ChessBoardMsg) (model: ChessBoardModel) =
        match msg with
        | ChessBoardMsg.GotGameState(Ok gameStateResult) ->
            let newModel = {
                model with
                    GameState = gameStateResult
                    ErrorMsg = None
            }
            newModel, Cmd.none
        | ChessBoardMsg.GotGameState(Error err) ->
            let newModel = { model with ErrorMsg = Some err }
            newModel, Cmd.none
        | ChessBoardMsg.MakeMoveRequest s ->
            let newModel = { model with ErrorMsg = None }
            let cmd = Cmd.OfAsync.perform chessApi.move s ChessBoardMsg.GotGameState
            newModel, cmd
        | ChessBoardMsg.ChessSquareMsg (ChessSquareMsg.SquareClickedMsg (row, col) as sqMsg) ->
            let pc = model.SquareModels[row][col]

            let validRangeOpt =
                pc.Piece
                |> Option.map (fun pc ->
                    MovementRange.movementRange (row, col) pc model.GameState.Board
                )

            let cmd =
                validRangeOpt
                |> Option.map (fun lst ->
                    lst
                    |> ChessSquareMsg.ValidTargetMsg
                    |> ChessBoardMsg.ChessSquareMsg
                    |> Cmd.ofMsg
                )
                |> Option.defaultValue Cmd.none

            let newSquares = updateSquares model sqMsg
            let newSelected = Some (row, col)

            let newModel = { model with SquareModels = newSquares; SelectedSquare = newSelected }
            newModel, cmd
        | ChessBoardMsg.ChessSquareMsg sqMsg ->
            let newSquares = updateSquares model sqMsg
            let newModel = { model with SquareModels = newSquares }
            newModel, Cmd.none

[<RequireQualifiedAccess>]
module ChessBoardView =
    let view (model: ChessBoardModel) (dispatch: ChessBoardMsg -> unit) =
        Html.section [
            let errorMsg = model.ErrorMsg |> Option.defaultValue "Ok"
            Html.p $"%A{errorMsg}"

            let mkRow (row: ChessSquareModel array) =
                Html.div [
                    prop.style [
                        style.display.grid
                        style.gridTemplateColumns [ 150; 150; 150; 150; 150; 150; 150; 150 ]
                    ]
                    prop.children (row |> Array.map (fun sq -> ChessSquareView.view sq (ChessBoardMsg.ChessSquareMsg >> dispatch)))
                ]

            Html.div [
                prop.className "chessboard"
                prop.children (
                    model.SquareModels
                    |> Array.map mkRow
                    |> Array.rev
                )
            ]
        ]