using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2021.Puzzle01;

public static class Puzzle01
{
    private record StateOne(int Increased, int Decreased, int Last);

    [Test]
    public static void PartOne()
    {
        var aggregate = Inputs()
            .Aggregate(new StateOne(0, 0, 0), (state, i) =>
                i switch
                {
                    _ when state.Last == 0 || state.Last == i => state with { Last = i },
                    _ when i > state.Last => state with { Increased = state.Increased + 1, Last = i },
                    _ when i < state.Last => state with { Decreased = state.Decreased + 1, Last = i },
                    _ => throw new Exception()
                });

        Assert.Pass(aggregate.ToString());
    }

    private record StateTwo(int Increased, int Decreased, int Last, List<int> Window);

    [Test]
    public static void PartTwo()
    {
        var aggregate = Inputs()
            .Concat(new[] { 0 })
            .Aggregate(new StateTwo(0, 0, 0, new List<int>()),
                (state, i) =>
                    i switch
                    {
                        _ when state.Window.Count < 3 => state with { Window = new List<int>(state.Window) { i } },
                        _ when state.Window.Count == 3 &&
                               (state.Last == 0 || state.Last == state.Window.Sum()) => state
                            with
                            {
                                Last = state.Window.Sum(),
                                Window = state.Window.Slide(i)
                            },
                        _ when state.Window.Count == 3 && state.Window.Sum() > state.Last => state with
                        {
                            Increased = state.Increased + 1,
                            Last = state.Window.Sum(),
                            Window = state.Window.Slide(i)
                        },
                        _ when state.Window.Count == 3 && state.Window.Sum() < state.Last => state with
                        {
                            Decreased = state.Decreased + 1,
                            Last = state.Window.Sum(),
                            Window = state.Window.Slide(i)
                        },
                        _ => throw new Exception()
                    });

        Assert.Pass(aggregate.ToString());
    }

    private static IEnumerable<int> Inputs() =>
        Helpers.Inputs(Path.Combine("Puzzle01", "input.txt"), int.Parse);
        
    private static List<int> Slide(this List<int> list, int i)
    {
        var range = list.GetRange(1, list.Count - 1);
        range.Add(i);
        return range;
    }
}