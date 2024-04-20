[<RequireQualifiedAccess>]
module GameRegistry

open Chess

//ResizeArray()
let mutable game = GameState.init

let startGame () =
    game <- GameState.init
    game

let move (moveStr: string) =
    game <- GameState.update game moveStr
    game