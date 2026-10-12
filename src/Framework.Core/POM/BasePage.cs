using Framework.Core.ScreenPlay.Locators;
using Framework.Core.ScreenPlay.Tasks;

namespace Framework.Core.POM;

/// <summary>
/// Base for Page Objects in the ScreenPlay-flavoured POM pattern.
///
/// Subclasses describe three things:
///   <list type="bullet">
///     <item><description>The page identity (<see cref="Name"/>, <see cref="Url"/>).</description></item>
///     <item><description>The <see cref="ITarget"/>s exposed for use by Tasks/Interactions/Questions.</description></item>
///     <item><description>Domain-level helpers used by tests (navigation, "open page", ...).</description></item>
///   </list>
///
/// Implementations are encouraged to expose the targets as <c>public static readonly</c>
/// fields (or records) so they can be passed into Interactions/Questions by reference,
/// rather than hand-written selector strings scattered across tests.
/// </summary>
public abstract class BasePage
{
    /// <summary>
    /// Human-readable page name (e.g. <c>"Google home"</c>).
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Canonical URL for the page. Used by <see cref="Open"/>.
    /// </summary>
    public abstract string Url { get; }

    /// <summary>
    /// Optional page title assertion used as a soft readiness check
    /// after navigation. When null, no implicit title validation is performed.
    /// </summary>
    public virtual string? ExpectedTitle => null;

    /// <summary>
    /// All <see cref="ITarget"/>s declared by the page. Default implementation
    /// returns an empty sequence; subclasses override to surface the targets
    /// they expose (useful for documentation, validation and reporting).
    /// </summary>
    public virtual IEnumerable<ITarget> Targets => [];

    /// <summary>
    /// Returns the navigation Task (ScreenPlay <see cref="NavigateToUrl"/>)
    /// that opens the page. Acts as a domain helper:
    /// <c>Actor.AttemptsToAsync(GoogleHome.Open());</c>
    /// </summary>
    /// <param name="waitFor">
    /// Optional Playwright wait-until condition
    /// (<c>"load"</c> (default), <c>"domcontentloaded"</c>, <c>"networkidle"</c>).
    /// </param>
    public virtual NavigateToUrl Open(string? waitFor = null)
        => NavigateToUrl.To(Url, waitFor);

    // ---- Factory helpers used by subclasses to declare targets ----

    /// <summary>Declares a CSS-selector target.</summary>
    protected static Target Css(string name, string selector, string description = "", TimeSpan? timeout = null)
        => Target.Css(name, selector, description, timeout);

    /// <summary>Declares an XPath target.</summary>
    protected static Target XPath(string name, string selector, string description = "", TimeSpan? timeout = null)
        => Target.XPath(name, selector, description, timeout);

    /// <summary>Declares a text-based target.</summary>
    protected static Target ByText(string name, string text, string description = "", TimeSpan? timeout = null)
        => Target.ByText(name, text, description, timeout);

    /// <summary>Declares a <c>data-testid</c>-based target.</summary>
    protected static Target ByTestId(string name, string testId, string description = "", TimeSpan? timeout = null)
        => Target.ByTestId(name, testId, description, timeout);

    /// <summary>Declares a label-based target.</summary>
    protected static Target ByLabel(string name, string label, string description = "", TimeSpan? timeout = null)
        => Target.ByLabel(name, label, description, timeout);

    /// <summary>Declares a placeholder-based target.</summary>
    protected static Target ByPlaceholder(string name, string placeholder, string description = "", TimeSpan? timeout = null)
        => Target.ByPlaceholder(name, placeholder, description, timeout);
}
