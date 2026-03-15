using Framework.Core.ScreenPlay.Interfaces;

namespace Scaffolding.Tests.ScreenPlay.Api.Questions;

/// <summary>
/// Gets the HTTP status code from the last API response.
/// </summary>
public class ResponseStatus : IQuestion<int>
{
    /// <inheritdoc/>
    public string Description => "HTTP status code";

    /// <summary>
    /// Creates a new ResponseStatus question.
    /// </summary>
    /// <returns>A new ResponseStatus question.</returns>
    public static ResponseStatus Value() => new();

    /// <inheritdoc/>
    public Task<int> AnsweredByAsync(IActor actor)
    {
        var status = actor.Remembered<int>("last_response_status");
        return Task.FromResult(status);
    }
}

/// <summary>
/// Gets the response body from the last API response.
/// </summary>
public class ResponseBody : IQuestion<string>
{
    /// <inheritdoc/>
    public string Description => "response body";

    /// <summary>
    /// Creates a new ResponseBody question.
    /// </summary>
    /// <returns>A new ResponseBody question.</returns>
    public static ResponseBody Value() => new();

    /// <inheritdoc/>
    public Task<string> AnsweredByAsync(IActor actor)
    {
        var body = actor.Remembered<string>("last_response_body") ?? string.Empty;
        return Task.FromResult(body);
    }
}
