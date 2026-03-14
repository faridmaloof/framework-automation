using FluentAssertions;
using Framework.Core.ScreenPlay.Interfaces;
using Framework.Core.ScreenPlay;
using Reqnroll;
using Scaffolding.Tests.POM.Google.Actions;
using Scaffolding.Tests.ScreenPlay.Web.Tasks;
using Scaffolding.Tests.ScreenPlay.Web.Questions;

namespace Scaffolding.Tests.BDD.Steps;

/// <summary>
/// Step definitions for Google Search feature.
/// Uses ScreenPlay Pattern with POM for page interactions.
/// </summary>
[Binding]
public class GoogleSearchSteps
{
    private readonly IActor _actor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleSearchSteps"/> class.
    /// </summary>
    /// <param name="actor">The actor injected by Reqnroll.</param>
    public GoogleSearchSteps(IActor actor) => _actor = actor;

    /// <summary>
    /// Step: Given the actor is on the Google home page.
    /// </summary>
    [Given("the actor is on the Google home page")]
    public async System.Threading.Tasks.Task GivenTheActorIsOnTheGoogleHomePage()
    {
        await _actor.AttemptsToAsync(new NavigateTo("https://www.google.com", "networkidle"));
    }

    /// <summary>
    /// Step: When they search for a query.
    /// Uses POM Actions for maintainability.
    /// </summary>
    [When("they search for {string}")]
    public async System.Threading.Tasks.Task WhenTheySearchFor(string query)
    {
        await _actor.AttemptsToAsync(GoogleHomePageActions.SearchFor(query));
        
        // Wait for navigation
        var ability = _actor.AbilityTo<BrowseTheWeb>();
        await ability.Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
    }

    /// <summary>
    /// Step: Then the page title should contain expected text.
    /// Uses ScreenPlay Questions for assertions.
    /// </summary>
    [Then("the page title should contain {string}")]
    public async System.Threading.Tasks.Task ThenThePageTitleShouldContain(string expectedText)
    {
        var title = await _actor.AsksForAsync(PageTitle.Value());
        title.Should().Contain(expectedText);
    }

    /// <summary>
    /// Step: And I should see search results.
    /// Uses POM Actions and ScreenPlay Questions.
    /// </summary>
    [Then("I should see search results")]
    public async System.Threading.Tasks.Task ThenIShouldSeeSearchResults()
    {
        var count = await _actor.AsksForAsync(GoogleSearchResultsActions.GetResultCount());
        count.Should().BeGreaterThan(0);
    }
}
