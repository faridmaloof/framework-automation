using Framework.Core.BDD;
using Framework.Core.ScreenPlay;
using Framework.Core.ScreenPlay.Api.Abilities;
using Framework.Core.ScreenPlay.Api.Tasks;
using Framework.Core.ScreenPlay.Interfaces;
using Reqnroll;

namespace Scaffolding.Tests.BDD.Steps;

/// <summary>
/// Step definitions for the JSONPlaceholder API feature.
/// Steps are thin: they hand Screenplay API Tasks / Ensure* tasks to the
/// actor — request plumbing and assertion logic live in the framework's
/// reusable Api tasks, not here.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="JsonPlaceholderApiSteps"/> class.
/// </remarks>
/// <param name="scenarioContext">The scenario context.</param>
[Binding]
public class JsonPlaceholderApiSteps(ScenarioContext scenarioContext)
{
    private readonly ScenarioContext _scenarioContext = scenarioContext;
    private const string ApiBaseUrl = "https://jsonplaceholder.typicode.com";

    /// <summary>
    /// Gets (or lazily creates) the API actor for the current scenario.
    /// </summary>
    private IActor Actor => ScenarioActor.GetOrCreate(
        _scenarioContext,
        "ApiActor",
        () =>
        {
            var actor = new Actor("API Client");
            actor.WhoCan(new CallAnApi(ApiBaseUrl));
            return actor;
        });

    /// <summary>Step: Given the API is available.</summary>
    [Given("the API is available")]
    public Task GivenTheApiIsAvailable()
        => Task.CompletedTask;

    /// <summary>Step: When I request all posts.</summary>
    [When("I request all posts")]
    public Task WhenIRequestAllPosts()
        => Actor.AttemptsToAsync(GetResource.From("/posts"));

    /// <summary>Step: When I request the post with the given id.</summary>
    [When("I request post with id {int}")]
    public Task WhenIRequestPostWithId(int postId)
        => Actor.AttemptsToAsync(GetResource.From($"/posts/{postId}"));

    /// <summary>Step: When I request all users.</summary>
    [When("I request all users")]
    public Task WhenIRequestAllUsers()
        => Actor.AttemptsToAsync(GetResource.From("/users"));

    /// <summary>Step: Then I should receive a list of posts.</summary>
    [Then("I should receive a list of posts")]
    public Task ThenIShouldReceiveAListOfPosts()
        => Actor.AttemptsToAsync(
            EnsureBodyIsValidJson.Now("the API must respond with JSON"),
            EnsureBodyIsArray.Now("a collection endpoint must return a JSON array"));

    /// <summary>Step: Then I should receive the post details.</summary>
    [Then("I should receive the post details")]
    public Task ThenIShouldReceiveThePostDetails()
        => Actor.AttemptsToAsync(
            EnsureBodyIsValidJson.Now("the API must respond with JSON"),
            EnsureBodyIsObject.Now("a single-resource endpoint must return a JSON object"));

    /// <summary>Step: And the first post should have an ID.</summary>
    [Then("the first post should have an ID")]
    public Task ThenTheFirstPostShouldHaveAnId()
        => Actor.AttemptsToAsync(
            EnsureBodyContainsField.Named("id", "each post must expose an 'id' field"));

    /// <summary>Step: And the post title should not be empty.</summary>
    [Then("the post title should not be empty")]
    public Task ThenThePostTitleShouldNotBeEmpty()
        => Actor.AttemptsToAsync(
            EnsureBodyContainsField.Named("title", "each post must expose a 'title' field"));

    /// <summary>Step: Then I should receive a list of users.</summary>
    [Then("I should receive a list of users")]
    public Task ThenIShouldReceiveAListOfUsers()
        => Actor.AttemptsToAsync(
            EnsureBodyIsValidJson.Now("the API must respond with JSON"),
            EnsureBodyIsArray.Now("a collection endpoint must return a JSON array"));

    /// <summary>Step: And the first user should have an email.</summary>
    [Then("the first user should have an email")]
    public Task ThenTheFirstUserShouldHaveAnEmail()
        => Actor.AttemptsToAsync(
            EnsureBodyContainsField.Named("email", "each user must expose an 'email' field"));
}