using FluentAssertions;
using Framework.Core.ScreenPlay;
using Framework.Core.ScreenPlay.Interfaces;
using Microsoft.Playwright;
using Reqnroll;
using Scaffolding.Tests.POM.Google.Actions;
using Scaffolding.Tests.ScreenPlay.Web.Questions;
using Scaffolding.Tests.ScreenPlay.Web.Tasks;

namespace Scaffolding.Tests.BDD.Steps;

/// <summary>
/// Step definitions for Google Search feature.
/// Uses ScreenPlay Pattern with POM for page interactions.
/// </summary>
[Binding]
public class GoogleSearchSteps
{
    private ScenarioContext _scenarioContext;
    private IActor? _actor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleSearchSteps"/> class.
    /// </summary>
    /// <param name="scenarioContext">The scenario context.</param>
    public GoogleSearchSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    private IActor Actor
    {
        get
        {
            if (_actor == null)
            {
                if (!_scenarioContext.TryGetValue("Actor", out var actorFromContext))
                {
                    throw new InvalidOperationException("Actor not initialized. BeforeScenario hook must run first.");
                }
                _actor = (IActor)actorFromContext;
            }
            return _actor;
        }
    }

    /// <summary>
    /// Step: Given the actor is on the Google home page.
    /// </summary>
    [Given("the user is on the Google home page")]
    public async Task GivenTheActorIsOnTheGoogleHomePage()
    {
        await Actor.AttemptsToAsync(new NavigateTo("https://www.google.com", "networkidle"));
    }

    /// <summary>
    /// Step: When they search for a query.
    /// Uses POM Actions for maintainability.
    /// </summary>
    [When("they search for {string}")]
    public async Task WhenTheySearchFor(string query)
    {
        await Actor.AttemptsToAsync(GoogleHomePageActions.SearchFor(query));

        // Wait for navigation
        var ability = Actor.AbilityTo<BrowseTheWeb>();
        await ability.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    /// <summary>
    /// Step: Then the search should execute without errors.
    /// This step verifies the search was performed successfully.
    /// </summary>
    [Then("the search should execute without errors")]
    public async Task ThenTheSearchShouldExecuteWithoutErrors()
    {
        // Wait for page to be stable after search
        var ability = Actor.AbilityTo<BrowseTheWeb>();
        await ability.Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Verify we're on a search results page (URL contains /search)
        var url = await Actor.AsksForAsync(new PageUrl());
        url.Should().Contain("/search", "Should be on search results page");
    }

    /// <summary>
    /// Step: Then I should see search results.
    /// Uses ScreenPlay Questions for assertions.
    /// </summary>
    [Then("I should see search results")]
    public async Task ThenIShouldSeeSearchResults()
    {
        var hasResults = await Actor.AsksForAsync(SearchResultsVisible.Value());
        hasResults.Should().BeTrue("Should have search results");
    }

    /// <summary>
    /// Step: And the search results count should be greater than minimum.
    /// Uses ScreenPlay Questions for assertions.
    /// </summary>
    [Then("the search results count should be greater than {int}")]
    public async Task ThenSearchResultsCountShouldBeGreaterThan(int minValue)
    {
        var count = await Actor.AsksForAsync(SearchResultsCount.Value());
        count.Should().BeGreaterThan(minValue, $"Should have more than {minValue} results");
    }
}
