namespace Chess.Api

open Chess

[<RequireQualifiedAccess>]
type ChessSquareMsg =
    | SquareClickedMsg of row: int * col: int
    | ValidTargetMsg of (int * int) list
    | ResetMsg

[<RequireQualifiedAccess>]
type ChessBoardMsg =
    | MakeMoveRequest of string
    | GotGameState of Result<GameState, string>
    | ChessSquareMsg of ChessSquareMsg

[<RequireQualifiedAccess>]
type ServerMsg =
    | ResetGame
    | MakeMove of string

[<RequireQualifiedAccess>]
module BridgeConfig =
    let endpoint = "/socket"