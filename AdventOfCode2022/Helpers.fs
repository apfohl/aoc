module AdventOfCode2022.Helpers

open System.IO

let lines mapper (path: string) =
    seq {
        use reader = new StreamReader(path)

        while not reader.EndOfStream do
            yield reader.ReadLine()
    }
    |> Seq.map mapper