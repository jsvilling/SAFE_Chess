namespace Chess

type GameState =
    {
      GameId: System.Guid
      Active: Color
      Board: ChessPiece option array array
      Moves: string array
      ErrorMsg: string option
      IsChess: bool
    }

[<RequireQualifiedAccess>]
module GameState =
    let init () = {
        GameId = System.Guid.NewGuid()
        Active = Color.White
        Board = ChessBoard.init
        Moves = [||]
        ErrorMsg = None
        IsChess = false
    }

    let update (prevState: GameState) (move: string) =
        match ChessBoard.tryApplyMove prevState.Board move with
        | Error err ->
            let nextState = { prevState with ErrorMsg = Some err }
            nextState
        | Ok newBoard ->
            let newMoves = Array.append prevState.Moves [| move |]
            let newActive = Color.other prevState.Active
            let isChess = MovementRange.hasChess newBoard newActive
            let nextState = { prevState with Active = newActive; Moves = newMoves; Board = newBoard; ErrorMsg = None; IsChess = isChess }
            nextState

    let updateAll (initialState: GameState) (moves: string array) =
        let folder (state: GameState list) (move: string) =
            let prev = (List.last state)
            match prev.ErrorMsg with
            | None -> state @ [ update (List.last state) move ]
            | Some _ -> state

        moves |> Array.fold folder [initialState]