using AdventOfCode.Core;

namespace AdventOfCode._2022;
public class P2RockPaperScissors : IPuzzle
{
    private StrategyRow[] _input = null;

    private const int LoseScore = 0;
    private const int DrawScore = 3;
    private const int WinScore = 6;

    public void Initialize(string[] input)
        => _input = input
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(str => StrategyRow.Parse(str[0], str[2]))
        .ToArray();

    public PuzzleResult SolvePartOne()
        => PuzzleResult.Success(_input.Select(strategy => strategy.GetScoreWith(StandardRpsLookup)).Sum());

    public PuzzleResult SolvePartTwo()
        => PuzzleResult.Success(_input.Select(strategy => strategy.GetScoreWith(RiggedRpsLookup)).Sum());


    private abstract record RPS(int Score)
    {
        public abstract RPS DefeatedBy { get; }
        public abstract RPS Defeats { get; }

        public static RPS Rock => new Rock();
        public static RPS Paper => new Paper();
        public static RPS Scissors => new Scissors();
    }

    private record Rock() : RPS(1)
    {
        public override RPS DefeatedBy => Paper;

        public override RPS Defeats => Scissors;
    }

    private record Paper() : RPS(2)
    {
        public override RPS DefeatedBy => Scissors;

        public override RPS Defeats => Rock;
    }

    private record Scissors() : RPS(3)
    {
        public override RPS DefeatedBy => Rock;

        public override RPS Defeats => Paper;
    }

    private Dictionary<char, Func<RPS, RPS>> StandardRpsLookup = new()
    {
        { 'X', _ => RPS.Rock },
        { 'Y', _ => RPS.Paper },
        { 'Z', _ => RPS.Scissors },
    };

    private Dictionary<char, Func<RPS, RPS>> RiggedRpsLookup = new()
    {
        { 'X', opponent => opponent.Defeats },
        { 'Y', opponent => opponent },
        { 'Z', opponent => opponent.DefeatedBy },
    };

    private record StrategyRow(RPS OpponentChoice, char GuideChoice)
    {
        public static StrategyRow Parse(char first, char second)
        {
            var opponent = first switch
            {
                'A' => RPS.Rock,
                'B' => RPS.Paper,
                'C' => RPS.Scissors,
                _ => throw new InvalidOperationException($"Unexpected character '{first}' (opponent) in input")
            };
            return new(opponent, second);
        }

        public int GetScoreWith(IDictionary<char, Func<RPS, RPS>> lookup)
        {
            var ownChoice = lookup[GuideChoice](OpponentChoice);
            if (ownChoice.DefeatedBy == OpponentChoice)
                return ownChoice.Score + LoseScore;
            if (ownChoice.Defeats == OpponentChoice)
                return ownChoice.Score + WinScore;
            return ownChoice.Score + DrawScore;
        }
    };
}
