using Framework.Core.ScreenPlay;
using Framework.Core.ScreenPlay.Interfaces;

namespace Scaffolding.Tests.ScreenPlay.Web.Questions;

/// <summary>
/// Gets the count of search results.
/// This Question returns the actual count.
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
