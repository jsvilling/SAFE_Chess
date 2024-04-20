namespace Chess.Api

open Chess

type IChessApi = {
    start: unit -> Async<Result<GameState, string>>
    move: string -> Async<Result<GameState, string>>
}