using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2021.Puzzle02;

public static class Puzzle02
{
    private enum Direction
    {
        Forward,
        Down,
        Up
    }

    private record StateOne(int Horizontal, int Depth);

    [Test]
    public static void PartOne()
    {
        var (horizontal, depth) = Inputs().Aggregate(new StateOne(0, 0), (state, step) => step.Direction switch
        {
            Direction.Forward => state with { Horizontal = state.Horizontal + step.Amount },
            Direction.Down => state with { Depth = state.Depth + step.Amount },
            Direction.Up => state with { Depth = state.Depth - step.Amount },
            _ => throw new ArgumentOutOfRangeException()
        });

        Assert.Pass((horizontal * depth).ToString());
    }

    private record StateTwo(int Horizontal, int Depth, int Aim);

    [Test]
    public static void PartTwo()
    {
        var (horizontal, depth, _) = Inputs().Aggregate(new StateTwo(0, 0, 0), (state, step) =>
            step.Direction switch
            {
                Direction.Forward => state with
                {
                    Horizontal = state.Horizontal + step.Amount,
                    Depth = state.Depth + state.Aim * step.Amount
                },
                Direction.Down => state with { Aim = state.Aim + step.Amount },
                Direction.Up => state with { Aim = state.Aim - step.Amount },
                _ => throw new ArgumentOutOfRangeException()
            });

        Assert.Pass((horizontal * depth).ToString());
    }

    private static IEnumerable<(Direction Direction, int Amount)> Inputs() =>
        Helpers.Inputs(Path.Combine("Puzzle02", "input.txt"), Parse);

    private static (Direction Direction, int Amount) Parse(string line)
    {
        var inputs = line.Split(' ');
        return (Enum.Parse<Direction>(inputs[0].CapitalCase()), int.Parse(inputs[1]));
    }
}