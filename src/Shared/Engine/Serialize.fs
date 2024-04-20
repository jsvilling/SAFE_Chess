[<RequireQualifiedAccess>]
module PrintConsole

open Chess
open ChessBoard

let printPiece (piece: ChessPiece) =
    match piece with
    | ChessPiece.Pawn Color.White -> "♙"
    | ChessPiece.Rook Color.White -> "♖"
    | ChessPiece.Knight Color.White -> "♘"
    | ChessPiece.Bishop Color.White -> "♗"
    | ChessPiece.Queen Color.White -> "♕"
    | ChessPiece.King Color.White -> "♔"

    | ChessPiece.Pawn Color.Black -> "♟︎"
    | ChessPiece.Rook Color.Black -> "♜"
    | ChessPiece.Knight Color.Black -> "♞"
    | ChessPiece.Bishop Color.Black -> "♝"
    | ChessPiece.Queen Color.Black -> "♛"
    | ChessPiece.King Color.Black -> "♚"

let printPieceOpt (piece: ChessPiece option) =
    piece
    |> Option.map printPiece
    |> Option.defaultValue ""

let serializeBoard (board: ChessPiece option array array) =
    board
    |> Array.rev
    |> Array.map (fun row -> row |> Array.map printPieceOpt |> String.concat "\t")
    |> String.concat "\n \n \n"

let serializeState (state: GameState) =
    let boardStr = serializeBoard state.Board
    let errorStr = state.ErrorMsg |> Option.defaultValue "-"
    let headerStr = $"Active: %A{state.Active} - Chess: %b{state.IsChess} - Error: %s{errorStr}"
    $"%s{headerStr} \n \n %s{boardStr}"

let printBoard (board: ChessPiece option array array) =
    board
    |> serializeBoard
    |> (fun rowStr -> printfn $"%s{rowStr}\n")

let printGame (state: GameState) = printBoard state.Board