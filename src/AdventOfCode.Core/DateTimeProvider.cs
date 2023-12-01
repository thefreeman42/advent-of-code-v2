namespace AdventOfCode.Core;

public interface IDateTimeProvider
{
    public DateTime GetNow();
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetNow() => DateTime.UtcNow;
}
