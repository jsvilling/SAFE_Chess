namespace Chess

open System

module ChessBoard =
    let private startingRow (color: Color) =
        [|
            ChessPiece.Rook color |> Some
            ChessPiece.Knight color |> Some
            ChessPiece.Bishop color |> Some
            ChessPiece.Queen color |> Some
            ChessPiece.King color |> Some
            ChessPiece.Bishop color |> Some
            ChessPiece.Knight color |> Some
            ChessPiece.Rook color |> Some
        |]

    let colNames = [| "A"; "B"; "C"; "D"; "E"; "F"; "G"; "H" |] 
    let private fieldNotationForIndex row col =
        $"%s{colNames[col]}%i{row}"

    let init =
        [|
            startingRow Color.White
            Array.replicate 8 (Some (ChessPiece.Pawn Color.White))
            Array.replicate 8 None
            Array.replicate 8 None
            Array.replicate 8 None
            Array.replicate 8 None
            Array.replicate 8 (Some (ChessPiece.Pawn Color.Black))
            startingRow Color.Black
        |]
        
    let private toIndexes (move: string) =
        let colStr = move.Substring(0, 1)
        let rowStr = move.Substring(1)
        let iLetter = Array.findIndex (fun c -> c = colStr) colNames
        let iNumber = Int32.Parse(rowStr) - 1
        iNumber, iLetter
        
    let tryApplyMove (board: ChessPiece option array array) (move: string) =
        let mvs = move.Split ':'
        
        let fromRow, fromCol = toIndexes mvs[0]
        let toRow, toCol = toIndexes mvs[1]
        
        let range p = MovementRange.movementRange (fromRow, fromCol) p board
        let isInRange p =
            let ran = range p
            ran |> Seq.contains (toRow, toCol)
            
        match board[fromRow][fromCol] with
        | Some p as pOpt when isInRange p ->
            let newBoard = board |> Array.map Array.copy
            newBoard[fromRow][fromCol] <- Option.None
            newBoard[toRow][toCol] <- pOpt
            Ok newBoard
        | Some _ -> Error "Invalid Move"
        | None -> Error "No Piece Selected"
