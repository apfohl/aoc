[<Microsoft.FSharp.Core.AutoOpen>]
module Helpers

open System.IO
open System.Text.RegularExpressions

let lines mapper (path: string) =
    seq {
        use reader = new StreamReader(path)

        while not reader.EndOfStream do
            yield reader.ReadLine()
    }
    |> Seq.map mapper

let parseRegex regex input =
    Regex.Match(input, regex)
    |> fun m -> m.Groups
    |> Seq.skip 1
    |> Seq.map (fun item -> item.Value)
    |> Seq.toArray

let splitOn (c: char) (input: string) = input.Split c

let isDigit c = System.Char.IsDigit c

let loop n f s =
    List.fold (fun state _ -> f state) s [ 0 .. n - 1 ]

let change key mapper table =
    Map.change key (Option.map mapper) table

let notContains value = List.contains value >> not

let flip2 f a b = f b a

let isNotContainedIn list = (flip2 List.contains) list >> not

let flip3 f a b c = f c b a
