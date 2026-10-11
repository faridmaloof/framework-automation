using FluentAssertions;
using Framework.Core.ScreenPlay.Api.Tasks;
using Framework.Core.ScreenPlay.Interfaces;

namespace Framework.Core.ScreenPlay.Api.Questions;

// =====================================================================
// ScreenPlay API questions — pure reads of the actor's last-response
// memory populated by <see cref="GetResource"/> (or any other task).
// Composable: use <see cref="Ensure*"/> tasks in tests for assertions.
// =====================================================================

/// <summary>Gets the HTTP status code of the last API response.</summary>
public sealed class StatusCode : ScreenQuestion<int>
{
    /// <inheritdoc/>
    public override string Description => "last HTTP status code";

    /// <summary>Static factory.</summary>
    public static StatusCode OfLastResponse() => new();

    /// <inheritdoc/>
    public override Task<int> AnsweredByAsync(IActor actor)
        => Task.FromResult(actor.Remembered<int>(ApiMemoryKeys.LastStatusCode));
}

/// <summary>Gets the body string of the last API response.</summary>
public sealed class ResponseBody : ScreenQuestion<string>
{
    /// <inheritdoc/>
    public override string Description => "last response body";

    /// <summary>Static factory.</summary>
    public static ResponseBody OfLastResponse() => new();

    /// <inheritdoc/>
    public override Task<string> AnsweredByAsync(IActor actor)
        => Task.FromResult(actor.Remembered<string>(ApiMemoryKeys.LastBody) ?? string.Empty);
}

/// <summary>Predicate: body is valid JSON (starts with <c>{</c> or <c>[</c>).</summary>
public sealed class BodyIsValidJson : ScreenQuestion<bool>
{
    /// <inheritdoc/>
    public override string Description => "response body is valid JSON";

    /// <summary>Static factory.</summary>
    public static BodyIsValidJson Value() => new();

    /// <inheritdoc/>
    public override async Task<bool> AnsweredByAsync(IActor actor)
    {
        var body = await actor.AsksForAsync(ResponseBody.OfLastResponse());
        return !string.IsNullOrEmpty(body)
            && (body.TrimStart().StartsWith("{") || body.TrimStart().StartsWith("["));
    }
}

/// <summary>Predicate: body is a JSON array.</summary>
public sealed class BodyIsArray : ScreenQuestion<bool>
{
    /// <inheritdoc/>
    public override string Description => "response body is a JSON array";

    /// <summary>Static factory.</summary>
    public static BodyIsArray Value() => new();

    /// <inheritdoc/>
    public override async Task<bool> AnsweredByAsync(IActor actor)
    {
        var body = await actor.AsksForAsync(ResponseBody.OfLastResponse());
        return !string.IsNullOrEmpty(body) && body.TrimStart().StartsWith("[");
    }
}

/// <summary>Predicate: body is a JSON object.</summary>
public sealed class BodyIsObject : ScreenQuestion<bool>
{
    /// <inheritdoc/>
    public override string Description => "response body is a JSON object";

    /// <summary>Static factory.</summary>
    public static BodyIsObject Value() => new();

    /// <inheritdoc/>
    public override async Task<bool> AnsweredByAsync(IActor actor)
    {
        var body = await actor.AsksForAsync(ResponseBody.OfLastResponse());
        return !string.IsNullOrEmpty(body) && body.TrimStart().StartsWith("{");
    }
}

/// <summary>Predicate: body contains a specific field name (with quotes).</summary>
public sealed class BodyContainsField : ScreenQuestion<bool>
{
    private readonly string _fieldName;

    private BodyContainsField(string fieldName) => _fieldName = fieldName;

    /// <inheritdoc/>
    public override string Description => $"response body contains '{_fieldName}'";

    /// <summary>Static factory.</summary>
    public static BodyContainsField Named(string fieldName) => new(fieldName);

    /// <inheritdoc/>
    public override async Task<bool> AnsweredByAsync(IActor actor)
    {
        var body = await actor.AsksForAsync(ResponseBody.OfLastResponse());
        return body.Contains($"\"{_fieldName}\"", StringComparison.Ordinal);
    }
}

/// <summary>Predicate: last HTTP status was successful (2xx).</summary>
public sealed class StatusIsSuccessful : ScreenQuestion<bool>
{
    /// <inheritdoc/>
    public override string Description => "response status is 2xx";

    /// <summary>Static factory.</summary>
    public static StatusIsSuccessful Value() => new();

    /// <inheritdoc/>
    public override async Task<bool> AnsweredByAsync(IActor actor)
    {
        var status = await actor.AsksForAsync(StatusCode.OfLastResponse());
        return status is >= 200 and < 300;
    }
}
