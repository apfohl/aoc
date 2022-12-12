using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace AdventOfCode2021.Puzzle06;

public static class Puzzle06
{
    [TestCase(80)]
    public static void PartOne(int days)
    {
        var fish = Input().ToList();

        var daysLeft = days;
        var length = fish.Count;
        var i = 0;
        while (i < length && daysLeft > 0)
        {
            if (fish[i] == 0)
            {
                fish[i] = 6;
                fish.Add(8);
            }
            else
            {
                fish[i] -= 1;
            }
                
            if (i == length - 1)
            {
                length = fish.Count;
                i = 0;
                daysLeft--;
            }
            else
            {
                i++;
            }
        }

        Assert.Pass(fish.Count.ToString());
    }

    [Test]
    public static void PartTwo()
    {
        var fish = Input().Aggregate(new ulong[9], (state, i) =>
        {
            state[i] += 1;
            return state;
        });

        var evolution = Enumerable.Range(1, 256).Aggregate(fish, (state, _) =>
        {
            state[7] += state[0];

            return state.Rotate();
        });

        var result = evolution.Aggregate(0ul, (state, i) => state + i);

        Assert.Pass(result.ToString());
    }

    private static IEnumerable<int> Input() =>
        Helpers
            .Inputs(Path.Combine("Puzzle06", "input.txt"), s => s.Split(',').Select(int.Parse))
            .SelectMany(i => i);

    private static T[] Rotate<T>(this T[] array)
    {
        var result = new T[array.Length];
            
        var buffer = new T[1];
        Array.Copy(array, buffer, 1);
        Array.Copy(array, 1, result, 0, array.Length - 1);
        Array.Copy(buffer, 0, result, array.Length - 1, 1);

        return result;
    }
}