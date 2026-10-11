namespace Framework.Core.ScreenPlay.Locators;

/// <summary>
/// Default immutable <see cref="ITarget"/> implementation.
/// Immutable (record) so targets can be safely shared across threads
/// and reused by multiple actors / tests.
/// </summary>
/// <remarks>
/// Use the factory methods (<see cref="Css(string, string, string, TimeSpan?)"/>,
/// <see cref="ByText(string, string, string, TimeSpan?)"/>, ...) instead of the
/// primary constructor unless you have specific needs. Targets declared inside
/// a <see cref="Framework.Core.ScreenPlay.Pages.PageObject"/> or a
/// <see cref="Framework.Core.ScreenPlay.Pages.Component"/> get their
/// <see cref="Page"/> / <see cref="Parent"/> context stamped automatically.
/// </remarks>
/// <param name="Name">Human-readable name (required).</param>
/// <param name="Selector">Selector expression (required).</param>
/// <param name="Description">Optional semantic description.</param>
/// <param name="Timeout">Optional per-target timeout.</param>
/// <param name="Strategy">Optional selector strategy.</param>
/// <param name="Page">Owning page name (report context).</param>
/// <param name="Parent">Enclosing component target (scope), when any.</param>
public sealed record Target(string Name,
                            string Selector,
                            string Description = "",
                            TimeSpan? Timeout = null,
                            string? Strategy = null,
                            string? Page = null,
                            ITarget? Parent = null) : ITarget {
    /// <inheritdoc/>
    public string Location {
        get {
            var parts = new List<string>();
            for (var ancestor = Parent; ancestor is not null; ancestor = ancestor.Parent)
                parts.Insert(0, ancestor.Name);
            if (!string.IsNullOrEmpty(Page)) parts.Add(Page!);
            return string.Join(" › ", parts);
        }
    }

    /// <summary>
    /// Returns a human-readable representation for logs and reports:
    /// <c>"Usuario (Login)"</c> or <c>"Enviar (Modal de confirmación › Login)"</c>.
    /// The raw selector is intentionally omitted — it is a technical detail
    /// available through <see cref="Selector"/>.
    /// </summary>
    public override string ToString() {
        var location = Location;
        return string.IsNullOrEmpty(location) ? Name : $"{Name} ({location})";
    }

    // ---- Factory helpers (preferred over the primary constructor) ----

    /// <summary>Creates a CSS-selector target.</summary>
    public static Target Css(
        string name,
        string selector,
        string description = "",
        TimeSpan? timeout = null)
        => new(name, selector, description, timeout, "css");

    /// <summary>Creates an XPath target.</summary>
    public static Target XPath(
        string name,
        string selector,
        string description = "",
        TimeSpan? timeout = null)
        => new(name, selector, description, timeout, "xpath");

    /// <summary>Creates a <c>text=</c> target (Playwright semantic text locators).</summary>
    public static Target ByText(
        string name,
        string text,
        string description = "",
        TimeSpan? timeout = null)
        => new(name, text, description, timeout, "text");

    /// <summary>Creates a <c>test-id=</c> target.</summary>
    public static Target ByTestId(
        string name,
        string testId,
        string description = "",
        TimeSpan? timeout = null)
        => new(name, testId, description, timeout, "test-id");

    /// <summary>Creates a label-based target.</summary>
    public static Target ByLabel(
        string name,
        string label,
        string description = "",
        TimeSpan? timeout = null)
        => new(name, label, description, timeout, "label");

    /// <summary>Creates a placeholder-based target.</summary>
    public static Target ByPlaceholder(
        string name,
        string placeholder,
        string description = "",
        TimeSpan? timeout = null)
        => new(name, placeholder, description, timeout, "placeholder");

    /// <summary>Creates a <c>role=</c> target using Playwright's ARIA role engine.</summary>
    public static Target ByRole(
        string name,
        string role,
        string description = "",
        TimeSpan? timeout = null)
        => new(name, role, description, timeout, "role");
}
