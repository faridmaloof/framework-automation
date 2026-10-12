using Framework.Core.BDD;
using Framework.Core.ScreenPlay.Interfaces;
using Framework.Core.ScreenPlay.Web.Tasks;
using Reqnroll;
using Scaffolding.Tests.Pages.Google;

namespace Scaffolding.Tests.BDD.Steps;

/// <summary>
/// Step definitions for the Google Search feature.
/// Steps are thin: they only hand Screenplay Tasks / Ensure* tasks to the
/// actor — zero selectors, zero assertion logic and zero Playwright calls here.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GoogleSearchSteps"/> class.
/// </remarks>
/// <param name="scenarioContext">The scenario context.</param>
[Binding]
public class GoogleSearchSteps(ScenarioContext scenarioContext) {
    private readonly ScenarioContext _scenarioContext = scenarioContext;

    /// <summary>
    /// Gets the actor for the current scenario (created by the BeforeScenario hook).
    /// </summary>
    private IActor Actor => ScenarioActor.Get(_scenarioContext);

    /// <summary>
    /// Step: Given the user is on the Google home page.
    /// Opens the page through its own PageObject.
    /// </summary>
    [Given("the user is on the Google home page")]
    public Task GivenTheActorIsOnTheGoogleHomePage()
        => Actor.AttemptsToAsync(GoogleHomePage.Instance.Open("networkidle"));

    /// <summary>
    /// Step: When they search for a query.
    /// Delegates to the page-level task <see cref="SearchFor"/>.
    /// </summary>
    [When("they search for {string}")]
    public Task WhenTheySearchFor(string query)
        => Actor.AttemptsToAsync(SearchFor.Query(query));

    /// <summary>
    /// Step: Then the search should execute without errors.
    /// Asserts the real results container rendered. A bot-verification / CAPTCHA
    /// page ("verify you are human") has no results, so the scenario fails
    /// honestly instead of passing on a URL-only check that a blocked page can
    /// still satisfy.
    /// </summary>
    [Then("the search should execute without errors")]
    public Task ThenTheSearchShouldExecuteWithoutErrors()
        => Actor.AttemptsToAsync(
            EnsureVisible.Of(GoogleSearchResultsPage.Instance.ResultsContainer,
                "real search results (#search) must render — if Google returned a bot-verification " +
                "('verify you are human' / CAPTCHA) page, the search did not actually execute"));

    /// <summary>
    /// Step: Then I should see search results.
    /// Validation lives in the reusable Ensure* task.
    /// </summary>
    [Then("I should see search results")]
    public Task ThenIShouldSeeSearchResults()
        => Actor.AttemptsToAsync(
            EnsureVisible.Of(GoogleSearchResultsPage.Instance.ResultsContainer,
                "the results container must render after searching"));

    /// <summary>
    /// Step: Then the search results count should be greater than a minimum.
    /// Validation lives in the reusable Ensure* task.
    /// </summary>
    [Then("the search results count should be greater than {int}")]
    public Task ThenSearchResultsCountShouldBeGreaterThan(int minValue)
        => Actor.AttemptsToAsync(
            EnsureCountGreaterThan.Of(GoogleSearchResultsPage.Instance.ResultTitles, minValue,
                $"at least {minValue} result titles should be listed"));
}