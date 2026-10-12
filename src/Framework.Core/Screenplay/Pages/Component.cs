using Framework.Core.ScreenPlay.Locators;

namespace Framework.Core.ScreenPlay.Pages;

/// <summary>
/// A reusable UI block (header, footer, modal, data table, search box...)
/// that appears on one or more pages.
///
/// A component owns a <see cref="Root"/> target (the block itself) and
/// declares its own targets <b>scoped under that root</b>: resolution is
/// chained, so selectors stay short and unambiguous
/// (<c>#modal-confirmación → button</c> instead of a page-wide XPath).
///
/// Components are the "reusable pieces" of the UI layer: Tasks compose
/// Interactions over component targets, and components themselves are
/// shared across PageObjects without duplicating locators.
///
/// Example:
/// <code>
/// public sealed class ConfirmationModal : Component
/// {
///     public static readonly ConfirmationModal Instance =
///         new(Target.Css("modal de confirmación", "[data-testid='confirm-modal']"));
///
///     public ITarget AcceptButton => Css("botón aceptar", "[data-testid='accept']");
/// }
/// </code>
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Component"/> class.
/// </remarks>
/// <param name="root">The target that identifies the component's root element.</param>
public abstract class Component(ITarget root)
{

    /// <summary>
    /// The target identifying the component's root element in the DOM.
    /// </summary>
    public ITarget Root { get; } = root ?? throw new ArgumentNullException(nameof(root));

    /// <summary>
    /// Human-readable component name. Defaults to the root target's name.
    /// </summary>
    public virtual string Name => Root.Name;

    /// <summary>
    /// Page the component belongs to (report context), inherited from the root target.
    /// </summary>
    public virtual string? Page => Root.Page;

    /// <summary>
    /// The component's root target and all targets it declares.
    /// Useful for documentation and validation.
    /// </summary>
    public virtual IEnumerable<ITarget> Targets => [Root];

    // ---- Factory helpers: targets scoped under this component's root ----

    /// <summary>Declares a CSS-selector target inside this component.</summary>
    protected Target Css(string name, string selector, string description = "", TimeSpan? timeout = null)
        => Target.Css(name, selector, description, timeout) with { Parent = Root, Page = Root.Page };

    /// <summary>Declares an XPath target inside this component.</summary>
    protected Target XPath(string name, string selector, string description = "", TimeSpan? timeout = null)
        => Target.XPath(name, selector, description, timeout) with { Parent = Root, Page = Root.Page };

    /// <summary>Declares a text-based target inside this component.</summary>
    protected Target ByText(string name, string text, string description = "", TimeSpan? timeout = null)
        => Target.ByText(name, text, description, timeout) with { Parent = Root, Page = Root.Page };

    /// <summary>Declares a <c>data-testid</c>-based target inside this component.</summary>
    protected Target ByTestId(string name, string testId, string description = "", TimeSpan? timeout = null)
        => Target.ByTestId(name, testId, description, timeout) with { Parent = Root, Page = Root.Page };

    /// <summary>Declares a label-based target inside this component.</summary>
    protected Target ByLabel(string name, string label, string description = "", TimeSpan? timeout = null)
        => Target.ByLabel(name, label, description, timeout) with { Parent = Root, Page = Root.Page };

    /// <summary>Declares a placeholder-based target inside this component.</summary>
    protected Target ByPlaceholder(string name, string placeholder, string description = "", TimeSpan? timeout = null)
        => Target.ByPlaceholder(name, placeholder, description, timeout) with { Parent = Root, Page = Root.Page };

    /// <summary>Declares an ARIA-role-based target inside this component.</summary>
    protected Target ByRole(string name, string role, string description = "", TimeSpan? timeout = null)
        => Target.ByRole(name, role, description, timeout) with { Parent = Root, Page = Root.Page };
}