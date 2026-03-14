using Framework.Core.ScreenPlay.Interfaces;
using Scaffolding.Tests.POM.Google.Locators;
using Scaffolding.Tests.ScreenPlay.Web.Interactions;
using Scaffolding.Tests.ScreenPlay.Web.Questions;

namespace Scaffolding.Tests.POM.Google.Actions;

/// <summary>
/// Actions for the Google home page.
/// Contains only behavior - uses ScreenPlay interactions.
/// This follows the POM (Page Object Model) pattern with separated actions.
/// </summary>
public static class GoogleHomePageActions
{
    /// <summary>
    /// Enters a search term into the search input.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>A ScreenPlay interaction to enter text.</returns>
    public static Enter EnterSearchTerm(string query) =>
        Enter.TheText(query).Into(GoogleHomePageLocators.SearchInput);
    
    /// <summary>
    /// Clicks the search button.
    /// </summary>
    /// <returns>A ScreenPlay interaction to click.</returns>
    public static Click ClickSearchButton() =>
        Click.On(GoogleHomePageLocators.SearchButton);
    
    /// <summary>
    /// Performs a complete search action (enter term + click search).
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <returns>An array of ScreenPlay interactions.</returns>
    public static IAction[] SearchFor(string query) =>
        new IAction[]
        {
            EnterSearchTerm(query),
            ClickSearchButton()
        };
}

/// <summary>
/// Actions for the Google search results page.
/// Contains only behavior - uses ScreenPlay interactions.
/// </summary>
public static class GoogleSearchResultsActions
{
    /// <summary>
    /// Gets the count of search results.
    /// </summary>
    /// <returns>A ScreenPlay question for element count.</returns>
    public static ElementCount GetResultCount() =>
        ElementCount.Of(GoogleSearchResultsLocators.SearchResult);
    
    /// <summary>
    /// Gets the title of the first search result.
    /// </summary>
    /// <returns>A ScreenPlay question for element text.</returns>
    public static ElementText GetFirstResultTitle() =>
        ElementText.Of(GoogleSearchResultsLocators.SearchResultTitle);
}
