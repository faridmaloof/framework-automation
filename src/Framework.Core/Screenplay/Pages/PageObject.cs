using Framework.Core.Exceptions;
using Framework.Core.ScreenPlay.Locators;
using Framework.Core.ScreenPlay.Tasks;

namespace Framework.Core.ScreenPlay.Pages;

/// <summary>
/// The one base type for locating elements, grouped by page.
///
/// This is where the traditional Page Object Model collapses: a page class
/// is a <b>pure locator catalogue</b> — page identity (name, URL) plus the
/// <see cref="ITarget"/>s it exposes — with <b>no behaviour</b>. Behaviour
/// lives in Screenplay Tasks / Interactions / Questions, which consume the
/// targets declared here. This keeps the "one place per locator" benefit of
/// the POM without the antipattern of page objects that mix selectors,
/// waits, assertions and business logic.
///
/// Every target declared through the protected factories is stamped with the
/// page <see cref="Name"/>, so reports and failures read like
/// <c>'Usuario' (Login)</c> instead of <c>#usuario</c>.
///
/// Example:
/// <code>
/// public sealed class LoginPage : PageObject
/// {
///     public static readonly LoginPage Instance = new();
///     public override string Name => "Login";
///     public override string Url => "https://app.example.com/login";
///
///     public ITarget UserField  => ByLabel("usuario", "Usuario", "Credencial de acceso");
///     public ITarget SubmitButton => Css("botón ingresar", "[data-testid='login-submit']");
///
///     public override IEnumerable&lt;ITarget&gt; Targets => [UserField, SubmitButton];
/// }
/// </code>
/// </summary>
public abstract class PageObject
{
    /// <summary>
    /// Human-readable page name (e.g. <c>"Login"</c>). Stamped on every
    /// target declared by this page and used in reports and failures.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Canonical URL of the page, used by <see cref="Open"/>. Pages that are
    /// reached only through application flow (results, detail views) may
    /// leave this null.
    /// </summary>
    public virtual string? Url => null;

    /// <summary>
    /// Optional expected title used as a soft readiness check after
    /// navigation. When null, no implicit title validation is performed.
    /// </summary>
    public virtual string? ExpectedTitle => null;

    /// <summary>
    /// All <see cref="ITarget"/>s declared by the page. Subclasses override
    /// this to surface their targets for documentation, validation and
    /// reporting; the default is an empty sequence.
    /// </summary>
    public virtual IEnumerable<ITarget> Targets => [];

    /// <summary>
    /// Returns the navigation Task (Screenplay <see cref="NavigateToUrl"/>)
    /// that opens the page: <c>Actor.AttemptsToAsync(LoginPage.Instance.Open());</c>
    /// </summary>
    /// <param name="waitFor">
    /// Optional Playwright wait-until condition: <c>"load"</c> (default),
    /// <c>"domcontentloaded"</c> or <c>"networkidle"</c>.
    /// </param>
    /// <exception cref="FrameworkException">The page does not define a <see cref="Url"/>.</exception>
    public virtual NavigateToUrl Open(string? waitFor = null)
    {
        if (string.IsNullOrWhiteSpace(Url))
            throw new FrameworkException(
                $"Page '{Name}' does not define a Url; navigate to it with an explicit task instead.");
        return NavigateToUrl.To(Url, waitFor);
    }

    // ---- Factory helpers used by subclasses to declare targets ----
    // They stamp the page name so failures read "'Usuario' (Login)".

    /// <summary>Declares a CSS-selector target owned by this page.</summary>
    protected Target Css(string name, string selector, string description = "", TimeSpan? timeout = null)
        => Target.Css(name, selector, description, timeout) with { Page = Name };

    /// <summary>Declares an XPath target owned by this page.</summary>
    protected Target XPath(string name, string selector, string description = "", TimeSpan? timeout = null)
        => Target.XPath(name, selector, description, timeout) with { Page = Name };

    /// <summary>Declares a text-based target owned by this page.</summary>
    protected Target ByText(string name, string text, string description = "", TimeSpan? timeout = null)
        => Target.ByText(name, text, description, timeout) with { Page = Name };

    /// <summary>Declares a <c>data-testid</c>-based target owned by this page.</summary>
    protected Target ByTestId(string name, string testId, string description = "", TimeSpan? timeout = null)
        => Target.ByTestId(name, testId, description, timeout) with { Page = Name };

    /// <summary>Declares a label-based target owned by this page.</summary>
    protected Target ByLabel(string name, string label, string description = "", TimeSpan? timeout = null)
        => Target.ByLabel(name, label, description, timeout) with { Page = Name };

    /// <summary>Declares a placeholder-based target owned by this page.</summary>
    protected Target ByPlaceholder(string name, string placeholder, string description = "", TimeSpan? timeout = null)
        => Target.ByPlaceholder(name, placeholder, description, timeout) with { Page = Name };

    /// <summary>Declares an ARIA-role-based target owned by this page.</summary>
    protected Target ByRole(string name, string role, string description = "", TimeSpan? timeout = null)
        => Target.ByRole(name, role, description, timeout) with { Page = Name };
}