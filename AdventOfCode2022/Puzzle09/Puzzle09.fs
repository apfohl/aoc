module AdventOfCode2022.Puzzle09.Puzzle09

open System.IO
open AdventOfCode2022.Helpers
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
    | [| direction; amount |] ->
        { direction = toDirection direction
          amount = int amount }
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
          tailPositions = [(0,0)] }
    |> fun state -> List.length state.tailPositions
    |> printfn "%A"
