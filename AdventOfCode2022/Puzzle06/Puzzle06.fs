module AdventOfCode2022.Puzzle06.Puzzle06

open System.IO
open NUnit.Framework

let getPosition windowSize =
    let findMarker state character =
        match character with
        | index, window when (window |> Array.distinct |> Array.length) = windowSize && state = 0 -> index
        | _ -> state

    Path.Combine("Puzzle06", "input.txt")
    |> lines id
    |> Seq.head
    |> Seq.windowed windowSize
    |> Seq.indexed
    |> Seq.fold findMarker 0
    |> fun i -> i + windowSize
    |> printfn "%d"

[<Test>]
let PartOne () =
    getPosition 4

[<Test>]
let PartTwo () =
    getPosition 14
