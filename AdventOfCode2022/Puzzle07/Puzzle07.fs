module AdventOfCode2022.Puzzle07.Puzzle07

open System.IO
open NUnit.Framework

type Node = { name: string; content: Content }

and Content =
    | Directory of children: Node list
    | File of size: int

let rec insert (path: string list) (child: Node) (tree: Node) : Node =
    let mergeAndRemove (path: string list) (nodes: Node list) (index: int, node: Node) : Node list =
        (insert path child node) :: (List.removeAt index nodes)

    match path, tree.content with
    | current :: _, Directory children when tree.name = current && path.Length = 1 ->
        { tree with content = Directory(child :: children) }
    | _ :: next :: tail, Directory children ->
        List.indexed children
        |> List.find (fun (_, c) -> c.name = next)
        |> mergeAndRemove (next :: tail) children
        |> fun c -> { tree with content = Directory(c) }
    | _ -> failwith "Wrong type"

[<Test>]
let TestInsertDirectoryChild () =
    { name = "/"
      content =
        Directory(
            [ { name = "readme.md"
                content = File(4096) }
              { name = "files"
                content = Directory([]) } ]
        ) }
    |> insert
        [ "/" ]
        { name = "LICENSE"
          content = File(8192) }
    |> insert
        [ "/"; "files" ]
        { name = "photo.jpg"
          content = File(8192) }
    |> insert
        [ "/"; "files" ]
        { name = "birds"
          content = Directory([]) }
    |> insert
        [ "/"; "files"; "birds" ]
        { name = "eagle.jpg"
          content = File(1234) }
    |> printfn "%A"

type ParseMode =
    | Command
    | Listing

type ParserState =
    { mode: ParseMode
      tree: Node
      path: string list }

let changeDirectory (state: ParserState) (directory: string) : ParserState =
    match state.path, directory with
    | path, ".." -> { state with path = path[.. path.Length - 2] }
    | _, "/" -> { state with path = [ "/" ] }
    | path, d -> { state with path = List.rev path |> fun l -> d :: l |> List.rev }

let createDirectory (node: Node) (path: string list) (directory: string) : Node =
    node
    |> insert
        path
        { name = directory
          content = Directory([]) }

let createFile (node: Node) (path: string list) (size: int) (file: string) : Node =
    node |> insert path { name = file; content = File(size) }

let rec parseLine (state: ParserState) (line: string) =
    match line.Split [| ' ' |] with
    | [| "$"; "cd"; directory |] when state.mode = Command -> changeDirectory state directory
    | [| "$"; "ls" |] when state.mode = Command -> { state with mode = Listing }
    | [| "dir"; directory |] when state.mode = Listing ->
        { state with tree = (createDirectory state.tree state.path directory) }
    | [| size; file |] when state.mode = Listing ->
        { state with tree = (createFile state.tree state.path (int size) file) }
    | _ when line.StartsWith("$") && state.mode = Listing -> parseLine { state with mode = Command } line
    | _ -> failwith $"Can not parse line: '{line}'"

type AtMostState = { absolute: int; result: int list }

let rec atMost (max: int) (source: Node) : AtMostState =
    let folder state node =
        let c = atMost max node

        match node.content with
        | Directory _ when c.absolute <= max ->
            { state with
                absolute = state.absolute + c.absolute
                result = c.absolute :: List.concat [ state.result; c.result ] }
        | Directory _ when c.absolute > max ->
            { state with
                absolute = state.absolute + c.absolute
                result = List.concat [ state.result; c.result ] }
        | _ -> { state with absolute = state.absolute + c.absolute }

    match source.content with
    | File size -> { absolute = size; result = [] }
    | Directory [] -> { absolute = 0; result = [] }
    | Directory children -> List.fold folder { absolute = 0; result = [] } children

[<Test>]
let TestAtMost () =
    { name = "/"
      content =
        Directory(
            [ { name = "readme.md"
                content = File(4096) }
              { name = "files"
                content =
                  Directory(
                      [ { name = "readme.md"
                          content = File(4096) } ]
                  ) } ]
        ) }
    |> atMost 100000
    |> printfn "%A"

[<Test>]
let PartOne () =
    Path.Combine("Puzzle07", "input.txt")
    |> lines id
    |> Seq.fold
        parseLine
        { mode = Command
          tree = { name = "/"; content = Directory([]) }
          path = [] }
    |> fun parserState ->
        match parserState with
        | { ParserState.tree = tree } -> tree
    |> atMost 70000000
    |> fun state ->
        match state with
        | { AtMostState.result = result } -> result
    |> List.sum
    |> printfn "%A"

[<Test>]
let PartTwo () =
    Path.Combine("Puzzle07", "input.txt")
    |> lines id
    |> Seq.fold
        parseLine
        { mode = Command
          tree = { name = "/"; content = Directory([]) }
          path = [] }
    |> fun parserState ->
        match parserState with
        | { ParserState.tree = tree } -> tree
    |> atMost 70000000
    |> fun state ->
        match state with
        | { AtMostState.absolute = absolute
            AtMostState.result = result } -> absolute, result |> List.sortDescending
    |> printfn "%A"
