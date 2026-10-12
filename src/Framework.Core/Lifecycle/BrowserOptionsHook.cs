using Microsoft.Playwright;

namespace Framework.Core.Lifecycle;

/// <summary>
/// Extensibility hooks for Playwright browser launch and context configuration.
/// Allows consumers of the library to inject custom browser arguments, proxies,
/// permissions, geolocation, HTTP headers, or storage state without modifying Core.
/// </summary>
public static class BrowserOptionsHook {
    /// <summary>
    /// Invoked before launching the browser. Use this to add custom CLI arguments,
    /// proxy settings, or downloads configuration.
    /// </summary>
    public static Action<BrowserTypeLaunchOptions>? ConfigureLaunch { get; set; }

    /// <summary>
    /// Invoked before creating a browser context. Use this to configure permissions,
    /// geolocation, extra HTTP headers, storage state, or bypass CSP.
    /// </summary>
    public static Action<BrowserNewContextOptions>? ConfigureContext { get; set; }

    /// <summary>
    /// Invoked immediately after a new page is created. Use this to attach event listeners,
    /// intercept routes, or inject scripts.
    /// </summary>
    public static Func<IPage, Task>? OnPageCreatedAsync { get; set; }

    /// <summary>
    /// Resets all configured hooks to null.
    /// </summary>
    public static void Reset()
    {
        ConfigureLaunch = null;
        ConfigureContext = null;
        OnPageCreatedAsync = null;
    }
}
