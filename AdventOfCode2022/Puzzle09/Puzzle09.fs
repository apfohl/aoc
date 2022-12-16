module AdventOfCode2022.Puzzle09.Puzzle09

open System.IO
open NUnit.Framework

type Direction =
    | Up
    | Right
    | Down
    | Left

type Move = { direction: Direction; amount: int }

let toDirection (value: string) =
    match Seq.toList value with
    | 'U' :: _ -> Up
    | 'R' :: _ -> Right
    | 'D' :: _ -> Down
    | 'L' :: _ -> Left
    | direction -> failwith $"Wrong direction: {direction}"

let inMoves (line: string) =
    match line.Split [| ' ' |] with
    | [| first; second |] ->
        { direction = toDirection first
          amount = int second }
    | line -> failwith $"Line could no be parsed: '{line}'"

type Rope = { head: int * int; tail: int * int }

type State =
    { rope: Rope
      tailPositions: (int * int) list }

let moveHead (direction: Direction) (state: State) : State =
    match direction, state.rope.head with
    | Up, (x, y) -> { state with rope = { state.rope with head = x, y + 1 } }
    | Right, (x, y) -> { state with rope = { state.rope with head = x + 1, y } }
    | Down, (x, y) -> { state with rope = { state.rope with head = x, y - 1 } }
    | Left, (x, y) -> { state with rope = { state.rope with head = x - 1, y } }

let moveTail (direction: Direction) (state: State) : State =
    let sq x = x * x

    let distance (x1, y1) (x2, y2) =
        let x = x2 - x1 |> sq |> double
        let y = y2 - y1 |> sq |> double

        x + y |> sqrt

    let newState x y (s: State) =
        let xTail, yTail = s.rope.tail
        let newRope = { s.rope with tail = xTail + x, yTail + y }

        let newTailPositions =
            if s.tailPositions |> List.contains newRope.tail then
                s.tailPositions
            else
                newRope.tail :: s.tailPositions

        { s with
            rope = newRope
            tailPositions = newTailPositions }

    match state.rope.head, state.rope.tail, direction with
    | head, tail, _ when distance head tail <= sqrt 2.0 -> state
    | (xHead, _), (xTail, _), Up when xHead > xTail -> state |> newState 1 1
    | (xHead, _), (xTail, _), Up when xHead = xTail -> state |> newState 0 1
    | (xHead, _), (xTail, _), Up when xHead < xTail -> state |> newState -1 1
    | (_, yHead), (_, yTail), Right when yHead > yTail -> state |> newState 1 1
    | (_, yHead), (_, yTail), Right when yHead = yTail -> state |> newState 1 0
    | (_, yHead), (_, yTail), Right when yHead < yTail -> state |> newState 1 -1
    | (xHead, _), (xTail, _), Down when xHead > xTail -> state |> newState 1 -1
    | (xHead, _), (xTail, _), Down when xHead = xTail -> state |> newState 0 -1
    | (xHead, _), (xTail, _), Down when xHead < xTail -> state |> newState -1 -1
    | (_, yHead), (_, yTail), Left when yHead > yTail -> state |> newState -1 1
    | (_, yHead), (_, yTail), Left when yHead = yTail -> state |> newState -1 0
    | (_, yHead), (_, yTail), Left when yHead < yTail -> state |> newState -1 -1
    | _ -> failwith "Wrong combination!"

let rec moveRope (state: State) (move: Move) : State =
    match move.direction, move.amount with
    | _, 0 -> state
    | direction, _ ->
        moveRope (state |> moveHead direction |> moveTail direction) { move with amount = move.amount - 1 }

[<Test>]
let PartOne () =
    Path.Combine("Puzzle09", "input.txt")
    |> lines inMoves
    |> Seq.fold
        moveRope
        { rope = { head = 0, 0; tail = 0, 0 }
          tailPositions = [ (0, 0) ] }
    |> fun state -> List.length state.tailPositions
    |> printfn "%A"

type Point = int * int

type Knot = { position: Point; visits: Point list }

type MultiRope = Knot list

let rec moveMultiRope (rope: MultiRope) (move: Move) : MultiRope =
    let updateKnots (direction: Direction) (rope: MultiRope) : MultiRope =
        let moveKnot (precedingKnot: Point option) (knot: Knot) : Knot * Point option =
            let move (offsetX: int) (offsetY: int) (knot: Knot) : Knot * Point option =
                let movedKnot x y v =
                    let position = x + offsetX, y + offsetY
                    let visits = if v |> List.contains position then v else position :: v

                    { knot with
                        position = position
                        visits = visits },
                    Some position

                match knot with
                | { position = x, y; visits = visits } -> movedKnot x y visits

            let moveHead (knot: Knot) : Knot * Point option =
                match direction with
                | Up -> knot |> move 0 1
                | Right -> knot |> move 1 0
                | Down -> knot |> move 0 -1
                | Left -> knot |> move -1 0

            let moveTail (head: Point) (tail: Knot) : Knot * Point option =
                let signedAbsolute (value: int) : int * int =
                    match value with
                    | v when v < 0 -> -1 * v, -1
                    | v when v >= 0 -> v, 1
                    | _ -> failwith "Invalid value!"

                let differences =
                    match head, tail with
                    | (xHead, yHead), { position = (xTail, yTail) } ->
                        (xHead - xTail |> signedAbsolute), (yHead - yTail |> signedAbsolute)

                match differences with
                | (x, offsetX), (y, offsetY) when x > 1 && y > 0 -> tail |> move offsetX offsetY
                | (x, offsetX), (y, offsetY) when x > 0 && y > 1 -> tail |> move offsetX offsetY
                | (x, offsetX), _ when x > 1 -> tail |> move offsetX 0
                | _, (y, offsetY) when y > 1 -> tail |> move 0 offsetY
                | _ -> tail, Some tail.position

            match precedingKnot, knot with
            | None, _ -> knot |> moveHead
            | Some head, tail -> moveTail head tail

        match rope |> List.mapFold moveKnot None with
        | r, _ -> r

    match move.direction, move.amount with
    | _, 0 -> rope
    | direction, _ -> moveMultiRope (rope |> updateKnots direction) { move with amount = move.amount - 1 }

[<Test>]
let PartTwo () =
    Path.Combine("Puzzle09", "input.txt")
    |> lines inMoves
    |> Seq.fold moveMultiRope (List.init 10 (fun _ -> { position = 0, 0; visits = [ 0, 0 ] }))
    |> fun rope -> rope |> List.last |> (fun knot -> knot.visits.Length)
    |> printfn "%A"
