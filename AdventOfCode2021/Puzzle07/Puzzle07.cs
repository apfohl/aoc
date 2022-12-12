using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2021.Puzzle07;

public static class Puzzle07
{
    [Test]
    public static void PartOne()
    {
        var positions = Input().ToList();

        var fuel = Enumerable
            .Range(positions.Min(), positions.Max() + 1)
            .Select(p => positions.Aggregate(0, (state, i) => state + Math.Abs(p - i)))
            .Prepend(int.MaxValue)
            .Min();

        Assert.Pass(fuel.ToString());
    }

    [Test]
    public static void PartTwo()
    {
        var positions = Input().ToList();

        var fuel = Enumerable
            .Range(positions.Min(), positions.Max() + 1)
            .Select(p => positions.Aggregate(0, (state, i) => state + Math.Abs(p - i).Expand()))
            .Prepend(int.MaxValue)
            .Min();

        Assert.Pass(fuel.ToString());
    }

    [Test]
    public static void TestExpand() => 
        Assert.AreEqual(15, 5.Expand());

    private static int Expand(this int fuel) =>
        Enumerable.Range(0, fuel + 1).Aggregate(0, (state, i) => state + i);

    private static IEnumerable<int> Input() =>
        Helpers
            .Inputs(Path.Combine("Puzzle07", "input.txt"), s => s.Split(',').Select(int.Parse))
            .SelectMany(i => i);
}