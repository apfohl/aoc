module AdventOfCode2022.Puzzle10.Puzzle10

open System.IO
open NUnit.Framework

type Instruction =
    | NOOP
    | ADDX of int

let toInstruction (line: string) : Instruction =
    match line.Split [| ' ' |] with
    | [| _ |] -> NOOP
    | [| _; second |] -> ADDX(int second)
    | line -> failwith $"Line could no be parsed: '{line}'"

type Cpu =
    { register: int
      cycles: int
      signals: int list }

let rec cycle (cpu: Cpu) (apply: int -> int) (count: int) : Cpu =
    let currentCycle = cpu.cycles + 1

    let signals =
        if currentCycle = 20 || (currentCycle - 20) % 40 = 0 then
            (cpu.register * currentCycle) :: cpu.signals
        else
            cpu.signals

    match count with
    | 0 -> { cpu with register = cpu.register |> apply }
    | c ->
        cycle
            { cpu with
                cycles = currentCycle
                signals = signals }
            apply
            (c - 1)

let execute (cpu: Cpu) (instruction: Instruction) : Cpu =
    match instruction with
    | NOOP -> cycle cpu id 1
    | ADDX i -> cycle cpu (fun r -> r + i) 2

let runProgram (instructions: Instruction seq) : Cpu =
    instructions
    |> Seq.fold
        execute
        { register = 1
          cycles = 0
          signals = [] }

[<Test>]
let PartOne () =
    Path.Combine("Puzzle10", "input.txt")
    |> lines toInstruction
    |> runProgram
    |> fun cpu -> cpu.signals |> List.sum
    |> printfn "%A"

type Buffer = char list list

type Screen =
    { buffer: Buffer
      sprite: int
      cycles: int }

let rec drawCycle (screen: Screen) (apply: int -> int) (count: int) : Screen =
    let isWithinSprite pixel =
        pixel >= screen.sprite - 1 && pixel <= screen.sprite + 1

    let draw pixel =
        if pixel |> isWithinSprite then '#' else '.'

    let currentCycle = screen.cycles + 1

    let changedBuffer: Buffer =
        match screen.buffer, screen.cycles % 40 with
        | [], _ -> [ [ draw 0 ] ]
        | rows, 0 -> [ draw 0 ] :: rows
        | row :: rows, pixel -> (draw pixel :: row) :: rows

    match count with
    | 0 -> { screen with sprite = screen.sprite |> apply }
    | c ->
        drawCycle
            { screen with
                cycles = currentCycle
                buffer = changedBuffer }
            apply
            (c - 1)

let draw (screen: Screen) (instruction: Instruction) : Screen =
    match instruction with
    | NOOP -> drawCycle screen id 1
    | ADDX i -> drawCycle screen (fun r -> r + i) 2

let render (instructions: Instruction seq) : Screen =
    instructions |> Seq.fold draw { buffer = []; sprite = 1; cycles = 0 }

let printScreen (screen: Screen) =
    screen.buffer
    |> List.map (fun row -> row |> List.rev |> List.map string |> List.reduce (+))
    |> List.rev
    |> List.iter (printfn "%s")

[<Test>]
let PartTwo () =
    Path.Combine("Puzzle10", "input.txt")
    |> lines toInstruction
    |> render
    |> printScreen
