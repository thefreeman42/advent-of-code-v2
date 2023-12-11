using AdventOfCode.Core;

namespace AdventOfCode._2023;
public class P09MirageMaintenance : IPuzzle
{
    long[][] _input = null!;

    public void Initialize(string[] input) => _input = input
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => line.Split(" ").Select(long.Parse).ToArray())
        .ToArray();

    public PuzzleResult SolvePartOne()
        => PuzzleResult.Success(_input.Select(PredictFutureValue).Sum());

    private long PredictFutureValue(long[] sequence)
        => sequence.ToHashSet().Count == 1
            ? sequence[0]
            : sequence.Last() + PredictFutureValue(CalculateNextSequence(sequence));

    public PuzzleResult SolvePartTwo()
        => PuzzleResult.Success(_input.Select(PredictPastValue).Sum());

    private long PredictPastValue(long[] sequence)
        => sequence.ToHashSet().Count == 1
            ? sequence[0]
            : sequence.First() - PredictPastValue(CalculateNextSequence(sequence));

    private long[] CalculateNextSequence(long[] sequence)
        => sequence.Skip(1).Select((val, ix) => val - sequence[ix]).ToArray();
}
