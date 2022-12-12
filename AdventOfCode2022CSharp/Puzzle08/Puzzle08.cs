using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AdventOfCode2021;
using NUnit.Framework;

namespace AdventOfCode2022CSharp.Puzzle08;

internal sealed record Tree(
    int Height,
    bool Top = true,
    bool Right = true,
    bool Bottom = true,
    bool Left = true,
    int ScenicTop = 0,
    int ScenicRight = 0,
    int ScenicBottom = 0,
    int ScenicLeft = 0
)
{
    public bool IsVisible => Top || Right || Bottom || Left;
    public bool Top { get; set; } = Top;
    public bool Right { get; set; } = Right;
    public bool Bottom { get; set; } = Bottom;
    public bool Left { get; set; } = Left;

    public int ScenicScore => ScenicTop * ScenicRight * ScenicBottom * ScenicLeft;
    public int ScenicTop { get; set; } = ScenicTop;
    public int ScenicRight { get; set; } = ScenicRight;
    public int ScenicBottom { get; set; } = ScenicBottom;
    public int ScenicLeft { get; set; } = ScenicLeft;
}

public static class Puzzle08
{
    [Test]
    public static void PartOne()
    {
        var trees = Input().Update();

        var visibleTrees = trees.Aggregate(0,
            (rowState, rows) => rowState + rows.Aggregate(0,
                (columnState, column) => column.IsVisible ? columnState + 1 : columnState));

        Console.WriteLine(visibleTrees);
    }

    [Test]
    public static void PartTwo()
    {
        var trees = Input().Update();

        var bestScenicScore = trees.Aggregate(0,
            (rowState, rows) =>
            {
                var aggregate = rows.Aggregate(0,
                    (columnState, column) =>
                        column.ScenicScore > columnState ? column.ScenicScore : columnState);

                return aggregate > rowState ? aggregate : rowState;
            });

        Console.WriteLine(bestScenicScore);
    }

    private static IEnumerable<Tree[]> Update(this IReadOnlyList<Tree[]> trees)
    {
        for (var row = 1; row < trees.Count - 1; row++)
        {
            for (var column = 1; column < trees[row].Length - 1; column++)
            {
                trees.UpdateVisibility(row, column);
            }
        }

        return trees;
    }

    private static void UpdateVisibility(this IReadOnlyList<Tree[]> trees, int row, int column)
    {
        var current = trees[row][column];

        for (var r = row - 1; r >= 0; r--)
        {
            if (trees[r][column].Height >= current.Height)
            {
                current.Top = false;
                current.ScenicTop = current.ScenicTop == 0 ? row - r : current.ScenicTop;
            }
        }

        if (current.Top) current.ScenicTop = row;

        for (var c = column + 1; c < trees[row].Length; c++)
        {
            if (trees[row][c].Height >= current.Height)
            {
                current.Right = false;
                current.ScenicRight = current.ScenicRight == 0 ? c - column : current.ScenicRight;
            }
        }

        if (current.Right) current.ScenicRight = trees[row].Length - 1 - column;

        for (var r = row + 1; r < trees.Count; r++)
        {
            if (trees[r][column].Height >= current.Height)
            {
                current.Bottom = false;
                current.ScenicBottom = current.ScenicBottom == 0 ? r - row : current.ScenicBottom;
            }
        }

        if (current.Bottom) current.ScenicBottom = trees.Count - 1 - row;

        for (var c = column - 1; c >= 0; c--)
        {
            if (trees[row][c].Height >= current.Height)
            {
                current.Left = false;
                current.ScenicLeft = current.ScenicLeft == 0 ? column - c : current.ScenicLeft;
            }
        }

        if (current.Left) current.ScenicLeft = column;
    }

    private static Tree[][] Input() =>
        Helpers.Inputs(Path.Combine("Puzzle08", "input.txt"), line =>
            line.ToCharArray().Select(c => new Tree(int.Parse($"{c}"))).ToArray()
        ).ToArray();
}