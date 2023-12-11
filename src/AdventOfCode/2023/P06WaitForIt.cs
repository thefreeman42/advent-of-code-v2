using AdventOfCode.Core;

namespace AdventOfCode._2023;
public class P06WaitForIt : IPuzzle
{
    private Race[] _races = null!;
    private Race _bigRace = null!;

    public void Initialize(string[] input)
    {
        var timeStrings = SplitToValues(input[0]);
        var distStrings = SplitToValues(input[1]);
        var times = ParseValues(timeStrings);
        var distances = ParseValues(distStrings);
        _races = Enumerable.Range(0, times.Length)
            .Select(ix => new Race(times[ix], distances[ix]))
            .ToArray();

        var bigTime = long.Parse(string.Join("", timeStrings));
        var bigDist = long.Parse(string.Join("", distStrings));
        _bigRace = new Race(bigTime, bigDist);
    }

    private static string[] SplitToValues(string line)
        => line.Split(':')[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

    private static long[] ParseValues(string[] values)
        => values.Select(long.Parse).ToArray();

    public PuzzleResult SolvePartOne()
        => PuzzleResult.Success(_races.Select(r => r.GetNumberOfWaysToBeat()).Aggregate((n, acc) => n * acc));

    public PuzzleResult SolvePartTwo()
        => PuzzleResult.Success(_bigRace.GetNumberOfWaysToBeat());

    private record Race(long Time, long Distance)
    {
        public long GetNumberOfWaysToBeat()
        {
            var (r1, r2) = SolveQuadratic(-1, Time, -Distance);
            var low = r1 % 1 == 0 ? r1 + 1 : Math.Ceiling(r1);
            var high = r2 % 1 == 0 ? r2 - 1 : Math.Floor(r2);
            return (int)(high - low + 1);
        }

        private (double root1, double root2) SolveQuadratic(long a, long b, long c)
        {
            var root1 = (-b + Math.Sqrt(b * b - 4 * a * c)) / 2 * a;
            var root2 = (-b - Math.Sqrt(b * b - 4 * a * c)) / 2 * a;
            return (root1, root2);
        }
    };
}
