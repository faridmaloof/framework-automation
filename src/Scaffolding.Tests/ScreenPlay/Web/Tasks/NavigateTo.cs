using Framework.Core.ScreenPlay.Interfaces;
using Framework.Core.ScreenPlay;
using Microsoft.Playwright;

namespace Scaffolding.Tests.ScreenPlay.Web.Tasks;

/// <summary>
/// Navigate to a URL.
/// This is a ScreenPlay Task that represents a business-level action.
/// </summary>
public class NavigateTo : Framework.Core.Screenplay.Task
{
    private readonly string _url;
    private readonly string? _waitFor;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigateTo"/> class.
    /// </summary>
    /// <param name="url">The URL to navigate to.</param>
    /// <param name="waitFor">Optional wait condition (e.g., "networkidle").</param>
    public NavigateTo(string url, string? waitFor = null)
    {
        _url = url;
        _waitFor = waitFor;
    }

    /// <inheritdoc/>
    public override string TaskName => $"Navigate to {_url}";

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        var ability = actor.AbilityTo<BrowseTheWeb>();
        var waitUntil = _waitFor?.ToLowerInvariant() switch
        {
            "networkidle" => WaitUntilState.NetworkIdle,
            "domcontentloaded" => WaitUntilState.DOMContentLoaded,
            _ => WaitUntilState.Load
        };
        await ability.Page.GotoAsync(_url, new PageGotoOptions { WaitUntil = waitUntil });
    }
}
