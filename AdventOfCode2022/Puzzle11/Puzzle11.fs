module AdventOfCode2022.Puzzle11.Puzzle11

open System.IO
open NUnit.Framework

type Monkey =
    { items: int64 list
      operation: int64 -> int64
      divisible: int64
      test: int64 -> int
      count: int64 }

let parse lines : Map<int, Monkey> =
    let buildMonkey (lines: string array) =
        let number = lines[0] |> parseRegex "(\d+)" |> (fun matches -> int matches[0])

        let items =
            lines[1]
            |> splitOn ':'
            |> (fun m -> m[1] |> splitOn ',' |> Array.map int64 |> List.ofArray)

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
                | operator, value when value[0] |> isDigit -> (op operator) (int64 value)
                | operator, _ -> fun old -> (op operator) old old

        let int64 (matches: string array) = int64 matches[0]
        let divisible = lines[3] |> parseRegex "(\d+)" |> int64

        let test =
            let int (matches: string array) = int matches[0]
            let targetTrue = lines[4] |> parseRegex "(\d+)" |> int
            let targetFalse = lines[5] |> parseRegex "(\d+)" |> int

            function
            | value when value % divisible = 0L -> targetTrue
            | _ -> targetFalse

        number,
        { items = items
          operation = operation
          divisible = divisible
          test = test
          count = 0 }

    lines |> Seq.chunkBySize 7 |> Seq.map buildMonkey |> Map

let clear key monkeys =
    change
        key
        (fun monkey ->
            { monkey with
                items = []
                count = monkey.count + int64 (List.length monkey.items) })
        monkeys

let receive monkeys key item =
    change key (fun monkey -> { monkey with items = monkey.items @ [ item ] }) monkeys

let monkey f (monkeys: Map<int, Monkey>) key =
    let { items = items
          operation = operation
          test = test } =
        monkeys[key]

    let changeWorryLevel = operation >> f
    let target = changeWorryLevel >> test

    let throwItem monkeys item =
        receive monkeys (target item) (changeWorryLevel item)

    items |> List.fold throwItem monkeys |> clear key

let divideBy divisor x = x / divisor

let round f monkeys =
    Seq.fold (monkey f) monkeys (Map.keys monkeys)

[<Test>]
let PartOne () =
    let f = divideBy 3L

    Path.Combine("Puzzle11", "input.txt")
    |> lines id
    |> parse
    |> loop 20 (round f)
    |> Map.values
    |> Seq.map (fun monkey -> monkey.count)
    |> Seq.sortDescending
    |> Seq.take 2
    |> Seq.reduce (*)
    |> printfn "%A"

[<Test>]
let PartTwo () =
    let divisibleProduct =
        Map.values >> Seq.map (fun monkey -> monkey.divisible) >> Seq.fold (*) 1L

    let modByDivisible monkeys input = input % (divisibleProduct monkeys)

    let monkeys = Path.Combine("Puzzle11", "input.txt") |> lines id |> parse

    let f = modByDivisible monkeys

    monkeys
    |> loop 10000 (round f)
    |> Map.values
    |> Seq.map (fun monkey -> monkey.count)
    |> Seq.sortDescending
    |> Seq.take 2
    |> Seq.reduce (*)
    |> printfn "%A"
