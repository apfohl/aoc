module AdventOfCode2022.Puzzle11.Puzzle11

open System.IO
open NUnit.Framework

type Monkey =
    { items: int array
      operation: int -> int
      test: int -> int }

let parse lines : Map<int, Monkey> =
    let buildMonkey (lines: string array) =
        let number = lines[0] |> parseRegex "(\d+)" |> (fun matches -> int matches[0])

        let items =
            lines[1] |> splitOn ':' |> (fun m -> m[1] |> splitOn ',' |> Array.map int)

        let operation =
            let op =
                function
                | "*" -> (*)
                | "+" -> (+)
                | _ -> failwith "Not supported operator!"

            lines[2]
            |> parseRegex "(\W)\s(\w+)$"
            |> fun matches ->
                match matches[0], matches[1] with
                | operator, value when value[0] |> isDigit -> (op operator) (int value)
                | operator, _ -> fun old -> (op operator) old old

        let test =
            let divisible = lines[3] |> parseRegex "(\d+)" |> (fun matches -> int matches[0])
            let targetTrue = lines[4] |> parseRegex "(\d+)" |> (fun matches -> int matches[0])
            let targetFalse = lines[5] |> parseRegex "(\d+)" |> (fun matches -> int matches[0])

            function
            | value when value % divisible = 0 -> targetTrue
            | _ -> targetFalse

        number,
        { items = items
          operation = operation
          test = test }

    lines |> Seq.chunkBySize 7 |> Seq.map buildMonkey |> Map

[<Test>]
let PartOne () =
    Path.Combine("Puzzle11", "input.txt") |> lines id |> parse |> printfn "%A"
