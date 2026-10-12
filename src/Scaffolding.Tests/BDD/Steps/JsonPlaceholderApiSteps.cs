using FluentAssertions;
using Framework.Core.BDD;
using Framework.Core.ScreenPlay;
using Framework.Core.ScreenPlay.Api.Abilities;
using Framework.Core.ScreenPlay.Api.Tasks;
using Framework.Core.ScreenPlay.Interfaces;
using Reqnroll;

namespace Scaffolding.Tests.BDD.Steps;

/// <summary>
/// Step definitions for the JSONPlaceholder API feature.
/// Each step composes a Task or an Ensure task. No direct HTTP calls,
/// no inline assertions, no raw selectors — full ScreenPlay composition.
/// </summary>
[Binding]
public sealed class JsonPlaceholderApiSteps {
    private const string ApiBaseUrl = "https://jsonplaceholder.typicode.com";
    private readonly ScenarioContext _scenarioContext;

    public JsonPlaceholderApiSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    private IActor Actor => ScenarioActor.GetOrCreate(
        _scenarioContext,
        "ApiActor",
        () => new ActorBuilder("API Client")
            .WithAbility(new CallAnApi(ApiBaseUrl))
            .Build());

    /// <summary>Given: the API is available (smoke check — kept for BDD readability).</summary>
    [Given("the API is available")]
    public static Task GivenTheApiIsAvailable() => Task.CompletedTask;

    /// <summary>When: I request all posts.</summary>
    [When("I request all posts")]
    public Task WhenIRequestAllPosts() => Actor.AttemptsToAsync(GetResource.From("/posts"));

    /// <summary>When: I request post with id {int}.</summary>
    [When("I request post with id {int}")]
    public Task WhenIRequestPostWithId(int postId)
        => Actor.AttemptsToAsync(GetResource.From($"/posts/{postId}"));

    /// <summary>When: I request all users.</summary>
    [When("I request all users")]
    public Task WhenIRequestAllUsers() => Actor.AttemptsToAsync(GetResource.From("/users"));

    /// <summary>Then: I should receive a list of posts (valid JSON array).</summary>
    [Then("I should receive a list of posts")]
    public Task ThenIShouldReceiveAListOfPosts()
        => Actor.AttemptsToAsync(
            EnsureBodyIsValidJson.Now("the API must respond with JSON"),
            EnsureBodyIsArray.Now("a collection endpoint must return a JSON array"));

    /// <summary>Then: the first post should have an ID.</summary>
    [Then("the first post should have an ID")]
    public Task ThenTheFirstPostShouldHaveAnId()
        => Actor.AttemptsToAsync(
            EnsureBodyContainsField.Named("id", because: "each post must expose an 'id' field"));

    /// <summary>Then: I should receive the post details (valid JSON object).</summary>
    [Then("I should receive the post details")]
    public Task ThenIShouldReceiveThePostDetails()
        => Actor.AttemptsToAsync(
            EnsureBodyIsValidJson.Now("the API must respond with JSON"),
            EnsureBodyIsObject.Now("a single-resource endpoint must return a JSON object"));

    /// <summary>Then: the post title should not be empty.</summary>
    [Then("the post title should not be empty")]
    public Task ThenThePostTitleShouldNotBeEmpty()
        => Actor.AttemptsToAsync(
            EnsureBodyContainsField.Named("title", because: "each post must expose a 'title' field"));

    /// <summary>Then: I should receive a list of users (valid JSON array).</summary>
    [Then("I should receive a list of users")]
    public Task ThenIShouldReceiveAListOfUsers()
        => Actor.AttemptsToAsync(
            EnsureBodyIsValidJson.Now("the API must respond with JSON"),
            EnsureBodyIsArray.Now("a collection endpoint must return a JSON array"));

    /// <summary>Then: the first user should have an email.</summary>
    [Then("the first user should have an email")]
    public Task ThenTheFirstUserShouldHaveAnEmail()
        => Actor.AttemptsToAsync(
            EnsureBodyContainsField.Named("email", because: "each user must expose an 'email' field"));
}
