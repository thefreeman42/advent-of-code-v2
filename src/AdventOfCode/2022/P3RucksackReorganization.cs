using AdventOfCode.Core;

namespace AdventOfCode._2022;
public class P3RucksackReorganization : IPuzzle
{
    private Rucksack[] _input = null!;

    public void Initialize(string[] input)
    {
        List<Rucksack> results = [];
        foreach (var line in input.Where(l => !string.IsNullOrWhiteSpace(l)))
        {
            var compartmentSize = line.Length / 2;
            results.Add(new Rucksack(
                line.Take(compartmentSize).ToArray(),
                line.Skip(compartmentSize).ToArray()));
        }
        _input = [.. results];
    }

    public PuzzleResult SolvePartOne()
    {
        var result = _input
            .Select(rs => rs.FirstCompartment.Intersect(rs.SecondCompartment).Single())
            .Select(GetPriority)
            .Sum();
        return PuzzleResult.Success(result);
    }

    public PuzzleResult SolvePartTwo()
    {
        var result = 0;
        var ix = 0;
        while (ix < _input.Length)
        {
            var group = _input.Skip(ix).Take(3).Select(rs => rs.AllItems);
            var badge = group.Skip(1).Aggregate(
                new HashSet<char>(group.First()),
                (h, e) => { h.IntersectWith(e); return h; })
                .Single();
            result += GetPriority(badge);
            ix += 3;
        }
        return PuzzleResult.Success(result);
    }

    private int GetPriority(char c) => char.IsUpper(c) ? c - 'A' + 27 : c - 'a' + 1;

    private record Rucksack(char[] FirstCompartment, char[] SecondCompartment)
    {
        public char[] AllItems => [.. FirstCompartment, .. SecondCompartment];
    };
}
