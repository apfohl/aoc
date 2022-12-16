module AdventOfCode2022.Puzzle04.Puzzle04

open System.IO
open NUnit.Framework

let splitArea (area: string) =
    match area.Split [| '-' |] with
    | [| left; right |] -> (int left), (int right)
    | _ -> failwith "Wrong Input"

let asAreas (area: string) =
    area.Split [| ',' |]
    |> List.ofArray
    |> List.map splitArea

let findFullIncludedAreas t =
    match t with
    | (leftA, leftB) :: (rightA, rightB) :: _ when leftA >= rightA && leftB <= rightB -> 1
    | (leftA, leftB) :: (rightA, rightB) :: _ when rightA >= leftA && rightB <= leftB -> 1
    | _ -> 0

[<Test>]
let PartOne () =
    Path.Combine("Puzzle04", "input.txt")
    |> lines asAreas
    |> Seq.map findFullIncludedAreas
    |> Seq.sum
    |> printfn "%A"

let findOverlappingAreas t =
    match t with
    | (leftA, leftB) :: (rightA, rightB) :: _ when leftA >= rightA && leftB <= rightB -> 1
    | (leftA, leftB) :: (rightA, rightB) :: _ when rightA >= leftA && rightB <= leftB -> 1
    | (leftA, _) :: (rightA, rightB) :: _ when leftA >= rightA && leftA <= rightB -> 1
    | (_, leftB) :: (rightA, rightB) :: _ when leftB >= rightA && leftB <= rightB -> 1
    | (leftA, leftB) :: (rightA, _) :: _ when rightA >= leftA && rightA <= leftB -> 1
    | (leftA, leftB) :: (_, rightB) :: _ when rightB >= leftA && rightB <= leftB -> 1
    | _ -> 0

[<Test>]
let PartTwo () =
    Path.Combine("Puzzle04", "input.txt")
    |> lines asAreas
    |> Seq.map findOverlappingAreas
    |> Seq.sum
    |> printfn "%A"
