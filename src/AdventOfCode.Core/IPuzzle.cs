namespace AdventOfCode.Core;
public interface IPuzzle
{
    void Initialize(string[] input);
    PuzzleResult SolvePartOne();
    PuzzleResult SolvePartTwo();
}

public class PuzzleResult
{
    private readonly long? _solution;
    private readonly string? _error;

    private PuzzleResult(long? solution, string? error)
    {
        _solution = solution;
        _error = error;
    }

    public static PuzzleResult Success(long solution)
        => new(solution, null);
    public static PuzzleResult Failure(string error)
        => new(null, error);

    public static PuzzleResult Unsolved => new(null, null);

    public override string ToString()
    {
        if (_solution.HasValue) return _solution.Value.ToString();
        if (_error is not null) return _error;
        return "Not solved";
    }
}
