using Framework.Core.Configuration;
using Microsoft.Playwright;

namespace Framework.Core.Lifecycle;

/// <summary>
/// Owns the process-wide Playwright browser for a test run.
/// <para>
/// The browser is launched <b>lazily</b> (only when the first web scenario
/// needs it) and <b>shared</b> across scenarios for speed; every scenario
/// still gets its own isolated <see cref="IBrowserContext"/> (a "session").
/// Pure-API runs never launch a browser at all.
/// </para>
/// <para>
/// This is a run-scoped singleton (like an xUnit collection fixture): it
/// deliberately shares the browser <em>process</em>, never a session. It is
/// thread-safe — parallel scenarios may call <see cref="GetBrowserAsync"/>
/// concurrently and only one browser is ever created.
/// </para>
/// </summary>
public sealed class BrowserSessionManager
{
    /// <summary>
    /// The singleton instance of <see cref="BrowserSessionManager"/>. It is created lazily on first access to ensure that the browser is only launched when needed.
    /// </summary>
    private static readonly Lazy<BrowserSessionManager> Singleton =
        new(() => new BrowserSessionManager());

    /// <summary>Gets the shared, run-scoped instance.</summary>
    public static BrowserSessionManager Shared => Singleton.Value;

    /// <summary>
    /// A semaphore to ensure that only one thread can launch or shut down the browser at a time. This prevents race conditions when multiple scenarios attempt to access the browser concurrently.
    /// </summary>
    private readonly SemaphoreSlim _gate = new(1, 1);
    /// <summary>
    /// The Playwright instance used to launch the browser. It is created lazily when the browser is first requested.
    /// </summary>
    private IPlaywright? _playwright;
    /// <summary>
    /// The shared browser instance for the test run. It is created lazily when the first scenario requests it and is closed when the run ends.
    /// </summary>
    private IBrowser? _browser;

    /// <summary>
    /// Initializes a new instance of the <see cref="BrowserSessionManager"/> class. The constructor is private to enforce the singleton pattern, ensuring that only one instance exists for the entire test run.
    /// </summary>
    private BrowserSessionManager() { }

    /// <summary>Gets whether a browser is currently running.</summary>
    public bool IsRunning => _browser is not null;

    /// <summary>
    /// Returns the shared browser, launching it on first use. Thread-safe.
    /// </summary>
    public async Task<IBrowser> GetBrowserAsync()
    {
        if (_browser is not null) return _browser;

        await _gate.WaitAsync();
        try
        {
            if (_browser is not null) return _browser;

            var device = SettingsManager.DeviceSettings;
            _playwright = await Playwright.CreateAsync();
            var engine = MapEngine(_playwright, device.Browser);
            // Chromium-only anti-automation measure: hide navigator.webdriver so
            // bot checks (e.g. Google's) do not trivially flag the session as
            // automated. Mirrors MapEngine's engine set (Firefox/WebKit/Safari excluded).
            var chromium = device.Browser is not (Configuration.BrowserType.Firefox
                or Configuration.BrowserType.WebKit
                or Configuration.BrowserType.Safari);
            _browser = await engine.LaunchAsync(new BrowserTypeLaunchOptions {
                Headless = device.Headless,
                Args = chromium ? ["--disable-blink-features=AutomationControlled"] : [],
            });
            return _browser;
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>Closes the shared browser and releases Playwright, if running.</summary>
    public async Task ShutdownAsync()
    {
        await _gate.WaitAsync();
        try
        {
            if (_browser is not null)
                await _browser.CloseAsync();
            _playwright?.Dispose();
            _browser = null;
            _playwright = null;
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>
    /// Maps the configured browser type to the corresponding Playwright engine. This method is used to determine which Playwright engine to use based on the configuration settings.
    /// </summary>
    /// <param name="playwright">The Playwright instance used to launch the browser.</param>
    /// <param name="browser">The configured browser type.</param>
    /// <returns>The corresponding Playwright engine.</returns>
    private static IBrowserType MapEngine(IPlaywright playwright, Configuration.BrowserType browser) => browser switch
    {
        Configuration.BrowserType.Firefox => playwright.Firefox,
        Configuration.BrowserType.WebKit or Configuration.BrowserType.Safari => playwright.Webkit,
        _ => playwright.Chromium,
    };
}
