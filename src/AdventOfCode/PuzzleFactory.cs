using AdventOfCode._2022;
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
            (2022, 1) => new P1CalorieCounting(),
            (2022, 2) => new P2RockPaperScissors(),
            (2022, 3) => new P3RucksackReorganization(),
            (2023, 1) => new P1Trebuchet(),
            (2023, 2) => new P2CubeConundrum(),
            (2023, 3) => new P3GearRatios(),
            (2023, 4) => new P4Scratchcards(),
            (2023, 5) => new P5IfYouGiveASeedAFertilizer(),
            (2023, 6) => new P6WaitForIt(),
            (2023, 7) => new P7CamelCards(),
            _ => throw new ArgumentException($"No puzzle configured for year {year} and day {day}.")
        };
}
