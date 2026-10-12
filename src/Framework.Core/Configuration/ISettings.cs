namespace Framework.Core.Configuration;

/// <summary>
/// Framework settings interface.
/// NOTE: headless mode is NOT part of the framework section — it belongs to
/// <see cref="IDeviceSettings"/> (single source of truth, section "Device").
/// </summary>
public interface IFrameworkSettings {
    /// <summary>
    /// Gets the environment name.
    /// </summary>
    string Environment { get; }

    /// <summary>
    /// Gets the culture.
    /// </summary>
    string Culture { get; }
}

/// <summary>
/// Device settings interface.
/// </summary>
public interface IDeviceSettings {
    /// <summary>
    /// Gets the browser type.
    /// </summary>
    BrowserType Browser { get; }

    /// <summary>
    /// Gets whether to run in headless mode.
    /// </summary>
    bool Headless { get; }

    /// <summary>
    /// Gets the viewport width.
    /// </summary>
    int ViewportWidth { get; }

    /// <summary>
    /// Gets the viewport height.
    /// </summary>
    int ViewportHeight { get; }
}

/// <summary>
/// Evidence settings interface.
/// </summary>
public interface IEvidenceSettings {
    /// <summary>
    /// Gets the screenshot capture mode.
    /// </summary>
    ScreenshotMode Screenshot { get; }

    /// <summary>
    /// Gets the video recording mode.
    /// </summary>
    VideoMode Video { get; }

    /// <summary>
    /// Gets whether to enable trace recording.
    /// </summary>
    bool Trace { get; }

    /// <summary>
    /// Gets the evidence storage path.
    /// </summary>
    string StoragePath { get; }
}
