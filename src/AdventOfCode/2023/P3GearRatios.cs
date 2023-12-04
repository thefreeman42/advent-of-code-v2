using AdventOfCode.Core;
using System.Text.RegularExpressions;

namespace AdventOfCode._2023;
public class P3GearRatios : IPuzzle
{
    private Coordinate[][] _schematic = null!;

    public void Initialize(string[] input) => _schematic = input
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select((line, ix) => line.Select((c, cix) => new Coordinate(c, cix, ix)).ToArray())
        .ToArray();

    public PuzzleResult SolvePartOne()
    {
        var result = 0;
        for (int i = 0; i < _schematic.Length; i++)
        {
            List<Coordinate> buffer = [];
            bool isPartNumber = false;
            for (int j = 0; j < _schematic[i].Length; j++)
            {
                var current = _schematic[i][j];
                if (!current.IsNumeric())
                {
                    if (buffer.Count > 0 && isPartNumber)
                        result += ParseNumber(buffer);
                    buffer = [];
                    isPartNumber = false;
                    continue;
                }

                buffer.Add(current);
                if (current.HasAdjacentSymbol(_schematic))
                    isPartNumber = true;
            }

            if (buffer.Count > 0 && isPartNumber)
                result += ParseNumber(buffer);
        }
        return PuzzleResult.Success(result);
    }

    public PuzzleResult SolvePartTwo()
    {
        Dictionary<Coordinate, List<int>> gearDict = [];

        for (int i = 0; i < _schematic.Length; i++)
        {
            List<Coordinate> buffer = [];
            List<Coordinate> adjacentGears = [];
            for (int j = 0; j < _schematic[i].Length; j++)
            {
                var current = _schematic[i][j];
                if (!current.IsNumeric())
                {
                    if (buffer.Count > 0 && adjacentGears.Count > 0)
                    {
                        var number = ParseNumber(buffer);
                        foreach (var gear in new HashSet<Coordinate>(adjacentGears))
                        {
                            gearDict.TryAdd(gear, []);
                            gearDict[gear].Add(number);
                        }
                    }

                    buffer = [];
                    adjacentGears = [];
                    continue;
                }

                buffer.Add(current);
                var gears = current.FindAdjacentGears(_schematic);
                adjacentGears.AddRange(gears);
            }

            if (buffer.Count > 0 && adjacentGears.Count > 0)
            {
                var number = ParseNumber(buffer);
                foreach (var gear in new HashSet<Coordinate>(adjacentGears))
                {
                    gearDict.TryAdd(gear, []);
                    gearDict[gear].Add(number);
                }
            }
        }

        var result = gearDict.Where(kvp => kvp.Value.Count == 2).Sum(kvp => kvp.Value.First() * kvp.Value.Last());
        return PuzzleResult.Success(result);
    }

    private int ParseNumber(IEnumerable<Coordinate> coordinates)
        => int.Parse(string.Join("", coordinates.Select(c => c.Value)));

    private record Coordinate(char Value, int X, int Y)
    {
        private const char Empty = '.';
        private const char Gear = '*';
        public bool IsNumeric() => Regex.IsMatch(Value.ToString(), @"\d");
        public bool IsEmpty() => Value == Empty;
        public bool IsSymbol() => !IsNumeric() && !IsEmpty();
        public bool IsGear() => Value == Gear;
        public bool HasAdjacentSymbol(Coordinate[][] schematic)
        {
            for (int i = Y - 1; i < Y + 2; i++)
            {
                for (int j = X - 1; j < X + 2; j++)
                {
                    try
                    {
                        if (schematic[i][j].IsSymbol())
                            return true;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        continue;
                    }
                }
            }
            return false;
        }

        public IEnumerable<Coordinate> FindAdjacentGears(Coordinate[][] schematic)
        {
            List<Coordinate> result = [];
            for (int i = Y - 1; i < Y + 2; i++)
            {
                for (int j = X - 1; j < X + 2; j++)
                {
                    try
                    {
                        if (schematic[i][j].IsGear())
                            result.Add(schematic[i][j]);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        continue;
                    }
                }
            }
            return result;
        }
    };
}
