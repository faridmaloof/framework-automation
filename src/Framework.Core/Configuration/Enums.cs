namespace Framework.Core.Configuration;

/// <summary>
/// Screenshot capture mode.
/// </summary>
public enum ScreenshotMode
{
    /// <summary>
    /// No screenshots.
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// Capture only on failure.
    /// </summary>
    OnFailure = 1,

    /// <summary>
    /// Capture before each step.
    /// </summary>
    PreStep = 2,

    /// <summary>
    /// Capture after each step.
    /// </summary>
    PostStep = 3,

    /// <summary>
    /// Capture before and after each step.
    /// </summary>
    PreAndPost = 4,

    /// <summary>
    /// Always capture (Pre + Post + OnFailure).
    /// </summary>
    Always = 5
}

/// <summary>
/// Video recording mode.
/// </summary>
public enum VideoMode
{
    /// <summary>
    /// No video recording.
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// Record only on failure.
    /// </summary>
    OnFailure = 1,

    /// <summary>
    /// Always record video.
    /// </summary>
    Always = 2
}

/// <summary>
/// Browser type.
/// </summary>
public enum BrowserType
{
    /// <summary>
    /// Chromium browser.
    /// </summary>
    Chromium = 0,

    /// <summary>
    /// Firefox browser.
    /// </summary>
    Firefox = 1,

    /// <summary>
    /// WebKit browser.
    /// </summary>
    WebKit = 2
}
