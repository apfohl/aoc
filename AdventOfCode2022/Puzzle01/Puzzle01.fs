module AdventOfCode2022.Puzzle01.Puzzle01

open System.IO
open NUnit.Framework

let groupCalories state item =
    match item with
    | "" -> 0 :: state
    | _ ->
        match state with
        | [] -> int item :: state
        | head :: tail ->
            match head with
            | 0 -> int item :: tail
            | _ -> head + (int item) :: tail

[<Test>]
let PartOne () =
    Path.Combine("Puzzle01", "input.txt")
    |> lines id
    |> Seq.fold groupCalories []
    |> Seq.max
    |> printfn "%d"

[<Test>]
let PartTwo () =
    Path.Combine("Puzzle01", "input.txt")
    |> lines id
    |> Seq.fold groupCalories []
    |> Seq.sortDescending
    |> Seq.take 3
    |> Seq.sum
    |> printfn "%d"
