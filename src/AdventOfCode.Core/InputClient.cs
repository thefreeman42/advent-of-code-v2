using Microsoft.Extensions.Options;

namespace AdventOfCode.Core;

public interface IInputProvider
{
    Task<string[]> GetInputAsync(int year, int day, CancellationToken cancellationToken);
}

public class InputClient : IInputProvider
{
    private readonly HttpClient _client;
    private readonly string _sessionKey;

    public InputClient(
        IOptions<InputSettings> options)
    {
        _client = new HttpClient() { BaseAddress = new Uri(options.Value.BaseAddress) };
        _sessionKey = options.Value.SessionKey;
    }

    public async Task<string[]> GetInputAsync(int year, int day, CancellationToken cancellationToken)
    {
        var endpoint = $"{year}/day/{day}/input";

        try
        {
            _client.DefaultRequestHeaders.Add("Cookie", $"session={_sessionKey}");
            var response = await _client.GetStringAsync(endpoint, cancellationToken);
            return response.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();
        }
        catch
        {
            throw;
        }
    }
}


