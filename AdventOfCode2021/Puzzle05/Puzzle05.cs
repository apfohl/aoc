using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2021.Puzzle05;

public static class Puzzle05
{
    private sealed record Line((int x, int y) Start, (int x, int y) End);

    [Test]
    public static void PartOne()
    {
        var result = new Dictionary<(int x, int y), int>();

        Input().Select(Interpolate).ToList()
            .ForEach(points => points.ToList()
                .ForEach(point => result.AddOrIncrement(point)));

        Assert.Pass(result.Where(pair => pair.Value > 1).ToArray().Length.ToString());
    }

    [Test]
    public static void PartTwo()
    {
        var result = new Dictionary<(int x, int y), int>();

        Input().Select(InterpolateTwo).ToList()
            .ForEach(points => points.ToList()
                .ForEach(point => result.AddOrIncrement(point)));

        Assert.Pass(result.Where(pair => pair.Value > 1).ToArray().Length.ToString());
    }

    private static IEnumerable<(int x, int y)> Interpolate(Line line)
    {
        var (start, end) = line;

        if (start.x == end.x)
        {
            var c = start.y < end.y;
            for (var y = c.If(start.y, start.y);
                 c.If(y <= end.y, y >= end.y);
                 c.If(() => y++, () => y--))
            {
                yield return (start.x, y);
            }
        }
        else if (start.y == end.y)
        {
            var c = start.x < end.x;
            for (var x = c.If(start.x, start.x);
                 c.If(x <= end.x, x >= end.x);
                 c.If(() => x++, () => x--))
            {
                yield return (x, start.y);
            }
        }
    }

    private static IEnumerable<(int x, int y)> InterpolateTwo(Line line)
    {
        var (start, end) = line;

        if (start.x == end.x)
        {
            var c = start.y < end.y;
            for (var y = c.If(start.y, start.y);
                 c.If(y <= end.y, y >= end.y);
                 c.If(() => y++, () => y--))
            {
                yield return (start.x, y);
            }
        }
        else if (start.y == end.y)
        {
            var c = start.x < end.x;
            for (var x = c.If(start.x, start.x);
                 c.If(x <= end.x, x >= end.x);
                 c.If(() => x++, () => x--))
            {
                yield return (x, start.y);
            }
        }
        else
        {
            if (start.x < end.x && start.y < end.y)
            {
                for (var i = (start.x, start.y); i.x <= end.x; i = (i.x + 1, i.y + 1))
                {
                    yield return (i.x, i.y);
                }
            }
            else if (start.x > end.x && start.y < end.y)
            {
                for (var i = (start.x, start.y); i.y <= end.y; i = (i.x - 1, i.y + 1))
                {
                    yield return (i.x, i.y);
                }
            }
            else if (start.x > end.x && start.y > end.y)
            {
                for (var i = (start.x, start.y); i.x >= end.x; i = (i.x - 1, i.y - 1))
                {
                    yield return (i.x, i.y);
                }
            }
            else if (start.x < end.x && start.y > end.y)
            {
                for (var i = (start.x, start.y); i.x <= end.x; i = (i.x + 1, i.y - 1))
                {
                    yield return (i.x, i.y);
                }
            }
        }
    }


    private static void AddOrIncrement(this IDictionary<(int x, int y), int> dictionary, (int x, int y) key)
    {
        if (dictionary.TryGetValue(key, out var existing))
        {
            dictionary[key] = existing + 1;
        }
        else
        {
            dictionary.Add(key, 1);
        }
    }

    private static T If<T>(this bool condition, T onTrue, T onFalse) => condition ? onTrue : onFalse;

    private static bool If(this bool condition, bool onTrue, bool onFalse) => condition ? onTrue : onFalse;

    private static void If(this bool condition, Action onTrue, Action onFalse)
    {
        if (condition)
        {
            onTrue();
        }
        else
        {
            onFalse();
        }
    }

    private static IEnumerable<Line> Input() =>
        Helpers.Inputs(Path.Combine("Puzzle05", "input.txt"), s =>
        {
            var coordinates = s.Split(" -> ").Select(p =>
            {
                var c = p.Split(',');
                return (int.Parse(c[0]), int.Parse(c[1]));
            }).ToArray();

            return new Line(coordinates[0], coordinates[1]);
        });
}