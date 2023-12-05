using AdventOfCode.Core;
using System.Text.RegularExpressions;

namespace AdventOfCode._2023;
public class P5IfYouGiveASeedAFertilizer : IPuzzle
{
    private string[] _seedValues = [];
    private Dictionary<string, List<Map>> _maps = [];

    private const string SeedToSoil = "seed-to-soil";
    private const string SoilToFertilizer = "soil-to-fertilizer";
    private const string FertilizerToWater = "fertilizer-to-water";
    private const string WaterToLight = "water-to-light";
    private const string LightToTemperature = "light-to-temperature";
    private const string TemperatureToHumidity = "temperature-to-humidity";
    private const string HumidityToLocation = "humidity-to-location";

    private const string TestInput = @"seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4
";

    public void Initialize(string[] input)
    {
        //input = TestInput.Replace("\r", "").Split("\n").ToArray();
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
            currentList.Add(new Map(split[2], split[1], split[0]));
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
        return mapValue.HasValue ? mapValue.Value : sourceValue;
    }

    public PuzzleResult SolvePartTwo()
    {
        long minLocation = long.MaxValue;
        for (var i = 0; i < _seedValues.Length; i += 2)
        {
            var pair = _seedValues.Skip(i).Take(2).Select(long.Parse);
            var range = new Range(pair.First(), pair.Last());

        }
        return PuzzleResult.Success(minLocation);
    }

    private long MapSeedRangeToLocation(Range seedRange)
    {
        var soil = GetMapRanges(SeedToSoil, [seedRange]);
        var fertilizer = GetMapRanges(SoilToFertilizer, soil);
        var water = GetMapRanges(FertilizerToWater, fertilizer);
        var light = GetMapRanges(WaterToLight, water);
        var temp = GetMapRanges(LightToTemperature, light);
        var hum = GetMapRanges(TemperatureToHumidity, temp);
        var loc = GetMapRanges(HumidityToLocation, hum);
        return long.MinValue;
    }

    private Range[] GetMapRanges(string mapKey, Range[] ranges)
    {
        var mapValue = ranges.SelectMany(r => _maps[mapKey]
            .Select(x => x.GetDestinationRange(r)));
        // todo
        return mapValue.ToArray();
    }

    public IEnumerable<long> CreateRange(long start, long count)
    {
        var limit = start + count;

        while (start < limit)
        {
            yield return start;
            start++;
        }
    }

    private record Map(long Range, long SourceStart, long DestinationStart)
    {
        public long? GetDestinationOrDefault(long sourceValue)
        {
            var diff = sourceValue - SourceStart;
            if (diff < 0 || diff >= Range) return null;
            return DestinationStart + diff;
        }

        public Range GetDestinationRange(Range sourceRange)
        {
            //var dest = GetDestinationOrDefault(sourceValue);
            //if (dest.HasValue)
            //{
            //    var remaining =
            //}
            return sourceRange;
        }
    };

    private record Range(long Start, long Count);
}
