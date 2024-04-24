namespace Chess.Api

open Chess

type IChessApi = {
    start: unit -> Async<Result<GameState, string>>
}