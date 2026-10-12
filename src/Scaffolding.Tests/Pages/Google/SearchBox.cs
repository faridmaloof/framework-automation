using Framework.Core.ScreenPlay.Locators;
using Framework.Core.ScreenPlay.Pages;

namespace Scaffolding.Tests.Pages.Google;

/// <summary>
/// Reusable "search box" UI component (input + submit button inside the search form).
/// Demonstrates how shared UI blocks are modelled once and reused by several
/// pages without duplicating locators. Page classes stamp their own page name
/// on the targets they expose (see <see cref="GoogleHomePage"/>).
/// </summary>
public sealed class SearchBox : Component {
    /// <summary>Shared component instance (stateless).</summary>
    public static readonly SearchBox Instance = new();

    /// <summary>
    /// Private constructor to enforce singleton pattern. Use <see cref="Instance"/> instead.
    /// </summary>
    private SearchBox()
        : base(Target.Css(
            "cuadro de búsqueda",
            "form[action='/search']",
            "Formulario de búsqueda de Google (aparece en varias páginas)"))
    { }

    /// <summary>Campo donde se escribe el término de búsqueda.</summary>
    public Target QueryField => Css("campo de consulta", "[name='q']", "Input del término de búsqueda");

    /// <summary>Botón que envía el formulario de búsqueda.</summary>
    public Target SubmitButton => Css("botón de búsqueda", "[type='submit']", "Botón 'Buscar' del formulario");

    /// <inheritdoc/>
    public override IEnumerable<ITarget> Targets => [Root, QueryField, SubmitButton];
}