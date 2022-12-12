using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonadicBits;
using NUnit.Framework;

namespace AdventOfCode2021.Puzzle03;

using static Functional;

public static class Puzzle03
{
    private record ColumnCount(int Zeros, int Ones);

    private record Counter(Maybe<ColumnCount[]> ColumnCounts)
    {
        public static ColumnCount[] Empty(int length) =>
            new ColumnCount[length].Populate(new ColumnCount(0, 0));
    }

    [Test]
    public static void PartOne()
    {
        var (gamma, epsilon) = Inputs()
            .Aggregate(
                new Counter(Nothing),
                (counter, values) => counter with
                {
                    ColumnCounts = counter.ColumnCounts.Match(
                        columnCounts => columnCounts.Update(values),
                        () => Counter.Empty(values.Length).Update(values)).Just(),
                })
            .ColumnCounts.Match(
                columnCounts => columnCounts.Aggregate(
                    (gamma: new List<int>(), epsilon: new List<int>()),
                    (accu, columnCount) =>
                    {
                        var (zeros, ones) = columnCount;
                        accu.gamma.Add(ones > zeros ? 1 : 0);
                        accu.epsilon.Add(ones < zeros ? 1 : 0);

                        return accu;
                    }).ToInt(),
                () => throw new Exception());

        Assert.Pass((gamma * epsilon).ToString());
    }

    private static ColumnCount[] Update(this ColumnCount[] columns, IReadOnlyList<int> values)
    {
        for (var i = 0; i < values.Count; i++)
        {
            columns[i] = columns[i].Count(values[i]);
        }

        return columns;
    }

    private static ColumnCount Count(this ColumnCount columnCount, int value) =>
        value switch
        {
            0 => columnCount with { Zeros = columnCount.Zeros + 1 },
            1 => columnCount with { Ones = columnCount.Ones + 1 },
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };

    private static (int gamma, int epsilon) ToInt(
        this (IEnumerable<int> gamma, IEnumerable<int> epsilon) tuple) =>
        (Convert.ToInt32(string.Join("", tuple.gamma), 2), Convert.ToInt32(string.Join("", tuple.epsilon), 2));

    private static T[] Populate<T>(this T[] array, T value)
    {
        for (var i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }

        return array;
    }

    private static IEnumerable<int[]> Inputs() =>
        Helpers.Inputs(Path.Combine("Puzzle03", "input.txt"), Parse);

    private static int[] Parse(string line) =>
        line.ToCharArray().Select(c => int.Parse(c.ToString())).ToArray();

    private record State(int Zeros, int Ones, int[][] Values);

    [Test]
    public static void PartTwo()
    {
        var (oxygen, carbonDioxide) =
            (Inputs().ToArray().Calculate(Filter((ones, zeros) => ones >= zeros ? 1 : 0)),
                Inputs().ToArray().Calculate(Filter((ones, zeros) => zeros <= ones ? 0 : 1))).ToInt();
            
        Assert.Pass((oxygen * carbonDioxide).ToString());
    }

    private static IEnumerable<int> Calculate(this int[][] inputs,
        Func<IEnumerable<int[]>, int, IEnumerable<int[]>> filter)
    {
        var length = inputs[0].Length;

        int[][] values = inputs;
        for (var i = 0; i < length; i++)
        {
            values = filter(values, i).ToArray();
            if (values.Length == 1)
                break;
        }

        return values[0];
    }

    private static Func<IEnumerable<int[]>, int, IEnumerable<int[]>> Filter(Func<int, int, int> compare) =>
        (numbers, position) =>
        {
            var (zeros, ones, values) = numbers.Aggregate(new State(0, 0, Array.Empty<int[]>()), (state, digits) =>
                digits[position] switch
                {
                    0 => state with { Zeros = state.Zeros + 1, Values = state.Values.Append(digits).ToArray() },
                    1 => state with { Ones = state.Ones + 1, Values = state.Values.Append(digits).ToArray() },
                    _ => throw new ArgumentOutOfRangeException(nameof(digits), digits[position], null)
                });

            return values.Where(digits => digits[position] == compare(ones, zeros));
        };
}