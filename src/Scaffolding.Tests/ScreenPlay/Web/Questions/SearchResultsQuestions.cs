using Framework.Core.ScreenPlay;
using Framework.Core.ScreenPlay.Interfaces;

namespace Scaffolding.Tests.ScreenPlay.Web.Questions;

/// <summary>
/// Verifies that search results are visible on the page.
/// This is a ScreenPlay Question that queries system state.
/// </summary>
public class SearchResultsVisible : IQuestion<bool>
{
    /// <inheritdoc/>
    public string Description => "search results are visible";

    /// <summary>
    /// Creates a new SearchResultsVisible question.
    /// </summary>
    /// <returns>A new SearchResultsVisible question.</returns>
    public static SearchResultsVisible Value() => new();

    /// <inheritdoc/>
    public async Task<bool> AnsweredByAsync(IActor actor)
    {
        var ability = actor.AbilityTo<BrowseTheWeb>();
        var resultsLocator = ability.Page.Locator("div#search div.g");
        var count = await resultsLocator.CountAsync();
        return count > 0;
    }
}

/// <summary>
/// Gets the count of search results.
/// This is a ScreenPlay Question that queries system state.
/// </summary>
public class SearchResultsCount : IQuestion<int>
{
    /// <inheritdoc/>
    public string Description => "search results count";

    /// <summary>
    /// Creates a new SearchResultsCount question.
    /// </summary>
    /// <returns>A new SearchResultsCount question.</returns>
    public static SearchResultsCount Value() => new();

    /// <inheritdoc/>
    public async Task<int> AnsweredByAsync(IActor actor)
    {
        var ability = actor.AbilityTo<BrowseTheWeb>();
        var resultsLocator = ability.Page.Locator("div#search div.g");
        return await resultsLocator.CountAsync();
    }
}
