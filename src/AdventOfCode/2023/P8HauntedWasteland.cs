using AdventOfCode.Core;
using System.Text.RegularExpressions;

namespace AdventOfCode._2023;
public class P8HauntedWasteland : IPuzzle
{
    private char[] _directions = null!;
    private Node[] _nodes = null!;

    private const char L = 'L';
    private const char R = 'R';

    private const char A = 'A';
    private const char Z = 'Z';

    public void Initialize(string[] input)
    {
        _directions = input[0].Select(c => c).ToArray();
        _nodes = input.Skip(1)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => new Regex(@"([A-Z]+) = \(([A-Z]+), ([A-Z]+)\)").Match(line))
            .Select(re => new Node(re.Groups[1].Value, re.Groups[2].Value, re.Groups[3].Value))
            .ToArray();
    }

    public PuzzleResult SolvePartOne()
    {
        var currentNode = _nodes.Single(n => n.Label == "AAA");
        long steps = 0;
        while (true)
        {
            foreach (var dir in _directions)
            {
                steps++;
                currentNode = dir switch
                {
                    L => _nodes.Single(n => n.Label == currentNode.Left),
                    R => _nodes.Single(n => n.Label == currentNode.Right),
                    _ => throw new Exception("We shouldn't have gotten here")
                };
                if (currentNode.Label == "ZZZ")
                    return PuzzleResult.Success(steps);
            }
        }
    }

    public PuzzleResult SolvePartTwo()
    {
        var currentNodes = _nodes.Where(n => n.Label.EndsWith(A));
        long[] primes = [];
        foreach (var path in currentNodes)
        {
            var currentNode = path;
            long steps = 0;
            var reached = false;
            while (true)
            {
                foreach (var dir in _directions)
                {
                    steps++;
                    currentNode = dir switch
                    {
                        L => _nodes.Single(n => n.Label == currentNode.Left),
                        R => _nodes.Single(n => n.Label == currentNode.Right),
                        _ => throw new Exception("We shouldn't have gotten here")
                    };
                    if (currentNode.Label.EndsWith(Z))
                    {
                        primes = primes.Union(GetPrimeFactors(steps)).ToArray();
                        reached = true;
                        break;
                    }
                }
                if (reached) break;
            }
        }

        var lowestCommonMultiple = primes.Aggregate((n, acc) => n * acc);
        return PuzzleResult.Success(lowestCommonMultiple);
    }

    private static long[] GetPrimeFactors(long n)
    {
        long[] results = [];
        for (var p = 2; n > 1; p++)
            while (n % p == 0)
            {
                n /= p;
                results = [.. results, p];
            }
        return results;
    }

    private record Node(string Label, string Left, string Right);
}
