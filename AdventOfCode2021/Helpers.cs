using System;
using System.Collections.Generic;
using System.IO;

namespace AdventOfCode2021;

public static class Helpers
{
    public static IEnumerable<T> Inputs<T>(string path, Func<string, T> convert)
    {
        using var stream = File.OpenRead(path);
        using var reader = new StreamReader(stream);

        while (reader.ReadLine() is {} line)
            yield return convert(line);
    }

    public static string CapitalCase(this string input) =>
        char.ToUpper(input[0]) + input[1..];
}