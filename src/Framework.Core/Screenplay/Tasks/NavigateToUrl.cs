using Framework.Core.ScreenPlay.Interfaces;
using Microsoft.Playwright;

namespace Framework.Core.ScreenPlay.Tasks;

/// <summary>
/// Task to navigate to a URL.
/// Reusable across all web automation projects.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="NavigateToUrl"/> class.
/// </remarks>
/// <param name="url">The URL to navigate to.</param>
/// <param name="waitFor">Optional wait condition.</param>
public class NavigateToUrl(string url, string? waitFor = null) : ScreenTask {
    private readonly string _url = url;
    private readonly string? _waitFor = waitFor;

    /// <inheritdoc/>
    public override string TaskName => $"Navigate to {_url}";

    /// <inheritdoc/>
    public override async System.Threading.Tasks.Task PerformAsAsync(IActor actor) {
        var web = actor.AbilityTo<BrowseTheWeb>();
        var waitUntil = _waitFor?.ToLowerInvariant() switch {
            "networkidle" => WaitUntilState.NetworkIdle,
            "domcontentloaded" => WaitUntilState.DOMContentLoaded,
            _ => WaitUntilState.Load
        };
        await web.Page.GotoAsync(_url, new PageGotoOptions { WaitUntil = waitUntil });
    }

    /// <summary>
    /// Fluent factory method.
    /// </summary>
    /// <param name="url">The URL to navigate to.</param>
    /// <param name="waitFor">Optional wait condition.</param>
    /// <returns>A new NavigateToUrl task.</returns>
    public static NavigateToUrl To(string url, string? waitFor = null) => new(url, waitFor);
}
