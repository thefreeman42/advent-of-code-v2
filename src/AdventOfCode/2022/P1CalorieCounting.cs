using AdventOfCode.Core;

namespace AdventOfCode._2022;
public class P1CalorieCounting : IPuzzle
{
    private string[] _input = null!;

    public void Initialize(string[] input) => _input = input;

    public PuzzleResult SolvePartOne()
        => PuzzleResult.Success(GetCalories().Max());

    public PuzzleResult SolvePartTwo()
        => PuzzleResult.Success(GetCalories().OrderByDescending(c => c).Take(3).Sum());

    private int[] GetCalories()
    {
        List<int> calories = [];
        var counter = 0;
        foreach (var item in _input)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                calories.Add(counter);
                counter = 0;
                continue;
            }
            counter += int.Parse(item);
        }
        return calories.ToArray();
    }
}
