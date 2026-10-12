using Framework.Core.Configuration;
using Framework.Core.Lifecycle;
using Framework.Core.ScreenPlay;
using Framework.Core.Tools;
using Microsoft.Playwright;

namespace Framework.Core.Testing;

/// <summary>
/// Manages a Playwright browser session for non-BDD / classical tests (TDD, xUnit, NUnit, MSTest).
/// Handles isolated browser contexts, Actor creation, automated evidence collection,
/// video lifecycle (OnFailure/Always/Disabled), and native Allure report registration.
/// </summary>
public sealed class WebTestSession : IAsyncDisposable {
    private readonly string _suiteName;
    private readonly string _testName;
    private readonly DateTimeOffset _start;
    private readonly List<string> _videoPaths = [];
    private readonly List<IBrowserContext> _secondaryContexts = [];
    private readonly List<AllureReportHelper.AllureStep> _steps = [];

    private IBrowserContext? _context;
    private IPage? _page;
    private Actor? _actor;
    private bool _completed;
    private bool _disposed;

    private WebTestSession(string suiteName, string testName) {
        _suiteName = suiteName;
        _testName = testName;
        _start = DateTimeOffset.UtcNow;
    }

    /// <summary>Gets the primary Playwright page for this test session.</summary>
    public IPage Page => _page ?? throw new InvalidOperationException("Session has not been initialized. Call StartAsync() first.");

    /// <summary>Gets the primary Screenplay actor with <see cref="BrowseTheWeb"/> ability.</summary>
    public Actor Actor => _actor ?? throw new InvalidOperationException("Session has not been initialized. Call StartAsync() first.");

    /// <summary>
    /// Starts a new isolated web test session.
    /// </summary>
    /// <param name="testName">Name of the test (e.g. method name).</param>
    /// <param name="suiteName">Name of the test suite (e.g. class name).</param>
    /// <param name="actorName">Name of the primary actor (default: "User").</param>
    /// <returns>A running WebTestSession instance.</returns>
    public static async Task<WebTestSession> StartAsync(
        string? testName = null,
        string? suiteName = null,
        string actorName = "User") {
        var session = new WebTestSession(
            suiteName ?? "Classical Tests",
            testName ?? $"Test_{DateTime.UtcNow:yyyyMMdd_HHmmss}");

        var fw = SettingsManager.FrameworkSettings;
        var device = SettingsManager.DeviceSettings;
        var allure = SettingsManager.AllureSettings;

        var browser = await BrowserSessionManager.Shared.GetBrowserAsync();
        var evidenceDir = ReportService.ResolveEvidenceDirectory(allure, fw.Evidence);

        var contextOptions = new BrowserNewContextOptions {
            ViewportSize = new ViewportSize { Width = device.ViewportWidth, Height = device.ViewportHeight },
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36",
            RecordVideoDir = fw.Evidence.Video == VideoMode.Disabled
                ? null
                : Path.Combine(evidenceDir, "videos"),
        };
        BrowserOptionsHook.ConfigureContext?.Invoke(contextOptions);
        session._context = await browser.NewContextAsync(contextOptions);

        await session._context.AddInitScriptAsync("Object.defineProperty(navigator, 'webdriver', {get: () => undefined});");
        session._context.SetDefaultTimeout(fw.Timeouts.ActionTimeoutSeconds * 1000f);
        session._context.SetDefaultNavigationTimeout(fw.Timeouts.NavigationTimeoutSeconds * 1000f);

        if (fw.Evidence.Trace) {
            await session._context.Tracing.StartAsync(new TracingStartOptions {
                Screenshots = true,
                Snapshots = true
            });
        }

        session._page = await session._context.NewPageAsync();
        if (BrowserOptionsHook.OnPageCreatedAsync != null)
            await BrowserOptionsHook.OnPageCreatedAsync(session._page);

        if (fw.Evidence.Video != VideoMode.Disabled && session._page.Video is not null) {
            try {
                var vPath = await session._page.Video.PathAsync();
                session._videoPaths.Add(vPath);
            }
            catch { /* video not ready */ }
        }

        session._actor = new Actor(actorName);
        session._actor.WhoCan(BrowseTheWeb.With(session._page));

        return session;
    }

    /// <summary>
    /// Creates a secondary actor with its own isolated browser context (session/cookies)
    /// to test multi-user interactions (e.g. Admin + User).
    /// </summary>
    /// <param name="name">Name of the persona.</param>
    /// <returns>A secondary Actor with its own page.</returns>
    public async Task<Actor> CreatePersonaAsync(string name) {
        var fw = SettingsManager.FrameworkSettings;
        var device = SettingsManager.DeviceSettings;
        var allure = SettingsManager.AllureSettings;

        var browser = await BrowserSessionManager.Shared.GetBrowserAsync();
        var evidenceDir = ReportService.ResolveEvidenceDirectory(allure, fw.Evidence);

        var context = await browser.NewContextAsync(new BrowserNewContextOptions {
            ViewportSize = new ViewportSize { Width = device.ViewportWidth, Height = device.ViewportHeight },
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36",
            RecordVideoDir = fw.Evidence.Video == VideoMode.Disabled
                ? null
                : Path.Combine(evidenceDir, "videos"),
        });

        await context.AddInitScriptAsync("Object.defineProperty(navigator, 'webdriver', {get: () => undefined});");
        context.SetDefaultTimeout(fw.Timeouts.ActionTimeoutSeconds * 1000f);
        context.SetDefaultNavigationTimeout(fw.Timeouts.NavigationTimeoutSeconds * 1000f);

        var page = await context.NewPageAsync();
        if (fw.Evidence.Video != VideoMode.Disabled && page.Video is not null)
            try { _videoPaths.Add(await page.Video.PathAsync()); } catch { }

        _secondaryContexts.Add(context);

        var actor = new Actor(name);
        actor.WhoCan(BrowseTheWeb.With(page));
        return actor;
    }

    /// <summary>
    /// Records a named step for reporting in Allure.
    /// </summary>
    public void AddStep(string name, bool passed, long startMs, long stopMs, string? error = null) {
        _steps.Add(new AllureReportHelper.AllureStep(
            name,
            passed ? "passed" : "failed",
            startMs,
            stopMs,
            StatusDetail: error is not null ? new AllureReportHelper.AllureStepDetail(error, null) : null));
    }

    /// <summary>
    /// Completes the test session, collecting failure evidence (screenshots/traces),
    /// managing video files, and writing the test result to Allure.
    /// </summary>
    /// <param name="passed">Whether the test passed.</param>
    /// <param name="errorMessage">Error message if failed.</param>
    /// <param name="errorTrace">Stack trace if failed.</param>
    /// <param name="frameworkName">Framework label (e.g. "xunit", "nunit", "mstest").</param>
    public async Task CompleteAsync(
        bool passed,
        string? errorMessage = null,
        string? errorTrace = null,
        string frameworkName = "classical") {
        if (_completed) return;
        _completed = true;

        var fw = SettingsManager.FrameworkSettings;
        var allure = SettingsManager.AllureSettings;
        var attachments = new List<string>();

        // 1. Capture screenshot / trace if failed
        if (_page is not null && _context is not null) {
            var artifacts = await EvidenceCollector.CollectAsync(
                _page,
                _context,
                fw.Evidence,
                _testName,
                !passed);

            if (artifacts.ScreenshotPath is not null) attachments.Add(artifacts.ScreenshotPath);
            if (artifacts.TracePath is not null) attachments.Add(artifacts.TracePath);
        }

        // 2. Close contexts so Playwright flushes video
        foreach (var sec in _secondaryContexts)
            try { await sec.CloseAsync(); } catch { }
        _secondaryContexts.Clear();

        try { if (_context is not null) await _context.CloseAsync(); } catch { }

        // 3. Handle videos per VideoMode
        if (fw.Evidence.Video == VideoMode.OnFailure)
        {
            if (!passed)
            {
                foreach (var v in _videoPaths)
                    if (File.Exists(v)) attachments.Add(v);
            }
            else
            {
                foreach (var v in _videoPaths)
                    try { if (File.Exists(v)) File.Delete(v); } catch { }
                _videoPaths.Clear();
            }
        }
        else if (fw.Evidence.Video == VideoMode.Always)
        {
            foreach (var v in _videoPaths)
                if (File.Exists(v)) attachments.Add(v);
        }
        else
        {
            foreach (var v in _videoPaths)
                try { if (File.Exists(v)) File.Delete(v); } catch { }
            _videoPaths.Clear();
        }

        // 4. Write native Allure result
        var resultsDir = ReportService.ResolveRunResultsDirectory(allure, fw.Reporting);
        var durationMs = (long)(DateTimeOffset.UtcNow - _start).TotalMilliseconds;

        AllureReportHelper.WriteResult(
            resultsDir,
            _suiteName,
            _testName,
            passed,
            durationMs,
            tags: ["classical", frameworkName],
            errorMessage: errorMessage,
            errorTrace: errorTrace,
            attachments: attachments,
            steps: _steps.Count > 0 ? _steps.ToArray() : null,
            testFramework: frameworkName);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync() {
        if (_disposed) return;
        _disposed = true;

        if (!_completed)
            await CompleteAsync(passed: true);
    }
}
