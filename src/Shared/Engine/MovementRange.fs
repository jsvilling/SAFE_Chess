namespace Chess

[<RequireQualifiedAccess>]
module MovementRange =

    let rec isInBoard (row: int, col: int) =
        (max row col) < 8 && (min row col) >= 0

    let rec private rangeToLimit
        (upper: int, lower: int as bounds)
        (board: ChessPiece option array array)
        (transX: int -> int)
        (transY: int -> int)
        (color: Color)
        (r: int)
        (c: int)
        (moves: (int * int) list)
        =
        let nR = transX r
        let nC = transY c
        let hasPiece () =
            let pcOpt = board[nR][nC]
            pcOpt.IsSome

        if max nR nC >= upper || min nR nC < lower then
            moves
        else
            match board[nR][nC] with
            | Some piece when ChessPiece.hasOtherColor piece color -> (moves @ [(nR, nC)])
            | Some _ -> moves
            | None ->
                let nR = transX r
                let nC = transY c
                rangeToLimit bounds board transX transY color nR nC (moves @ [(nR, nC)])

    let rec private rangeToLimitForChess
        (board: ChessPiece option array array)
        (transX: int -> int)
        (transY: int -> int)
        (r: int)
        (c: int)
        (moves: (int * int) list)
        =
        let nR = transX r
        let nC = transY c
        let hasPiece () =
            let pcOpt = board[nR][nC]
            pcOpt.IsSome
        if max nR nC >= 8 || min nR nC < 0 then
            if hasPiece () then
                (moves @ [(nR, nC)])
            else
                moves
        else
            let nR = transX r
            let nC = transY c
            rangeToLimitForChess board transX transY nR nC (moves @ [(nR, nC)])

    let private rangeToBorder = rangeToLimit (8, 0)

    let private knight (row: int, col: int) =
        [
            (row + 2, col + 1)
            (row + 2, col - 1)
            (row - 2, col + 1)
            (row - 2, col - 1)

            (row + 1, col + 2)
            (row + 1, col - 2)
            (row - 1, col + 2)
            (row - 1, col - 2)
        ] |> List.filter isInBoard

    let private king row col =
        [
            (row + 1, col)
            (row + 1, col - 1)
            (row + 1, col + 1)
            (row, col + 1)
            (row, col - 1)
            (row - 1, col)
            (row - 1, col + 1)
            (row - 1, col - 1)
        ] |> List.filter isInBoard

    let private diagonalsToBorder board color (row: int, col: int) =
        let up r = r + 1
        let down r = r - 1
        let right r = r + 1
        let left r = r - 1

        [
            rangeToBorder board up right color
            rangeToBorder board up left color
            rangeToBorder board down right color
            rangeToBorder board down left color
        ]  |> List.collect (fun mv -> mv row col [])

    let private horizontalsToBorder board color (row: int, col: int) =
        let up r = r + 1
        let down r = r - 1
        let right r = r + 1
        let left r = r - 1

        [
            rangeToBorder board up id color
            rangeToBorder board down id color
            rangeToBorder board id right color
            rangeToBorder board id left color
        ]  |> List.collect (fun mv -> mv row col [])

    let keep (row: int, col: int) (color: Color) (board: ChessPiece option array array) =
        board[row][col]
        |> Option.map (fun p -> ChessPiece.hasOtherColor p color)
        |> Option.defaultValue true

    let movementRange (row: int, col: int) (piece: ChessPiece) (board: ChessPiece option array array) =
        let color = ChessPiece.color piece
        let horizontals () = horizontalsToBorder board color (row, col)
        let diagonals () = diagonalsToBorder board color (row, col)

        match piece with
        | ChessPiece.Pawn Color.White when row = 1-> [ (row + 1, col); (row + 2, col)  ]
        | ChessPiece.Pawn Color.White when row < 7 -> [ (row + 1, col) ]

        | ChessPiece.Pawn Color.Black when row = 6 -> [ (row - 1, col); (row - 2, col)  ]
        | ChessPiece.Pawn Color.Black when row > 0 -> [ (row - 1, col) ]

        | ChessPiece.Rook _ -> horizontalsToBorder board color (row, col)
        | ChessPiece.Knight _ -> knight (row, col)
        | ChessPiece.Bishop _ -> diagonals ()
        | ChessPiece.Queen _ -> List.append (horizontals ()) (diagonals ())
        | ChessPiece.King _ -> king row col
        | _ -> []
        |> List.filter (fun rc -> keep rc (ChessPiece.color piece) board)

    let hasChess (board: ChessPiece option array array) (defenderColor: Color) =
        let attackerColor = Color.other defenderColor
        let kingPosition =
            Array.indexed board
            |> Array.choose (fun (i, r) ->
                Array.tryFindIndex (fun p -> p = Some (ChessPiece.King defenderColor)) r
                |> Option.map (fun j -> (i, j))
            )
            |> Array.head

        let all = [
            ChessPiece.Pawn attackerColor
            ChessPiece.Rook defenderColor
            ChessPiece.Knight defenderColor
            ChessPiece.Bishop attackerColor
            ChessPiece.Queen defenderColor
        ]

        all
        |> List.exists (fun p ->
            let mvs = movementRange kingPosition p board
            mvs |> List.exists (fun (r, c) -> board[r][c] = Some p)
        )