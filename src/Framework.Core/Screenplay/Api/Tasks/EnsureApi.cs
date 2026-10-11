using FluentAssertions;
using Framework.Core.ScreenPlay.Api.Questions;
using Framework.Core.ScreenPlay.Interfaces;

namespace Framework.Core.ScreenPlay.Api.Tasks;

// =====================================================================
// API Ensure* tasks — combine a Question with FluentAssertions.
// Mirror of Web/Tasks/Ensure for the API domain.
// =====================================================================

/// <summary>Ensures the last API response status is 2xx.</summary>
public sealed class EnsureStatusSuccessful : ScreenTask
{
    private readonly string _because;
    private EnsureStatusSuccessful(string because) => _because = because;

    /// <inheritdoc/>
    public override string TaskName => "Ensure response status is 2xx";

    /// <inheritdoc/>
    public override string Description =>
        string.IsNullOrEmpty(_because) ? TaskName : $"{TaskName}: {_because}";

    /// <summary>Static factory.</summary>
    public static EnsureStatusSuccessful Now(string because = "")
        => new(because);

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        var ok = await actor.AsksForAsync(StatusIsSuccessful.Value());
        ok.Should().BeTrue(_because);
    }
}

/// <summary>Ensures the last response body is a JSON array.</summary>
public sealed class EnsureBodyIsArray : ScreenTask
{
    private readonly string _because;
    private EnsureBodyIsArray(string because) => _because = because;

    /// <inheritdoc/>
    public override string TaskName => "Ensure response body is a JSON array";

    /// <inheritdoc/>
    public override string Description =>
        string.IsNullOrEmpty(_because) ? TaskName : $"{TaskName}: {_because}";

    /// <summary>Static factory.</summary>
    public static EnsureBodyIsArray Now(string because = "")
        => new(because);

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        var ok = await actor.AsksForAsync(BodyIsArray.Value());
        ok.Should().BeTrue(_because);
    }
}

/// <summary>Ensures the last response body is a JSON object.</summary>
public sealed class EnsureBodyIsObject : ScreenTask
{
    private readonly string _because;
    private EnsureBodyIsObject(string because) => _because = because;

    /// <inheritdoc/>
    public override string TaskName => "Ensure response body is a JSON object";

    /// <inheritdoc/>
    public override string Description =>
        string.IsNullOrEmpty(_because) ? TaskName : $"{TaskName}: {_because}";

    /// <summary>Static factory.</summary>
    public static EnsureBodyIsObject Now(string because = "")
        => new(because);

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        var ok = await actor.AsksForAsync(BodyIsObject.Value());
        ok.Should().BeTrue(_because);
    }
}

/// <summary>Ensures the last response body contains the given field name.</summary>
public sealed class EnsureBodyContainsField : ScreenTask
{
    private readonly string _fieldName;
    private readonly string _because;
    private EnsureBodyContainsField(string fieldName, string because)
    {
        _fieldName = fieldName;
        _because = because;
    }

    /// <inheritdoc/>
    public override string TaskName => $"Ensure response body contains '{_fieldName}'";

    /// <inheritdoc/>
    public override string Description =>
        string.IsNullOrEmpty(_because) ? TaskName : $"{TaskName}: {_because}";

    /// <summary>Static factory.</summary>
    public static EnsureBodyContainsField Named(string fieldName, string because = "")
        => new(fieldName, because);

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        var ok = await actor.AsksForAsync(BodyContainsField.Named(_fieldName));
        ok.Should().BeTrue(_because);
    }
}

/// <summary>Ensures the last response body is valid JSON (object or array).</summary>
public sealed class EnsureBodyIsValidJson : ScreenTask
{
    private readonly string _because;
    private EnsureBodyIsValidJson(string because) => _because = because;

    /// <inheritdoc/>
    public override string TaskName => "Ensure response body is valid JSON";

    /// <inheritdoc/>
    public override string Description =>
        string.IsNullOrEmpty(_because) ? TaskName : $"{TaskName}: {_because}";

    /// <summary>Static factory.</summary>
    public static EnsureBodyIsValidJson Now(string because = "")
        => new(because);

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        var ok = await actor.AsksForAsync(BodyIsValidJson.Value());
        ok.Should().BeTrue(_because);
    }
}
