using Framework.Core.BDD;
using Framework.Core.ScreenPlay.Tasks;
using Framework.Core.ScreenPlay.Web.Tasks;
using Reqnroll;

namespace Scaffolding.Tests.BDD.Steps;

/// <summary>
/// Steps demonstrating multi-actor scenarios with INDEPENDENT sessions (spec §6).
/// <para>
/// The primary actor ("the user"/"they") shares the scenario page. Secondary
/// personas ("the administrator") are requested explicitly through
/// <see cref="ScenarioActor.PersonaAsync"/>, which gives them their own
/// isolated browser context — so two actors can act on the same resource
/// through different sessions without state leaking between them.
/// </para>
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MultiActorSessionsSteps"/> class.
/// </remarks>
/// <param name="scenarioContext">The scenario context.</param>
[Binding]
public class MultiActorSessionsSteps(ScenarioContext scenarioContext)
{
    /// <summary>
    /// The scenario context, used to store and retrieve actors and their sessions.
    /// </summary>
    private readonly ScenarioContext _scenarioContext = scenarioContext;

    /// <summary>Step: Given the user is on the page "url" (primary persona).</summary>
    [Given(@"the user is on the page ""(.*)""")]
    public Task GivenTheUserIsOnThePage(string url)
        => ScenarioActor.Persona(_scenarioContext, "user")
            .AttemptsToAsync(NavigateToUrl.To(url));

    /// <summary>
    /// Step: Given the administrator is on the page "url". The administrator is
    /// a secondary persona with its OWN isolated session (spec §6).
    /// </summary>
    [Given(@"the administrator is on the page ""(.*)""")]
    public async Task GivenTheAdministratorIsOnThePage(string url)
    {
        var admin = await ScenarioActor.PersonaAsync(_scenarioContext, "administrator");
        await admin.AttemptsToAsync(NavigateToUrl.To(url));
    }

    /// <summary>Step: Then the user should be on the page containing "url" (primary session).</summary>
    [Then(@"the user should be on the page ""(.*)""")]
    public Task ThenTheUserShouldBeOnThePage(string url)
        => ScenarioActor.Persona(_scenarioContext, "user")
            .AttemptsToAsync(EnsureUrlContains.Text(url, "the user's own session must show its page"));

    /// <summary>Step: Then the administrator should be on the page containing "url" (isolated session).</summary>
    [Then(@"the administrator should be on the page ""(.*)""")]
    public async Task ThenTheAdministratorShouldBeOnThePage(string url)
    {
        var admin = await ScenarioActor.PersonaAsync(_scenarioContext, "administrator");
        await admin.AttemptsToAsync(EnsureUrlContains.Text(url, "the administrator's session must be independent of the user's"));
    }
}
