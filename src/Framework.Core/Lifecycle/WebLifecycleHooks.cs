using Framework.Core.BDD;
using Framework.Core.Configuration;
using Framework.Core.ScreenPlay;
using Microsoft.Playwright;
using Reqnroll;

namespace Framework.Core.Lifecycle;

/// <summary>
/// Core-owned lifecycle for <b>web</b> scenarios (tagged <c>@web</c>).
/// <para>
/// This binding ships in the framework, so a test project does not implement
/// any browser/evidence lifecycle — it only declares <c>@web</c> scenarios.
/// It reuses <see cref="BrowserSessionManager"/> (one shared browser per run,
/// launched lazily) and <see cref="EvidenceCollector"/> (evidence policy),
/// and reads every value (headless, viewport, timeouts, evidence) from Core's
/// <see cref="SettingsManager"/>.
/// </para>
/// <para>
/// API-only runs never enter these hooks, so they never pay for a browser.
/// </para>
/// </summary>
/// <remarks>Initializes a new instance of the <see cref="WebLifecycleHooks"/> class.</remarks>
/// <param name="scenarioContext">The Reqnroll scenario context.</param>
[Binding]
public sealed class WebLifecycleHooks(ScenarioContext scenarioContext)
{
    /// <summary>
    /// The scenario's isolated browser context (session). It is stored in the scenario context for easy access by steps and tasks.
    /// </summary>
    private IBrowserContext? _context;
    /// <summary>
    /// The scenario's primary page (the one shared by the primary persona). It is stored in the scenario context for easy access by steps and tasks.
    /// </summary>
    private IPage? _page;

    // Secondary personas created via ScenarioActor.PersonaAsync get their OWN
    // isolated browser context (session). Tracked so we can close them all.
    private readonly List<IBrowserContext> _secondaryContexts = [];
    private readonly List<string> _videoPaths = [];

    /// <summary>
    /// Runs per scenario but only acts on <c>@web</c>-tagged scenarios: it
    /// creates an isolated browser context (viewport, timeouts, evidence per
    /// configuration) and the scenario's personas. Each context is a fresh
    /// "session", so scenarios never share cookies or storage.
    /// Non-web scenarios (e.g. <c>@api</c>) return immediately and never pay
    /// for a browser.
    /// </summary>
    [BeforeScenario]
    public async Task BeforeScenario()
    {
        if (!IsWeb()) return;

        var fw = SettingsManager.FrameworkSettings;
        var device = SettingsManager.DeviceSettings;

        var browser = await BrowserSessionManager.Shared.GetBrowserAsync();
        var evidenceDir = ReportService.ResolveEvidenceDirectory(SettingsManager.AllureSettings, fw.Evidence);
        _context = await browser.NewContextAsync(new BrowserNewContextOptions {
            ViewportSize = new ViewportSize { Width = device.ViewportWidth, Height = device.ViewportHeight },
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36",
            RecordVideoDir = fw.Evidence.Video == VideoMode.Disabled
                ? null
                : Path.Combine(evidenceDir, "videos"),
        });
        await _context.AddInitScriptAsync("Object.defineProperty(navigator, 'webdriver', {get: () => undefined});");
        _context.SetDefaultTimeout(fw.Timeouts.ActionTimeoutSeconds * 1000f);
        _context.SetDefaultNavigationTimeout(fw.Timeouts.NavigationTimeoutSeconds * 1000f);

        if (fw.Evidence.Trace)
            await _context.Tracing.StartAsync(new TracingStartOptions { Screenshots = true, Snapshots = true });

        _page = await _context.NewPageAsync();
        scenarioContext["Page"] = _page;
        scenarioContext["ScenarioStartTime"] = DateTimeOffset.UtcNow;
        scenarioContext["StepIndex"] = 0;

        // Resolve the video path once (it exists from page creation). The file
        // is finalized when the context closes (AfterScenario) and registered
        // as a scenario-level attachment by RunHistoryHooks — no folder scan.
        try
        {
            if (fw.Evidence.Video != VideoMode.Disabled && _page.Video is not null)
            {
                var vPath = await _page.Video.PathAsync();
                _videoPaths.Add(vPath);
                scenarioContext["VideoPath"] = vPath;
            }
        }
        catch { /* video unavailable */ }

        // Persona factory: any new subject found in a step ("an administrator",
        // "the system"...) is built through this factory with full abilities.
        ScenarioActor.RegisterPersonaFactory(scenarioContext, name => {
            var actor = new Actor(name);
            actor.WhoCan(BrowseTheWeb.With(_page!));
            return actor;
        });

        // Async factory: each SECONDARY persona gets its OWN isolated browser
        // context (its own cookies/session), so two actors can act on the same
        // resource through different sessions (spec §6). The primary persona
        // ("user"/"they") still shares the scenario page above.
        ScenarioActor.RegisterPersonaFactoryAsync(scenarioContext, async name =>
        {
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
            {
                try { _videoPaths.Add(await page.Video.PathAsync()); } catch { }
            }
            _secondaryContexts.Add(context);

            var actor = new Actor(name);
            actor.WhoCan(BrowseTheWeb.With(page));
            return actor;
        });

        // Primary persona: "the user"/"they" resolve to this actor by default.
        var primary = new Actor("User");
        primary.WhoCan(BrowseTheWeb.With(_page));
        scenarioContext["Actor"] = primary;
        scenarioContext["BrowseTheWeb"] = primary.AbilityTo<BrowseTheWeb>();
        ScenarioActor.RegisterPersona(scenarioContext, "user", primary);
    }

    /// <summary>
    /// Runs after each scenario but only acts on <c>@web</c>-tagged scenarios:
    /// captures failure evidence (screenshot/trace), registers the artifact
    /// paths in the scenario context, and closes the isolated context. Non-web
    /// scenarios return immediately. Lower Order than the universal
    /// <c>[AfterScenario]</c> so it runs BEFORE the result is written (Reqnroll
    /// runs After hooks in ascending Order), letting RunHistoryHooks attach
    /// THIS scenario's video/failure-screenshot/trace by path.
    /// </summary>
    [AfterScenario(Order = -100)]
    public async Task AfterScenario()
    {
        if (!IsWeb()) return;

        var fw = SettingsManager.FrameworkSettings;
        var title = scenarioContext.ScenarioInfo.Title;
        var failed = scenarioContext.TestError != null;

        // Evidence only (screenshots/traces/video). Actor disposal is done for
        // every scenario (web and API) in RunHistoryHooks.AfterScenario.
        // Artifact paths are registered so the result writer attaches only
        // THIS scenario's files (no shared-folder scanning, no leakage).
        if (_page is not null && _context is not null)
        {
            var artifacts = await EvidenceCollector.CollectAsync(_page, _context, fw.Evidence, title, failed);
            if (artifacts.ScreenshotPath is not null) scenarioContext["FailureScreenshotPath"] = artifacts.ScreenshotPath;
            if (artifacts.TracePath is not null) scenarioContext["TracePath"] = artifacts.TracePath;
        }

        // Close the isolated contexts of any secondary personas first, then the
        // scenario's primary context. Closing an already-closed context is safe.
        foreach (var secondary in _secondaryContexts)
            try { await secondary.CloseAsync(); } catch { /* already closed */ }
        _secondaryContexts.Clear();

        try { if (_context != null) await _context.CloseAsync(); }
        catch { /* already closed by an ability */ }

        scenarioContext["VideoPaths"] = _videoPaths;
        if (_videoPaths.Count > 0) scenarioContext["VideoPath"] = _videoPaths[0];
    }

    private bool IsWeb() =>
        scenarioContext.ScenarioInfo.Tags.Contains("web", StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Runs once after all scenarios finish: shuts down the shared browser if it was ever launched. Non-web runs never launch a browser, so this is a no-op for them.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [AfterTestRun]
    public static Task AfterTestRun()
        => BrowserSessionManager.Shared.ShutdownAsync();
}

/// <summary>
/// Extension methods for <see cref="Reqnroll.ScenarioContext"/> that make it
/// easier for the lifecycle hooks to access per-step metadata and the
/// scenario's evidence root.
/// </summary>
public static class ScenarioContextExtensions
{
    /// <summary>
    /// The zero-based step index of the currently executing step, as tracked
    /// by <see cref="Framework.Core.Lifecycle.RunHistoryHooks"/>. Falls back
    /// to 0 when the context was not given an explicit index.
    /// </summary>
    public static int GetStepIndex(this ScenarioContext context) =>
        context.ContainsKey("StepIndex") ? (int)context["StepIndex"] : 0;

    /// <summary>
    /// The human-readable name of the currently executing step, or "step" when
    /// the context does not carry it. This is used only as a file-name token.
    /// </summary>
    public static string GetStepName(this ScenarioContext context) =>
        context.GetString("StepName") ?? "step";

    /// <summary>
    /// Returns the value of a key in the scenario context, or null when the
    /// key is absent or the value is not a string.
    /// </summary>
    public static string? GetString(this ScenarioContext context, string key) =>
        context.TryGetValue(key, out var value) && value is string s ? s : null;

    /// <summary>
    /// The per-scenario evidence root. Screenshots (web) and http logs (API)
    /// are written under <c>{root}/step-evidence/{index}</c>, so every scenario
    /// owns a private folder and artifacts never leak across scenarios.
    /// Stable within a scenario: the id is generated once and cached in the
    /// scenario context.
    /// </summary>
    public static string ScenarioEvidenceRoot(this ScenarioContext context)
    {
        var baseDir = Framework.Core.Lifecycle.ReportService.ResolveEvidenceDirectory();
        if (!context.TryGetValue("ScenarioEvidenceId", out var idObj) || idObj is not string id)
        {
            id = Guid.NewGuid().ToString("N")[..8];
            context["ScenarioEvidenceId"] = id;
        }
        return Path.Combine(baseDir, "scenarios", id);
    }
}
