using System.ComponentModel.DataAnnotations;

namespace AdventOfCode.Core;

public class InputSettings
{
    public const string SectionKey = "Input";
    public string BaseAddress { get; set; } = "https://adventofcode.com";
    [Required] public string SessionKey { get; set; } = null!;
}
