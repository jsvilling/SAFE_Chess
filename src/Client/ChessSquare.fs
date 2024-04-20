namespace Chess.Ui

open Chess
open Feliz

[<RequireQualifiedAccess>]
type ChessSquareMsg =
    | SquareClickedMsg of row: int * col: int
    | ValidTargetMsg of (int * int) list
    | ResetMsg

type ChessSquareModel = {
    Row: int
    Col: int
    Piece: ChessPiece option
    Selected: bool
    ValidTarget: bool
}

[<RequireQualifiedAccess>]
module ChessSquareModel =
    let init (i: int) (j: int) (pieceOpt: ChessPiece option) = {
        Row = i
        Col = j
        Piece = pieceOpt
        Selected = false
        ValidTarget = false
    }

    let update (msg: ChessSquareMsg) (model: ChessSquareModel) =
        match msg with
        | ChessSquareMsg.ValidTargetMsg targets ->
            let isTarget = targets |> List.contains (model.Row, model.Col)
            let newModel = { model with ValidTarget = isTarget; }
            newModel
        | ChessSquareMsg.SquareClickedMsg (row, col) when row = model.Row && col = model.Col ->
            let newModel = { model with Selected = true }
            newModel
        | ChessSquareMsg.SquareClickedMsg _ ->
            let newModel = { model with Selected = false }
            newModel
        | ChessSquareMsg.ResetMsg ->
            let newModel = { model with Selected = false; ValidTarget = false; }
            newModel

[<RequireQualifiedAccess>]
module ChessSquareView =
    let view (model: ChessSquareModel) (dispatch: ChessSquareMsg -> unit) =
        let bgColor =
            match model.Selected, model.ValidTarget, (model.Col + model.Row) % 2 with
            | true, _, _ -> "lightgrey"
            | _, true, _ -> "purple"
            | _, _, 0 -> "green"
            | _ -> "black"

        Html.div [
            prop.style [
                style.width (length.px 150)
                style.height (length.px 150)
                style.backgroundColor bgColor
                style.fontSize (length.em 6)
            ]
            prop.children [ Html.span (PrintConsole.printPieceOpt model.Piece) ]
            prop.onClick (fun _ -> dispatch (ChessSquareMsg.SquareClickedMsg(model.Row, model.Col)))
        ]