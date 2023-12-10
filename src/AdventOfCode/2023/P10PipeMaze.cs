using AdventOfCode.Core;

namespace AdventOfCode._2023;
public class P10PipeMaze : IPuzzle
{
    Dictionary<(int, int), Pipe> _pipeMap = null!;

    public void Initialize(string[] input) => _pipeMap = input
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .SelectMany((line, ix) => line.Trim().Select((c, cix) => Pipe.FromInput(c, cix, ix)))
        .ToDictionary(item => item.Coordinates);

    public PuzzleResult SolvePartOne()
        => PuzzleResult.Success(MapCircuit().Length / 2);

    public PuzzleResult SolvePartTwo()
    {
        var circuit = MapCircuit().Select(c => c.Coordinates).ToHashSet();
        var columns = _pipeMap.Max(kvp => kvp.Key.Item1) + 1;
        var rows = _pipeMap.Max(kvp => kvp.Key.Item2) + 1;

        var tracker = Enumerable.Range(0, columns).ToDictionary(x => x, _ => (false, (Bounding?)null));
        long inCount = 0;
        for (var y = 0; y < rows; y++)
            for (var x = 0; x < columns; x++)
            {
                var pipe = _pipeMap[(x, y)];
                if (pipe.IsStarting)
                    pipe = GetEquivalent(pipe);

                var (marked, bounding) = tracker[x];
                var isCircuit = circuit.Contains((x, y));

                if (!isCircuit && marked)
                    inCount++;
                else if (isCircuit)
                    tracker[x] = pipe switch
                    {
                        { LeftConnection: true, RightConnection: true } => (!marked, null),
                        { LeftConnection: true, BottomConnection: true } => (marked, Bounding.Left),
                        { RightConnection: true, BottomConnection: true } => (marked, Bounding.Right),
                        { LeftConnection: true, TopConnection: true } => (bounding == Bounding.Left ? marked : !marked, null),
                        { RightConnection: true, TopConnection: true } => (bounding == Bounding.Right ? marked : !marked, null),
                        _ => (marked, bounding)
                    };
            }

        return PuzzleResult.Success(inCount);
    }

    private Pipe[] MapCircuit()
    {
        var start = _pipeMap.Single(kvp => kvp.Value.IsStarting).Value;
        List<Pipe> results = [start];

        var current = GetConnectedPipes(start).First();
        var previousCoordinates = start.Coordinates;
        while (current != start)
        {
            results.Add(current);
            var nextCoordinates = current.GetNext(previousCoordinates);
            previousCoordinates = current.Coordinates;
            current = _pipeMap[nextCoordinates];
        }
        return [.. results];
    }

    private Pipe GetEquivalent(Pipe pipe)
    {
        if (!pipe.IsStarting) return pipe;
        var (l, r, t, b) = (false, false, false, false);
        if (_pipeMap.TryGetValue((pipe.X - 1, pipe.Y), out var left) && left.RightConnection) l = true;
        if (_pipeMap.TryGetValue((pipe.X + 1, pipe.Y), out var right) && right.LeftConnection) r = true;
        if (_pipeMap.TryGetValue((pipe.X, pipe.Y - 1), out var top) && top.BottomConnection) t = true;
        if (_pipeMap.TryGetValue((pipe.X, pipe.Y + 1), out var bottom) && bottom.TopConnection) b = true;
        return (l, r, t, b) switch
        {
            (true, true, false, false) => Pipe.Horizontal(pipe.X, pipe.Y),
            (false, false, true, true) => Pipe.Vertical(pipe.X, pipe.Y),
            (true, false, true, false) => Pipe.BottomRightCorner(pipe.X, pipe.Y),
            (true, false, false, true) => Pipe.TopRightCorner(pipe.X, pipe.Y),
            (false, true, true, false) => Pipe.BottomLeftCorner(pipe.X, pipe.Y),
            (false, true, false, true) => Pipe.TopLeftCorner(pipe.X, pipe.Y),
            _ => throw new InvalidOperationException()
        };
    }

    private IEnumerable<Pipe> GetConnectedPipes(Pipe startPipe)
    {
        if (_pipeMap.TryGetValue((startPipe.X - 1, startPipe.Y), out var left) && left.RightConnection)
            yield return left;
        if (_pipeMap.TryGetValue((startPipe.X + 1, startPipe.Y), out var right) && right.LeftConnection)
            yield return right;
        if (_pipeMap.TryGetValue((startPipe.X, startPipe.Y - 1), out var top) && top.BottomConnection)
            yield return top;
        if (_pipeMap.TryGetValue((startPipe.X, startPipe.Y + 1), out var bottom) && bottom.TopConnection)
            yield return bottom;
    }



    private enum Bounding
    {
        Left,
        Right
    }

    private class Pipe
    {
        public int X { get; }
        public int Y { get; }
        public bool LeftConnection { get; }
        public bool RightConnection { get; }
        public bool TopConnection { get; }
        public bool BottomConnection { get; }
        public bool IsStarting { get; }

        public (int X, int Y) Coordinates => (X, Y);

        private Pipe(int x, int y,
            bool leftConnection,
            bool rightConnection,
            bool topConnection,
            bool bottomConnection,
            bool isStarting = false)
        {
            X = x;
            Y = y;

            LeftConnection = leftConnection;
            if (leftConnection) Connections.Add((x - 1, y));

            RightConnection = rightConnection;
            if (rightConnection) Connections.Add((x + 1, y));

            TopConnection = topConnection;
            if (topConnection) Connections.Add((x, y - 1));

            BottomConnection = bottomConnection;
            if (bottomConnection) Connections.Add((x, y + 1));

            IsStarting = isStarting;
        }

        private List<(int, int)> Connections = [];

        public (int X, int Y) GetNext((int, int) previous)
            => Connections.Single(c => c != previous);

        public static Pipe FromInput(char c, int x, int y)
            => c switch
            {
                '|' => Vertical(x, y),
                '-' => Horizontal(x, y),
                'F' => TopLeftCorner(x, y),
                'L' => BottomLeftCorner(x, y),
                '7' => TopRightCorner(x, y),
                'J' => BottomRightCorner(x, y),
                'S' => Starting(x, y),
                '.' => None(x, y),
                _ => throw new NotImplementedException()
            };

        public static Pipe Vertical(int x, int y) => new(x, y, false, false, true, true);
        public static Pipe Horizontal(int x, int y) => new(x, y, true, true, false, false);
        public static Pipe TopLeftCorner(int x, int y) => new(x, y, false, true, false, true);
        public static Pipe BottomLeftCorner(int x, int y) => new(x, y, false, true, true, false);
        public static Pipe TopRightCorner(int x, int y) => new(x, y, true, false, false, true);
        public static Pipe BottomRightCorner(int x, int y) => new(x, y, true, false, true, false);
        private static Pipe None(int x, int y) => new(x, y, false, false, false, false, false);
        private static Pipe Starting(int x, int y) => new(x, y, false, false, false, false, true);

        public override bool Equals(object? obj) => obj is Pipe other && other.X == X && other.Y == Y;
        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}
