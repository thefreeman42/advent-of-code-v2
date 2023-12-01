using AdventOfCode.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AdventOfCode;
public class PuzzleHost(
    IInputProvider inputProvider,
    IPuzzleFactory puzzleFactory,
    IDateTimeProvider dateTimeProvider,
    IOptions<DateSettings> options,
    ILogger<PuzzleHost> logger) : BackgroundService
{
    private readonly DateSettings _settings = options.Value;
    private readonly IInputProvider _inputProvider = inputProvider;
    private readonly IPuzzleFactory _puzzleFactory = puzzleFactory;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;
    private readonly ILogger<PuzzleHost> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var (year, day) = GetDate();
        _logger.LogInformation("Executing puzzle solution for Day {Day} of {Year}", day, year);

        var puzzle = _puzzleFactory.GetPuzzle(year, day);
        var input = await _inputProvider.GetInputAsync(year, day, stoppingToken);

        puzzle.Initialize(input);

        var partOneResult = puzzle.SolvePartOne();
        _logger.LogInformation("Part 1 solution: {PartOneSolution}", partOneResult);
        var partTwoResult = puzzle.SolvePartTwo();
        _logger.LogInformation("Part 2 solution: {PartTwoSolution}", partTwoResult);
    }

    private (int year, int day) GetDate()
    {
        if (!_settings.UseToday &&
            _settings.YearOverride is { } year &&
            _settings.DayOverride is { } day)
            return (year, day);

        var now = _dateTimeProvider.GetNow();
        if (now.Month != 12)
            throw new InvalidOperationException("It's not even December... Disable `UseToday` and specify overrides please.");
        return (now.Year, now.Day);
    }
}
