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
