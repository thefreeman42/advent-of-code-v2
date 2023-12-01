namespace AdventOfCode.Core;

public class DateSettings
{
    public const string SectionKey = "Date";

    public bool UseToday { get; set; } = true;

    public int? YearOverride { get; set; }
    public int? DayOverride { get; set; }
}