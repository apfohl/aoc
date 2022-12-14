module AdventOfCode2022.Puzzle10.Puzzle10

open System.IO
open AdventOfCode2022.Helpers
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
