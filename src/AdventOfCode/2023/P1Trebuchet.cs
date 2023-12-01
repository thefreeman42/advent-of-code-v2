using AdventOfCode.Core;
using System.Text.RegularExpressions;

namespace AdventOfCode._2023;
internal class P1Trebuchet : IPuzzle
{
    private string[] _input = null!;

    public void Initialize(string[] input)
        => _input = input.Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

    public PuzzleResult SolvePartOne()
    {
        var values = _input.Select(line => GetCalibrationValue(line, @"([1-9])"));
        return PuzzleResult.Success(values.Sum());
    }

    public PuzzleResult SolvePartTwo()
    {
        var values = _input.Select(line => GetCalibrationValue(line, @"(one|two|three|four|five|six|seven|eight|nine|[1-9])"));
        return PuzzleResult.Success(values.Sum());
    }

    private int GetCalibrationValue(string line, string pattern)
    {
        var firstMatch = new Regex(pattern).Match(line);
        var lastMatch = new Regex(pattern, RegexOptions.RightToLeft).Match(line);
        return GetValue(firstMatch.Value) * 10 + GetValue(lastMatch.Value);
    }

    private int GetValue(string code)
    {
        if (int.TryParse(code, out var num))
            return num;

        return code switch
        {
            "one" => 1,
            "two" => 2,
            "three" => 3,
            "four" => 4,
            "five" => 5,
            "six" => 6,
            "seven" => 7,
            "eight" => 8,
            "nine" => 9,
            "zero" => 0,
            _ => throw new NotImplementedException()
        };
    }
}
