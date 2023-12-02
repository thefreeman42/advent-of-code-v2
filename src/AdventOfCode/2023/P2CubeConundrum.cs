using AdventOfCode.Core;
using System.Text.RegularExpressions;

namespace AdventOfCode._2023;
public class P2CubeConundrum : IPuzzle
{
    private Game[] _input = null!;

    public void Initialize(string[] input)
        => _input = input
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => line.Split(':')[1].Trim())
            .Select(line => line.Split("; "))
            .Select((sets, ix) => new Game(ix + 1, sets.Select(ParseCubes)))
            .ToArray();

    private CubeSet ParseCubes(string input)
    {
        var redMatch = new Regex(@"(\d+) red").Match(input);
        var greenMatch = new Regex(@"(\d+) green").Match(input);
        var blueMatch = new Regex(@"(\d+) blue").Match(input);
        return new CubeSet(
            redMatch.Success ? int.Parse(redMatch.Groups[1].ToString()) : 0,
            greenMatch.Success ? int.Parse(greenMatch.Groups[1].ToString()) : 0,
            blueMatch.Success ? int.Parse(blueMatch.Groups[1].ToString()) : 0);
    }

    public PuzzleResult SolvePartOne()
        => PuzzleResult.Success(_input
            .Where(g => g.IsPossible(12, 13, 14))
            .Sum(g => g.Id));

    public PuzzleResult SolvePartTwo()
        => PuzzleResult.Success(_input.Sum(g => g.GetMinimumPossibleSetPower()));

    private record CubeSet(int Red, int Green, int Blue)
    {
        public int Power => Red * Green * Blue;
    };
    private record Game(int Id, IEnumerable<CubeSet> Sets)
    {
        public bool IsPossible(int red, int green, int blue)
        {
            foreach (var (r, g, b) in Sets)
                if (r > red || g > green || b > blue)
                    return false;
            return true;
        }

        public int GetMinimumPossibleSetPower()
            => new CubeSet(Sets.Max(s => s.Red), Sets.Max(s => s.Green), Sets.Max(s => s.Blue)).Power;
    };
}
