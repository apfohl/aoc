using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2021.Puzzle08;

public static class Puzzle08
{
    private record Data(string[] Inputs, string[] Outputs);

    private record Result(int Ones, int Fours, int Sevens, int Eights)
    {
        public Result Add(Result toAdd) =>
            new(Ones: Ones + toAdd.Ones, Fours: Fours + toAdd.Fours, Sevens: Sevens + toAdd.Sevens, Eights: Eights + toAdd.Eights);

        public int Sum() => Ones + Fours + Sevens + Eights;
    }

    [Test]
    public static void PartOne()
    {
        var aggregate =
            Input().Aggregate(new Result(0, 0, 0, 0), (result, i) =>
                result.Add(i.Outputs.Aggregate(new Result(0, 0, 0, 0), (state, value) =>
                    value.Length switch
                    {
                        2 => state with { Ones = state.Ones + 1 },
                        4 => state with { Fours = state.Fours + 1 },
                        3 => state with { Sevens = state.Sevens + 1 },
                        7 => state with { Eights = state.Eights + 1 },
                        _ => state
                    })));

        Assert.Pass(aggregate.Sum().ToString());
    }

    private static IEnumerable<Data> Input() =>
        Helpers.Inputs(Path.Combine("Puzzle08", "input.txt"), line =>
        {
            var parts = line.Split(" | ");
            return new Data(parts[0].Split(' '), parts[1].Split(' '));
        });
}