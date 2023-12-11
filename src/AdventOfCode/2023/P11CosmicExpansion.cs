using AdventOfCode.Core;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode._2023;
public class P11CosmicExpansion : IPuzzle
{
    private char[][] _input = null!;

    private const char Empty = '.';

    public void Initialize(string[] input) => _input = input
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => line.Select(c => c).ToArray())
        .ToArray();

    public PuzzleResult SolvePartOne()
        => PuzzleResult.Success(GetShortestDistances(2).Sum());

    public PuzzleResult SolvePartTwo()
        => PuzzleResult.Success(GetShortestDistances(1000000).Sum());

    private IEnumerable<long> GetShortestDistances(long expansionFactor)
    {
        var doubleRows = _input.Select((row, ix) => (row, ix)).Where(item => item.row.All(c => c == Empty)).Select(item => item.ix).ToArray();
        var doubleColumns = Enumerable.Range(0, _input[0].Length).Where(colN => _input.All(row => row[colN] == Empty)).ToArray();
        var galaxyPairs = GetGalaxyPairs();
        var expansionValue = expansionFactor - 1;

        foreach (var (g1, g2) in galaxyPairs)
        {
            // x distance
            var lowX = Math.Min(g1.X, g2.X);
            var highX = Math.Max(g1.X, g2.X);
            var expansionX = doubleColumns.Where(colN => colN >= lowX && colN <= highX).Count() * expansionValue;
            var x = highX - lowX + expansionX;

            // y distance
            var lowY = Math.Min(g1.Y, g2.Y);
            var highY = Math.Max(g1.Y, g2.Y);
            var expansionY = doubleRows.Where(rowN => rowN >= lowY && rowN <= highY).Count() * expansionValue;
            var y = highY - lowY + expansionY;

            yield return x + y;
        }
    }

    private IEnumerable<Galaxy> GetGalaxies()
        => _input
            .SelectMany((row, y) => row.Select((c, x) => (x, y, c)))
            .Where(item => item.c != Empty)
            .Select(item => new Galaxy(item.x, item.y));

    private IEnumerable<(Galaxy, Galaxy)> GetGalaxyPairs()
    {
        var galaxies = GetGalaxies();
        var combinations = from g1 in galaxies
                           from g2 in galaxies
                           where g1 != g2
                           select (g1, g2);
        return combinations.Distinct(new GalaxyPairComparer());
    }

    private record Galaxy(int X, int Y);

    private class GalaxyPairComparer : IEqualityComparer<(Galaxy, Galaxy)>
    {
        public bool Equals((Galaxy, Galaxy) x, (Galaxy, Galaxy) y)
            => (x.Item1 == y.Item1 && x.Item2 == y.Item2) || (x.Item1 == y.Item2 && x.Item2 == y.Item1);
        public int GetHashCode([DisallowNull] (Galaxy, Galaxy) obj)
            => 23 * obj.Item1.GetHashCode() * obj.Item2.GetHashCode();
    }
}
