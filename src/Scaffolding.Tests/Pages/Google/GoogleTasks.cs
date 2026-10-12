using Framework.Core.ScreenPlay;
using Framework.Core.ScreenPlay.Interfaces;
using Framework.Core.ScreenPlay.Web.Interactions;
using Microsoft.Playwright;

namespace Scaffolding.Tests.Pages.Google;

// =====================================================================
// Screenplay tasks grouped by page/flow (Google home + search results).
// Tasks are the business-level vocabulary of the tests: they compose the
// framework's atomic Interactions over the page's targets, so steps and
// feature files never see a selector.
// =====================================================================

/// <summary>
/// Searches Google for a term: types it into the search box and submits
/// with <c>Enter</c>, then waits for navigation to settle so follow-up
/// URL/state questions observe the final page.
/// </summary>
public sealed class SearchFor : ScreenTask {
    private readonly string _query;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchFor"/> class.
    /// </summary>
    /// <param name="query">The search query.</param>
    private SearchFor(string query) => _query = query;

    /// <inheritdoc/>
    public override string TaskName => $"Search for '{_query}'";

    /// <inheritdoc/>
    public override string Description => $"Type '{_query}' into the search box and submit the search";

    /// <summary>Builds the task for the given query.</summary>
    public static SearchFor Query(string query) => new(query);

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        await actor.AttemptsToAsync(
            Enter.TheText(_query).Into(GoogleHomePage.Instance.SearchQueryField).Now(),
            PressKey.EnterOn(GoogleHomePage.Instance.SearchQueryField));

        // The results URL/state questions read the page immediately after the
        // task, so wait until the navigation triggered by the search settles.
        var page = actor.AbilityTo<BrowseTheWeb>().Page;
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}