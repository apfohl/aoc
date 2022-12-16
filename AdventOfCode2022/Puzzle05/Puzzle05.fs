module AdventOfCode2022.Puzzle05.Puzzle05

open System
open System.IO
open NUnit.Framework

type Move =
    { crates: int
      origin: int
      destination: int }

let inMoves (line: string) =
    match line.Split [| ' ' |] with
    | [| _; crates; _; origin; _; destination |] ->
        { crates = int crates
          origin = int origin - 1
          destination = int destination - 1 }
    | _ -> failwith "Wrong line"

let moveSingle (state: char list list) origin destination =
    let findOrigin (state: char list) (list: int * char list) =
        match list with
        | i, x :: xs when i = origin -> xs, x :: state
        | _, l -> l, state

    let findDestination (state: char list) (list: int * char list) =
        match list with
        | i, l when i = destination -> List.concat [ state; l ], []
        | _, l -> l, state

    let mapped =
        state
        |> List.indexed
        |> List.mapFold findOrigin []
        |> fun tuple ->
            match tuple with
            | l, state -> List.indexed l, state
        |> fun tuple ->
            match tuple with
            | l, state -> List.mapFold findDestination state l

    match mapped with
    | list, _ -> list

let rec applyMove state move =
    match move with
    | { Move.crates = crates } when crates = 0 -> state
    | { Move.crates = crates
        Move.origin = origin
        Move.destination = destination } when crates > 0 ->
        applyMove (moveSingle state origin destination) { move with crates = crates - 1 }
    | _ -> failwith "Wrong input"

// [ [ 'N'; 'Z' ]
//   [ 'D'; 'C'; 'M' ]
//   [ 'P' ] ]

[<Test>]
let PartOne () =
    Path.Combine("Puzzle05", "input.txt")
    |> lines inMoves
    |> Seq.fold
        applyMove
        [ [ 'Q'
            'G'
            'P'
            'R'
            'L'
            'C'
            'T'
            'F' ]
          [ 'J'
            'S'
            'F'
            'R'
            'W'
            'H'
            'Q'
            'N' ]
          [ 'Q'; 'M'; 'P'; 'W'; 'H'; 'B'; 'F' ]
          [ 'F'; 'D'; 'T'; 'S'; 'V' ]
          [ 'Z'; 'F'; 'V'; 'W'; 'D'; 'L'; 'Q' ]
          [ 'S'; 'L'; 'C'; 'Z' ]
          [ 'F'; 'D'; 'V'; 'M'; 'B'; 'Z' ]
          [ 'B'; 'J'; 'T' ]
          [ 'H'
            'P'
            'S'
            'L'
            'G'
            'B'
            'N'
            'Q' ] ]
    |> Seq.fold
        (fun state item ->
            match item with
            | x :: _ -> x :: state
            | _ -> failwith "Wrong input")
        []
    |> List.rev
    |> List.toArray
    |> String
    |> printfn "%A"

// --------------------------------------------------------

let rec pick amount buffer stack =
    match stack with
    | [] -> stack, buffer
    | _ when amount = 0 -> stack, buffer
    | c :: s -> pick (amount - 1) (c :: buffer) s

[<Test>]
let testPick () =
    [ 'A'; 'B'; 'C'; 'D'; 'E' ]
    |> pick 3 []
    |> printfn "%A"

let rec place stack buffer =
    match buffer with
    | [] -> stack, buffer
    | c :: b -> place (c :: stack) b

[<Test>]
let testPlace () =
    [ 'C'; 'B'; 'A' ]
    |> place [ 'D'; 'E' ]
    |> printfn "%A"

let buffer (_, buffer) = buffer

let stack (stack, _) = stack

[<Test>]
let testPickAndPlace () =
    [ 'A'; 'B'; 'C'; 'D'; 'E' ]
    |> pick 3 []
    |> buffer
    |> place [ 'D'; 'E' ]
    |> stack
    |> printfn "%A"

let apply state (move: Move) =
    let pickUpFromOrigin (buffer: char list) (stack: int * char list) =
        match stack with
        | i, s when i = move.origin -> pick move.crates buffer s
        | _, s -> s, buffer

    let placeAtDestination (stacks: (int * char list) list, buffer: char list) =
        let placer (buffer: char list) (stack: int * char list) =
            match stack with
            | i, s when i = move.destination -> place s buffer
            | _, s -> s, buffer

        stacks
        |> List.mapFold placer buffer

    state
    |> List.indexed
    |> List.mapFold pickUpFromOrigin []
    |> fun tuple ->
        match tuple with
        | stacks, buffer -> stacks |> List.indexed, buffer
    |> placeAtDestination
    |> stack

[<Test>]
let PartTwo () =
    Path.Combine("Puzzle05", "input.txt")
    |> lines inMoves
    |> Seq.fold
        apply
        [ [ 'Q'
            'G'
            'P'
            'R'
            'L'
            'C'
            'T'
            'F' ]
          [ 'J'
            'S'
            'F'
            'R'
            'W'
            'H'
            'Q'
            'N' ]
          [ 'Q'; 'M'; 'P'; 'W'; 'H'; 'B'; 'F' ]
          [ 'F'; 'D'; 'T'; 'S'; 'V' ]
          [ 'Z'; 'F'; 'V'; 'W'; 'D'; 'L'; 'Q' ]
          [ 'S'; 'L'; 'C'; 'Z' ]
          [ 'F'; 'D'; 'V'; 'M'; 'B'; 'Z' ]
          [ 'B'; 'J'; 'T' ]
          [ 'H'
            'P'
            'S'
            'L'
            'G'
            'B'
            'N'
            'Q' ] ]
        //[ [ 'N'; 'Z' ]
        //  [ 'D'; 'C'; 'M' ]
        //  [ 'P' ] ]
    |> Seq.fold
        (fun state item ->
            match item with
            | x :: _ -> x :: state
            | _ -> failwith "Wrong input")
        []
    |> List.rev
    |> List.toArray
    |> String
    |> printfn "%A"
