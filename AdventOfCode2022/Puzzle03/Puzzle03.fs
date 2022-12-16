module AdventOfCode2022.Puzzle03.Puzzle03

open System.IO
open NUnit.Framework

let mapCompartments (rucksack: string) =
    let compartmentLength = rucksack.Length / 2
    Seq.take compartmentLength rucksack, Seq.skip compartmentLength rucksack

let doubledItem (compartments: char seq * char seq) =
    match compartments with
    | left, right ->
        Seq.fold2
            (fun state l r ->
                if Seq.contains r left then r
                else if Seq.contains l right then l
                else state)
            '_'
            left
            right

let priority symbol =
    Seq.findIndex
        (fun item -> item = symbol)
        (Seq.concat [ [ 'a' .. 'z' ]
                      [ 'A' .. 'Z' ] ])
    |> fun v -> v + 1

[<Test>]
let PartOne () =
    Path.Combine("Puzzle03", "input.txt")
    |> lines mapCompartments
    |> Seq.map doubledItem
    |> Seq.map priority
    |> Seq.sum
    |> printfn "%d"

let toSting (chars: char []) = new string (chars)

let badge group =
    let reducer (first: string) (second: string) =
        first
        |> Seq.filter second.Contains
        |> Seq.toArray
        |> toSting

    group |> Seq.reduce reducer |> Seq.last

let groups backpacks =
    let group3 (state: list<list<string>>) (item: string) =
        match state, item with
        | [], _ -> [ [ item ] ]
        | x :: xs, _ when x.Length < 3 -> (item :: x) :: xs
        | x :: _, _ when x.Length = 3 -> [ item ] :: state
        | _ -> failwith "Wrong input"

    backpacks |> Seq.fold group3 []

[<Test>]
let PartTwo () =
    Path.Combine("Puzzle03", "input.txt")
    |> lines id
    |> Seq.toList
    |> groups
    |> Seq.map badge
    |> Seq.map priority
    |> Seq.sum
    |> printfn "%A"
