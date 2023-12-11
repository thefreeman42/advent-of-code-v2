using AdventOfCode.Core;

namespace AdventOfCode._2023;
public class P04Scratchcards : IPuzzle
{
    private Scratchcard[] _cards;

    public void Initialize(string[] input) => _cards = input
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => line.Split(':')[1].Split('|').Select(part => part.Trim()))
        .Select((parts, ix) => new Scratchcard(ix,
            parts.First().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse),
            parts.Last().Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)))
        .ToArray();

    public PuzzleResult SolvePartOne()
        => PuzzleResult.Success(_cards.Sum(c => c.GetScore()));

    public PuzzleResult SolvePartTwo()
    {
        Dictionary<int, int> copies = _cards.ToDictionary(c => c.Id, _ => 1);
        foreach (var card in _cards)
        {
            var matching = card.GetMatchingNumbers();
            for (var ix = card.Id + 1; ix < card.Id + 1 + matching; ix++)
                if (copies.ContainsKey(ix))
                    copies[ix] += copies[card.Id];
        }
        return PuzzleResult.Success(copies.Values.Sum());
    }

    private record Scratchcard(int Id, IEnumerable<int> WinningNumbers, IEnumerable<int> OwnNumbers)
    {
        public int GetMatchingNumbers() => WinningNumbers.Intersect(OwnNumbers).Count();
        public int GetScore() => (int)Math.Pow(2, GetMatchingNumbers() - 1);
    };
}
