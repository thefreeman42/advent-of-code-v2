using AdventOfCode.Core;

namespace AdventOfCode._2023;
public class P07CamelCards : IPuzzle
{
    private Hand[] _hands = null!;

    public void Initialize(string[] input) => _hands = input
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => line.Split(' '))
        .Select(line => new Hand(line[0].Select(c => new Card(c)).ToArray(), long.Parse(line[1])))
        .ToArray();

    public PuzzleResult SolvePartOne()
    {
        var orderedHands = _hands
            .OrderByDescending(h => (int)h.GetHandType())
            .ThenByDescending(h => h.Cards[0].GetValue())
            .ThenByDescending(h => h.Cards[1].GetValue())
            .ThenByDescending(h => h.Cards[2].GetValue())
            .ThenByDescending(h => h.Cards[3].GetValue())
            .ThenByDescending(h => h.Cards[4].GetValue())
            .Reverse();

        return PuzzleResult.Success(orderedHands.Select((hand, ix) => hand.Bid * (ix + 1)).Sum());
    }

    public PuzzleResult SolvePartTwo()
    {
        var orderedHands = _hands
            .OrderByDescending(h => (int)h.GetHandType(hasJoker: true))
            .ThenByDescending(h => h.Cards[0].GetValue(hasJoker: true))
            .ThenByDescending(h => h.Cards[1].GetValue(hasJoker: true))
            .ThenByDescending(h => h.Cards[2].GetValue(hasJoker: true))
            .ThenByDescending(h => h.Cards[3].GetValue(hasJoker: true))
            .ThenByDescending(h => h.Cards[4].GetValue(hasJoker: true))
            .Reverse();

        return PuzzleResult.Success(orderedHands.Select((hand, ix) => hand.Bid * (ix + 1)).Sum());
    }

    private record Hand(Card[] Cards, long Bid)
    {
        public HandType GetHandType(bool hasJoker = false)
        {
            if (hasJoker && Cards.Any(c => c.Label == 'J'))
                return GetHandTypeWithJoker();

            var labels = Cards.GroupBy(c => c.Label)
                .Select(g => g.Count())
                .OrderDescending()
                .ToArray();

            return labels.Length < 2
                ? MatchHandType(labels[0], null)
                : MatchHandType(labels[0], labels[1]);
        }

        private HandType GetHandTypeWithJoker()
        {
            var jokers = Cards.Count(c => c.IsJoker);
            var labels = Cards
                .Where(c => !c.IsJoker)
                .GroupBy(c => c.Label)
                .Select(g => g.Count())
                .OrderDescending()
                .ToArray();

            return labels.Length switch
            {
                0 => MatchHandType(jokers, null),
                1 => MatchHandType(labels[0] + jokers, null),
                _ => MatchHandType(labels[0] + jokers, labels[1])
            };
        }

        private static HandType MatchHandType(int topLabelCount, int? secondLabelCount)
            => (topLabelCount, secondLabelCount) switch
            {
                (5, null) => HandType.FiveOfAKind,
                (4, 1) => HandType.FourOfAKind,
                (3, 2) => HandType.FullHouse,
                (3, 1) => HandType.ThreeOfAKind,
                (2, 2) => HandType.TwoPair,
                (2, 1) => HandType.OnePair,
                _ => HandType.HighCard
            };
    };

    private enum HandType
    {
        HighCard = 0,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }

    private record Card(char Label)
    {
        public long GetValue(bool hasJoker = false) => long.TryParse(Label.ToString(), out var value)
            ? value
            : NonNumericValues(hasJoker)[Label];

        public bool IsJoker => Label == 'J';

        private static Dictionary<char, long> NonNumericValues(bool hasJoker) => new()
        {
            { 'T', 10 },
            { 'J', hasJoker ? 1 : 11 },
            { 'Q', 12 },
            { 'K', 13 },
            { 'A', 14 }
        };
    }
}
