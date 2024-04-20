namespace Chess

[<RequireQualifiedAccess>]
type Color =
    | White
    | Black

[<RequireQualifiedAccess>]
module Color =
    let other (color: Color) =
        match color with
        | Color.White -> Color.Black
        | Color.Black -> Color.White

[<RequireQualifiedAccess>]
type ChessPiece =
    | Pawn of Color
    | Rook of Color
    | Knight of Color
    | Bishop of Color
    | Queen of Color
    | King of Color

[<RequireQualifiedAccess>]
module ChessPiece =
    let hasColor (piece: ChessPiece) (reqColor: Color) =
        match piece with
        | ChessPiece.Pawn color
        | ChessPiece.Rook color
        | ChessPiece.Knight color
        | ChessPiece.Bishop color
        | ChessPiece.Queen color
        | ChessPiece.King color -> color = reqColor

    let color (piece: ChessPiece) =
        match piece with
        | ChessPiece.Pawn color
        | ChessPiece.Rook color
        | ChessPiece.Knight color
        | ChessPiece.Bishop color
        | ChessPiece.Queen color
        | ChessPiece.King color -> color