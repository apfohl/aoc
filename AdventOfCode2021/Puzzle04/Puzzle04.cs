using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonadicBits;
using NUnit.Framework;

namespace AdventOfCode2021.Puzzle04;

using static Functional;

public static class Puzzle04
{
    private enum Dimension
    {
        X,
        Y
    }

    private sealed class Field
    {
        public int Number { get; }

        public bool IsSelected { get; set; }

        public Field(int number) => Number = number;
    }

    private sealed class Board
    {
        private readonly Field[][] fields;

        public bool HasWon { get; private set; }

        public Board(Field[][] fields) => this.fields = fields;

        public int Sum() => 
            fields.Sum(field => field.Where(f => f.IsSelected == false).Sum(f => f.Number));

        public void Tip(int draw)
        {
            var location = Location(draw);
            location.Match(
                l =>
                {
                    var (x, y) = l;
                    fields[x][y].IsSelected = true;
                    Validate(l);
                },
                () => {});
        }

        private void Validate((int x, int y) location)
        {
            if (Loop(Dimension.X, location.x).Any(field => field.IsSelected == false) &&
                Loop(Dimension.Y, location.y).Any(field => field.IsSelected == false))
            {
                HasWon = false;
            }
            else
            {
                HasWon = true;
            }
        }

        private IEnumerable<Field> Loop(Dimension dimension, int index)
        {
            if (dimension == Dimension.Y)
            {
                foreach (var field in fields)
                {
                    yield return field[index];
                }
            }
            else
            {
                var field = fields[index];
                foreach (var t in field)
                {
                    yield return t;
                }
            }
                
        }

        private Maybe<(int x, int y)> Location(int number)
        {
            for (var i = 0; i < fields.Length; i++)
            {
                for (var j = 0; j < fields[i].Length; j++)
                {
                    if (fields[i][j].Number == number)
                    {
                        return (i, j).Just();
                    }
                }
            }

            return Nothing;
        }
    }

    private sealed record Input(IEnumerable<int> Draws, IEnumerable<Board> Boards)
    {
        public static Input FromFile(string path)
        {
            using var stream = File.OpenRead(path);
            using var reader = new StreamReader(stream);

            IEnumerable<int> draws = reader.ReadLine()!.Split(',').Select(int.Parse);
            List<Board> boards = new();

            reader.ReadLine();

            string? line;
            List<Field[]> tempBoard = new();
            while ((line = reader.ReadLine()) != null)
            {
                if (line == string.Empty)
                {
                    boards.Add(new Board(tempBoard.ToArray()));
                    tempBoard = new List<Field[]>();
                }
                else
                {
                    tempBoard.Add(line.Replace("  ", " ").TrimStart().Split(' ')
                        .Select(s => new Field(int.Parse(s))).ToArray());
                }
            }

            boards.Add(new Board(tempBoard.ToArray()));

            return new Input(draws, boards);
        }
    }

    [Test]
    public static void PartOne()
    {
        var input = Input.FromFile(Path.Combine("Puzzle04", "input.txt"));

        (Board board, int draw) winner = (null, 0)!;
        foreach (var draw in input.Draws)
        {
            foreach (var board in input.Boards)
            {
                board.Tip(draw);

                if (board.HasWon)
                {
                    winner = (board, draw);
                    break;
                }
            }

            if (winner.board != null) break;
        }

        Assert.Pass((winner.board!.Sum() * winner.draw).ToString());
    }

    [Test]
    public static void PartTwo()
    {
        var input = Input.FromFile(Path.Combine("Puzzle04", "input.txt"));

        List<(Board board, int draw)> winners = new();
        foreach (var draw in input.Draws)
        {
            foreach (var board in input.Boards)
            {
                if (winners.All(tuple => tuple.board != board))
                {
                    board.Tip(draw);
                }

                if (board.HasWon)
                {
                    if (winners.All(tuple => tuple.board != board))
                    {
                        winners.Add((board, draw));
                    }
                }
            }
        }

        var (b, d) = winners.Last();

        Assert.Pass((b.Sum() * d).ToString());
    }
}