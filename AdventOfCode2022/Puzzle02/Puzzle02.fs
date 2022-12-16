module AdventOfCode2022.Puzzle02.Puzzle02

open System.IO
open NUnit.Framework

let mapRound (round: string) =
    match (round.Split([| ' ' |]) |> List.ofArray) with
    | elf :: me :: _ -> elf, me
    | _ -> failwith "Wrong Input"

let scoreOne state item =
    match item with
    | "A", "X" -> state + 1 + 3 // rock rock
    | "A", "Y" -> state + 2 + 6 // rock paper
    | "A", "Z" -> state + 3 + 0 // rock scissors
    | "B", "Y" -> state + 2 + 3 // paper paper
    | "B", "X" -> state + 1 + 0 // paper rock
    | "B", "Z" -> state + 3 + 6 // paper scissors
    | "C", "Z" -> state + 3 + 3 // scissors scissors
    | "C", "X" -> state + 1 + 6 // scissors rock
    | "C", "Y" -> state + 2 + 0 // scissors paper
    | _ -> failwith "Wrong Input"

[<Test>]
let PartOne () =
    Path.Combine("Puzzle02", "input.txt")
    |> lines mapRound
    |> Seq.fold scoreOne 0
    |> printfn "%d"

// Draw -> Y
// Loose -> X
// Win -> Z
let scoreTwo state item =
    match item with
    | "A", "X" -> state + 3 + 0 // rock scissors
    | "A", "Y" -> state + 1 + 3 // rock rock
    | "A", "Z" -> state + 2 + 6 // rock paper
    | "B", "Y" -> state + 2 + 3 // paper paper
    | "B", "X" -> state + 1 + 0// paper rock
    | "B", "Z" -> state + 3 + 6 // paper scissors
    | "C", "Z" -> state + 1 + 6 // scissors rock
    | "C", "X" -> state + 2 + 0 // scissors paper
    | "C", "Y" -> state + 3 + 3 // scissors scissors
    | _ -> failwith "Wrong Input"

[<Test>]
let PartTwo () =
    Path.Combine("Puzzle02", "input.txt")
    |> lines mapRound
    |> Seq.fold scoreTwo 0
    |> printfn "%d"
