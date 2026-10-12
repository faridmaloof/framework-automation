using Framework.Core.ScreenPlay.Pages;

namespace Framework.Core.POM;

/// <summary>
/// Backwards-compatible alias for <see cref="PageObject"/>.
/// Modern Screenplay tests should inherit directly from <see cref="PageObject"/>.
/// </summary>
public abstract class BasePage : PageObject { }
