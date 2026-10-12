using Framework.Core.Configuration;
using Framework.Core.ScreenPlay;
using Framework.Core.ScreenPlay.Api.Abilities;
using Framework.Core.Tools;

namespace Framework.Core.Testing;

/// <summary>
/// Manages an API test session for non-BDD / classical tests (TDD, Unit/Integration).
/// Provisions an Actor with <see cref="CallAnApi"/>, tracks timing, and records Allure results.
/// </summary>
public sealed class ApiTestSession : IDisposable {
    private readonly string _suiteName;
    private readonly string _testName;
    private readonly DateTimeOffset _start;
    private readonly Actor _actor;
    private bool _completed;
    private bool _disposed;

    private ApiTestSession(string suiteName, string testName, string baseUrl, TimeSpan? timeout = null)
    {
        _suiteName = suiteName;
        _testName = testName;
        _start = DateTimeOffset.UtcNow;
        _actor = new Actor("ApiUser");
        _actor.WhoCan(new CallAnApi(baseUrl, timeout));
    }

    /// <summary>Gets the Screenplay Actor with the CallAnApi ability.</summary>
    public Actor Actor => _actor;

    /// <summary>
    /// Starts a new API test session.
    /// </summary>
    /// <param name="testName">Name of the test.</param>
    /// <param name="suiteName">Name of the test suite.</param>
    /// <param name="baseUrl">Base URL (if null, falls back to Api:BaseUrl in configuration).</param>
    /// <param name="timeout">Request timeout.</param>
    /// <returns>A running ApiTestSession instance.</returns>
    public static ApiTestSession Start(string? testName = null,
                                       string? suiteName = null,
                                       string? baseUrl = null,
                                       TimeSpan? timeout = null) {
        var effectiveBaseUrl = !string.IsNullOrWhiteSpace(baseUrl)
            ? baseUrl
            : SettingsManager.ApiSettings.BaseUrl;

        var effectiveTimeout = timeout
            ?? TimeSpan.FromSeconds(SettingsManager.ApiSettings.TimeoutSeconds);

        return new ApiTestSession(
            suiteName ?? "API Tests",
            testName ?? $"ApiTest_{DateTime.UtcNow:yyyyMMdd_HHmmss}",
            effectiveBaseUrl,
            effectiveTimeout);
    }

    /// <summary>
    /// Completes the API test and writes the result to Allure.
    /// </summary>
    public void Complete(bool passed,
                         string? errorMessage = null,
                         string? errorTrace = null,
                         string frameworkName = "classical") {
        if (_completed) return;
        _completed = true;

        var fw = SettingsManager.FrameworkSettings;
        var allure = SettingsManager.AllureSettings;
        var resultsDir = Lifecycle.ReportService.ResolveRunResultsDirectory(allure, fw.Reporting);
        var durationMs = (long)(DateTimeOffset.UtcNow - _start).TotalMilliseconds;

        AllureReportHelper.WriteResult(
            resultsDir,
            _suiteName,
            _testName,
            passed,
            durationMs,
            tags: ["api", "classical", frameworkName],
            errorMessage: errorMessage,
            errorTrace: errorTrace,
            testFramework: frameworkName);
    }

    /// <inheritdoc/>
    public void Dispose() {
        if (_disposed) return;
        _disposed = true;

        if (!_completed)
            Complete(passed: true);

        _actor.Dispose();
    }
}
