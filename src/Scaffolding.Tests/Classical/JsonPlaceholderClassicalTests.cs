using Framework.Core.ScreenPlay.Api.Tasks;
using Framework.Core.Testing;
using Xunit;

namespace Scaffolding.Tests.Classical;

/// <summary>
/// Demonstrates classical / TDD tests using Framework.Core in pure xUnit without BDD (zero Gherkin/Reqnroll).
/// Shows that the framework library can be consumed directly by any .NET test suite.
/// </summary>
public class JsonPlaceholderClassicalTests : ApiTestBase {
    public JsonPlaceholderClassicalTests() {
        Initialize(testName: "GetPosts_Returns200AndValidList", suiteName: "JSONPlaceholder Classical");
    }

    [Fact]
    public async Task GetPosts_ReturnsSuccessStatusCode() {
        await Actor.AttemptsToAsync(
            GetResource.From("/posts"),
            EnsureStatusSuccessful.Now("API should return 2xx success"),
            EnsureBodyIsValidJson.Now("Response should be valid JSON"),
            EnsureBodyIsArray.Now("Collection endpoint should return a JSON array")
        );
    }
}
