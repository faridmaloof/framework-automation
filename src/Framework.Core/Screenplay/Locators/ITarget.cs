namespace Framework.Core.ScreenPlay.Locators;

/// <summary>
/// Strongly-typed UI locator following the Screenplay pattern.
/// A <see cref="ITarget"/> carries the metadata needed to identify an
/// element in the UI (selector, human name, description, timeout, strategy)
/// plus the <em>context</em> required to produce human-readable reports:
/// the page it belongs to and, optionally, the enclosing component scope.
///
/// Targets are declared once per page inside <see cref="Pages.PageObject"/>
/// (or <see cref="Pages.Component"/>) subclasses and are consumed by
/// Interactions, Tasks and Questions. Reports and failures always reference
/// <see cref="Name"/> + <see cref="Location"/> — never the raw selector.
/// </summary>
public interface ITarget
{
    /// <summary>
    /// Human-readable name used in reports, traces and error messages
    /// (e.g. <c>"Usuario"</c>, <c>"Botón ingresar"</c>).
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Raw selector expression (CSS, XPath, text, role, test-id, ...).
    /// The interpretation depends on <see cref="Strategy"/>.
    /// Technical detail only — never shown as the primary failure reference.
    /// </summary>
    string Selector { get; }

    /// <summary>
    /// Semantic description of what the element represents in the domain.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Optional per-target timeout. When null, callers fall back to the
    /// actor / framework defaults.
    /// </summary>
    TimeSpan? Timeout { get; }

    /// <summary>
    /// Selector strategy. Common values: <c>css</c>, <c>xpath</c>,
    /// <c>text</c>, <c>role</c>, <c>label</c>, <c>placeholder</c>,
    /// <c>test-id</c>. When null, the consumer engine interprets the
    /// selector as CSS by default.
    /// </summary>
    string? Strategy { get; }

    /// <summary>
    /// Name of the page this target belongs to (e.g. <c>"Login"</c>).
    /// Stamped automatically when the target is declared inside a
    /// <see cref="Pages.PageObject"/>. Used in human-readable messages:
    /// <c>"Usuario (Login)"</c>.
    /// </summary>
    string? Page { get; }

    /// <summary>
    /// Optional enclosing target when this target lives inside a reusable
    /// UI component (header, modal, table). Resolution is scoped through
    /// the chain, from the outermost component down to this target.
    /// </summary>
    ITarget? Parent { get; }

    /// <summary>
    /// Human-readable location of the target: the component path (if any)
    /// followed by the page name, e.g. <c>"Cabecera principal › Login"</c>.
    /// Empty when neither scope nor page is known.
    /// </summary>
    string Location { get; }
}