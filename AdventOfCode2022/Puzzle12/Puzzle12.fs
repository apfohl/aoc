module AdventOfCode2022.Puzzle12.Puzzle12

open System.IO
open NUnit.Framework

type Grid = char[,]

let parseInput path : Grid =
    path |> lines Seq.toArray |> Seq.toArray |> array2D

let find character (grid: Grid) =
    let rec search r c =
        match r, c with
        | _ when c >= grid.GetLength 1 -> None
        | _ when r >= grid.GetLength 0 -> search 0 (c + 1)
        | _ when grid[r, c] = character -> Some(r, c)
        | _ -> search (r + 1) c

    search 0 0

let distance (lhs: char) (rhs: char) =
    let delta =
        match lhs, rhs with
        | 'S', r -> (int 'a') - (int r)
        | l, 'S' -> (int l) - (int 'a')
        | l, r -> (int l) - (int r)

    delta |> abs

let neighbors (x, y) (grid: Grid) =
    [ if x - 1 >= 0 && distance grid[x, y] grid[x - 1, y] <= 1 then
          yield x - 1, y
      if x + 1 < grid.GetLength 0 && distance grid[x, y] grid[x + 1, y] <= 1 then
          yield x + 1, y
      if y - 1 >= 0 && distance grid[x, y] grid[x, y - 1] <= 1 then
          yield x, y - 1
      if y + 1 < grid.GetLength 1 && distance grid[x, y] grid[x, y + 1] <= 1 then
          yield x, y + 1 ]

let rec trace grid visited (x, y) =
    let neighs = grid |> neighbors (x, y) |> List.filter (isNotContainedIn visited)

    match grid[x, y], neighs with
    | 'E', _
    | _, [] -> visited
    | _ -> neighs |> List.fold (trace grid) ((x, y) :: visited)

[<Test>]
let PartOne () =
    let grid = Path.Combine("Puzzle12", "debug.txt") |> parseInput
    let (Some start) = grid |> find 'S'

    grid |> (flip3 trace) start [ (0, 0) ] |> List.length |> printfn "%A"
