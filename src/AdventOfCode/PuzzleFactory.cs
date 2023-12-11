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
            (2022, 1) => new P01CalorieCounting(),
            (2022, 2) => new P02RockPaperScissors(),
            (2022, 3) => new P03RucksackReorganization(),
            (2023, 1) => new P01Trebuchet(),
            (2023, 2) => new P02CubeConundrum(),
            (2023, 3) => new P03GearRatios(),
            (2023, 4) => new P04Scratchcards(),
            (2023, 5) => new P05IfYouGiveASeedAFertilizer(),
            (2023, 6) => new P06WaitForIt(),
            (2023, 7) => new P07CamelCards(),
            (2023, 8) => new P08HauntedWasteland(),
            (2023, 9) => new P09MirageMaintenance(),
            (2023, 10) => new P10PipeMaze(),
            _ => throw new ArgumentException($"No puzzle configured for year {year} and day {day}.")
        };
}
