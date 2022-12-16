module AdventOfCode2022.Puzzle11.Puzzle11

open System.IO
open NUnit.Framework

type Monkey =
    { items: int list
      operation: int -> int
      test: int -> int
      count: int }

let parse lines : Map<int, Monkey> =
    let buildMonkey (lines: string array) =
        let number = lines[0] |> parseRegex "(\d+)" |> (fun matches -> int matches[0])

        let items =
            lines[1]
            |> splitOn ':'
            |> (fun m -> m[1] |> splitOn ',' |> Array.map int |> List.ofArray)

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
            let toInt (matches: string array) = int matches[0]
            let divisible = lines[3] |> parseRegex "(\d+)" |> toInt
            let targetTrue = lines[4] |> parseRegex "(\d+)" |> toInt
            let targetFalse = lines[5] |> parseRegex "(\d+)" |> toInt

            function
            | value when value % divisible = 0 -> targetTrue
            | _ -> targetFalse

        number,
        { items = items
          operation = operation
          test = test
          count = 0 }

    lines |> Seq.chunkBySize 7 |> Seq.map buildMonkey |> Map

let clear key monkeys =
    change
        key
        (fun monkey ->
            { monkey with
                items = []
                count = monkey.count + List.length monkey.items })
        monkeys

let receive monkeys key item =
    change key (fun monkey -> { monkey with items = item :: monkey.items }) monkeys

let monkey (monkeys: Map<int, Monkey>) key =
    let { items = items
          operation = operation
          test = test } =
        monkeys[key]

    let divide divisor (x: int) = int (int64 x / divisor)

    let target = operation >> divide 3L >> test

    let throwItem (monkeys: Map<int, Monkey>) (item: int) : Map<int, Monkey> = receive monkeys (target item) item

    items |> List.fold throwItem monkeys |> clear key

let round monkeys =
    Seq.fold monkey monkeys (Map.keys monkeys)

[<Test>]
let PartOne () =
    Path.Combine("Puzzle11", "input.txt")
    |> lines id
    |> parse
    |> loop 20 round
    |> Map.values
    |> Seq.map (fun monkey -> monkey.count)
    |> Seq.sortDescending
    |> Seq.take 2
    |> Seq.reduce (*)
    |> printfn "%A"
