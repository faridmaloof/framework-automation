using FluentAssertions;
using Framework.Core.Configuration;
using Framework.Core.Lifecycle;
using Framework.Core.Testing;
using Xunit;

namespace Framework.Core.Tests;

public class ClassicalTestingTests : IDisposable {
    private readonly string _tempDir;

    public ClassicalTestingTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "classic-test-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    [Fact]
    public void ApiTestSession_CanStartAndComplete_WritesAllureResult()
    {
        using var session = ApiTestSession.Start(
            testName: "GetUsers_ReturnsSuccess",
            suiteName: "UsersApiTests",
            baseUrl: "https://jsonplaceholder.typicode.com");

        session.Actor.Should().NotBeNull();
        session.Actor.Name.Should().Be("ApiUser");

        session.Complete(passed: true, frameworkName: "xunit");

        var resultsDir = Lifecycle.ReportService.ResolveRunResultsDirectory(SettingsManager.AllureSettings, SettingsManager.FrameworkSettings.Reporting);
        Directory.Exists(resultsDir).Should().BeTrue();
    }

    [Fact]
    public void Pipeline_AllureResultsDir_EnvironmentVariable_IsHonored()
    {
        var customPath = Path.Combine(_tempDir, "ci-allure-results");
        try
        {
            Environment.SetEnvironmentVariable("ALLURE_RESULTS_DIR", customPath);
            var settings = new AllureSettings();
            settings.ResultsDirectory.Should().Be(customPath);
        }
        finally
        {
            Environment.SetEnvironmentVariable("ALLURE_RESULTS_DIR", null);
        }
    }

    [Fact]
    public void BasePage_InheritsFrom_PageObject_ForBackwardsCompatibility()
    {
        typeof(Framework.Core.POM.BasePage)
            .IsSubclassOf(typeof(Framework.Core.ScreenPlay.Pages.PageObject))
            .Should().BeTrue();
    }

    [Fact]
    public void SecretsManager_ResolvesFromEnvironmentVariables()
    {
        try
        {
            Environment.SetEnvironmentVariable("SECRET_StripeApiKey", "sk_test_12345");
            SecretsManager.Get("StripeApiKey").Should().Be("sk_test_12345");
            SecretsManager.GetRequired("StripeApiKey").Should().Be("sk_test_12345");
        }
        finally
        {
            Environment.SetEnvironmentVariable("SECRET_StripeApiKey", null);
        }
    }

    [Fact]
    public void SecretsManager_ThrowsWhenRequiredSecretMissing()
    {
        var act = () => SecretsManager.GetRequired("NonExistentSecret_Key_XYZ");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Required secret 'NonExistentSecret_Key_XYZ' was not found*");
    }

    [Fact]
    public void SecretsManager_CustomProvider_CanBePluggedIn()
    {
        var mockProvider = new TestMockSecretsProvider();
        try
        {
            SecretsManager.SetProvider(mockProvider);
            SecretsManager.Get("VaultKey").Should().Be("VaultValue-999");
        }
        finally
        {
            SecretsManager.ResetProvider();
        }
    }

    [Fact]
    public void BrowserOptionsHook_AllowsCustomConfiguration()
    {
        try
        {
            var launched = false;
            BrowserOptionsHook.ConfigureLaunch = opts => launched = true;
            BrowserOptionsHook.ConfigureLaunch(new Microsoft.Playwright.BrowserTypeLaunchOptions());
            launched.Should().BeTrue();
        }
        finally
        {
            BrowserOptionsHook.Reset();
        }
    }

    [Fact]
    public async Task Perform_DelegateTask_ExecutesCorrectly()
    {
        var actor = new Framework.Core.ScreenPlay.Actor("Tester");
        var executed = false;
        await actor.AttemptsToAsync(Framework.Core.ScreenPlay.Perform.Task("Inline test task", a =>
        {
            executed = true;
            a.Name.Should().Be("Tester");
            return Task.CompletedTask;
        }));

        executed.Should().BeTrue();
    }

    private class TestMockSecretsProvider : ISecretsProvider
    {
        public string? GetSecret(string key) => key == "VaultKey" ? "VaultValue-999" : null;
        public T? GetSecretsSection<T>(string sectionName) where T : class, new() => null;
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_tempDir)) Directory.Delete(_tempDir, recursive: true); } catch { }
    }
}
