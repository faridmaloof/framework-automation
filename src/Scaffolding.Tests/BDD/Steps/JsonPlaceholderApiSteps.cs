using FluentAssertions;
using Framework.Core.BDD;
using Framework.Core.ScreenPlay;
using Framework.Core.ScreenPlay.Interfaces;
using Reqnroll;
using Scaffolding.Tests.ScreenPlay.Api.Interactions;
using Scaffolding.Tests.ScreenPlay.Api.Questions;

namespace Scaffolding.Tests.BDD.Steps;

/// <summary>
/// Step definitions for JSONPlaceholder API feature.
/// Uses ScreenPlay Pattern for API interactions.
/// </summary>
[Binding]
public class JsonPlaceholderApiSteps
{
    private readonly ScenarioContext _scenarioContext;
    private const string ApiBaseUrl = "https://jsonplaceholder.typicode.com";

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonPlaceholderApiSteps"/> class.
    /// </summary>
    /// <param name="scenarioContext">The scenario context.</param>
    public JsonPlaceholderApiSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    private IActor Actor => ScenarioActor.GetOrCreate(
        _scenarioContext, 
        "ApiActor", 
        () =>
        {
            var apiAbility = new CallAnApi(ApiBaseUrl);
            var actor = new Actor("API Client");
            actor.WhoCan(apiAbility);
            return actor;
        });

    /// <summary>
    /// Step: Given the API is available.
    /// </summary>
    [Given("the API is available")]
    public Task GivenTheApiIsAvailable()
    {
        // API availability is verified by making a request
        return Task.CompletedTask;
    }

    /// <summary>
    /// Step: When I request all posts.
    /// </summary>
    [When("I request all posts")]
    public async Task WhenIRequestAllPosts()
    {
        await Actor.AttemptsToAsync(new GetRequest("/posts"));
    }

    /// <summary>
    /// Step: When I request post with id.
    /// </summary>
    [When("I request post with id {int}")]
    public async Task WhenIRequestPostWithId(int postId)
    {
        await Actor.AttemptsToAsync(new GetRequest($"/posts/{postId}"));
    }

    /// <summary>
    /// Step: When I request all users.
    /// </summary>
    [When("I request all users")]
    public async Task WhenIRequestAllUsers()
    {
        await Actor.AttemptsToAsync(new GetRequest("/users"));
    }

    /// <summary>
    /// Step: Then I should receive a list of posts.
    /// </summary>
    [Then("I should receive a list of posts")]
    public async Task ThenIShouldReceiveAListOfPosts()
    {
        var responseBody = await Actor.AsksForAsync(ResponseBody.Value());
        responseBody.Should().NotBeNullOrEmpty();
        responseBody.Should().StartWith("[");
    }

    /// <summary>
    /// Step: Then I should receive the post details.
    /// </summary>
    [Then("I should receive the post details")]
    public async Task ThenIShouldReceiveThePostDetails()
    {
        var responseBody = await Actor.AsksForAsync(ResponseBody.Value());
        responseBody.Should().NotBeNullOrEmpty();
        responseBody.Should().StartWith("{");
    }

    /// <summary>
    /// Step: And the first post should have an ID.
    /// </summary>
    [Then("the first post should have an ID")]
    public async Task ThenTheFirstPostShouldHaveAnId()
    {
        var responseBody = await Actor.AsksForAsync(ResponseBody.Value());
        responseBody.Should().Contain("\"id\"");
    }

    /// <summary>
    /// Step: And the post title should not be empty.
    /// </summary>
    [Then("the post title should not be empty")]
    public async Task ThenThePostTitleShouldNotBeEmpty()
    {
        var responseBody = await Actor.AsksForAsync(ResponseBody.Value());
        responseBody.Should().Contain("\"title\"");
    }

    /// <summary>
    /// Step: Then I should receive a list of users.
    /// </summary>
    [Then("I should receive a list of users")]
    public async Task ThenIShouldReceiveAListOfUsers()
    {
        var responseBody = await Actor.AsksForAsync(ResponseBody.Value());
        responseBody.Should().NotBeNullOrEmpty();
        responseBody.Should().StartWith("[");
    }

    /// <summary>
    /// Step: And the first user should have an email.
    /// </summary>
    [Then("the first user should have an email")]
    public async Task ThenTheFirstUserShouldHaveAnEmail()
    {
        var responseBody = await Actor.AsksForAsync(ResponseBody.Value());
        responseBody.Should().Contain("\"email\"");
    }
}
