using AdventOfCode._2023;
using AdventOfCode.Core;

namespace AdventOfCode;

public interface IPuzzleFactory
{
    IPuzzle GetPuzzle(int year, int day);
}

public class PuzzleFactory : IPuzzleFactory
{
    public IPuzzle GetPuzzle(int year, int day)
        => (year, day) switch
        {
            (2023, 1) => new P1Trebuchet(),
            _ => throw new ArgumentException($"No puzzle configured for year {year} and day {day}.")
        };
}
