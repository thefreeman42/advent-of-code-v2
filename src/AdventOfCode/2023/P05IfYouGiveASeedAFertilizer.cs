using AdventOfCode.Core;
using System.Text.RegularExpressions;

namespace AdventOfCode._2023;
public class P05IfYouGiveASeedAFertilizer : IPuzzle
{
    private string[] _seedValues = [];
    private readonly Dictionary<string, List<Map>> _maps = [];

    private const string SeedToSoil = "seed-to-soil";
    private const string SoilToFertilizer = "soil-to-fertilizer";
    private const string FertilizerToWater = "fertilizer-to-water";
    private const string WaterToLight = "water-to-light";
    private const string LightToTemperature = "light-to-temperature";
    private const string TemperatureToHumidity = "temperature-to-humidity";
    private const string HumidityToLocation = "humidity-to-location";

    public void Initialize(string[] input)
    {
        _seedValues = input[0].Split(':')[1].Trim().Split(' ').ToArray();
        var lines = input.Skip(2);

        string currentKey = "";
        List<Map> currentList = [];
        foreach (var line in lines)
        {
            var re = new Regex(@"([a-z\-]+) map:").Match(line);
            if (re.Success)
            {
                currentKey = re.Groups[1].Value;
                currentList = [];
                continue;
            }
            if (line == string.Empty && currentList.Count > 0)
            {
                _maps.Add(currentKey, currentList);
                continue;
            }

            var split = line.Split(' ').Select(long.Parse).ToArray();
            currentList.Add(Map.FromInput(split[0], split[1], split[2]));
        }
    }

    public PuzzleResult SolvePartOne()
    {
        var locations = _seedValues.Select(long.Parse).Select(MapSeedToLocation);
        return PuzzleResult.Success(locations.Min());
    }

    private long MapSeedToLocation(long seed)
    {
        var soil = GetMapValue(SeedToSoil, seed);
        var fertilizer = GetMapValue(SoilToFertilizer, soil);
        var water = GetMapValue(FertilizerToWater, fertilizer);
        var light = GetMapValue(WaterToLight, water);
        var temp = GetMapValue(LightToTemperature, light);
        var hum = GetMapValue(TemperatureToHumidity, temp);
        return GetMapValue(HumidityToLocation, hum);
    }
    private long GetMapValue(string mapKey, long sourceValue)
    {
        var mapValue = _maps[mapKey]
            .Select(x => x.GetDestinationOrDefault(sourceValue))
            .FirstOrDefault(x => x.HasValue);
        return mapValue ?? sourceValue;
    }

    public PuzzleResult SolvePartTwo()
    {
        long minLocation = long.MaxValue;
        for (var i = 0; i < _seedValues.Length; i += 2)
        {
            var pair = _seedValues.Skip(i).Take(2).Select(long.Parse);
            var range = new Range(pair.First(), pair.Last());
            var locationRanges = MapSeedRangeToLocationRanges(range);
            minLocation = Math.Min(minLocation, locationRanges.Min(r => r.Start));
        }
        return PuzzleResult.Success(minLocation);
    }

    private Range[] MapSeedRangeToLocationRanges(Range seedRange)
    {
        var soil = GetMapRanges(SeedToSoil, [seedRange]);
        var fertilizer = GetMapRanges(SoilToFertilizer, soil);
        var water = GetMapRanges(FertilizerToWater, fertilizer);
        var light = GetMapRanges(WaterToLight, water);
        var temp = GetMapRanges(LightToTemperature, light);
        var hum = GetMapRanges(TemperatureToHumidity, temp);
        return GetMapRanges(HumidityToLocation, hum);
    }

    private Range[] GetMapRanges(string mapKey, Range[] ranges)
        => ranges.SelectMany(r => MapToRange([r], [], _maps[mapKey])).ToArray();

    private Range[] MapToRange(in Range[] sources, in Range[] destinations, IEnumerable<Map> maps)
    {
        var map = maps.FirstOrDefault();
        if (map == default) return [.. sources, .. destinations];

        Range[] newSources = [];
        Range[] newDestinations = destinations;

        foreach (var s in sources)
        {
            var (mapped, others) = map.CalculateIntersection(s);
            newSources = [.. newSources, .. others];
            if (mapped is Range dest) newDestinations = [.. newDestinations, dest];
        }

        return MapToRange(newSources, newDestinations, maps.Skip(1));
    }
    private record Range(long Start, long Count)
    {
        public long InclusiveEnd => Start + Count - 1;
    };

    private record Map(Range SourceRange, Range DestinationRange)
    {
        public static Map FromInput(long destinationStart, long sourceStart, long range)
            => new(
                new Range(sourceStart, range),
                new Range(destinationStart, range));

        public long MapDiff => DestinationRange.Start - SourceRange.Start;

        public long? GetDestinationOrDefault(long sourceValue)
        {
            var diff = sourceValue - SourceRange.Start;
            if (diff < 0 || diff >= SourceRange.Count) return null;
            return DestinationRange.Start + diff;
        }

        public (Range? Mapped, Range[] NonIntersecting) CalculateIntersection(Range source)
        {
            if (source.Start < SourceRange.Start && source.InclusiveEnd > SourceRange.InclusiveEnd)
            {
                return (DestinationRange, [
                    new(source.Start, SourceRange.Start - source.Start),
                    new(SourceRange.InclusiveEnd + 1, source.InclusiveEnd - SourceRange.InclusiveEnd)
                    ]);
            }

            if (source.Start >= SourceRange.Start && source.InclusiveEnd <= SourceRange.InclusiveEnd)
            {
                var startDiff = source.Start - SourceRange.Start;
                return (new(DestinationRange.Start + startDiff, source.Count), []);
            }

            if (source.Start >= SourceRange.Start && source.Start <= SourceRange.InclusiveEnd)
            {
                var startDiff = source.Start + SourceRange.Start;
                return (new(DestinationRange.Start + startDiff, DestinationRange.Count - startDiff), [
                    new(SourceRange.InclusiveEnd + 1, source.InclusiveEnd - SourceRange.InclusiveEnd)
                    ]);
            }

            if (source.InclusiveEnd >= SourceRange.Start && source.InclusiveEnd <= SourceRange.InclusiveEnd)
            {
                var startDiff = SourceRange.Start - source.Start;
                return (new(DestinationRange.Start, source.Count - startDiff), [
                    new(source.Start, startDiff)
                    ]);
            }

            return (null, [source]);
        }
    };
}
