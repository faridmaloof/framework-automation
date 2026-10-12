using Framework.Core.ScreenPlay.Locators;
using Framework.Core.ScreenPlay.Pages;

namespace Scaffolding.Tests.Pages.Google;

/// <summary>
/// Locator catalogue for the Google search results page — pure Screenplay
/// page object: page identity + targets only, no behaviour.
/// </summary>
public sealed class GoogleSearchResultsPage : PageObject {
    /// <summary>Stateless page object, shared by tasks, steps and questions.</summary>
    public static readonly GoogleSearchResultsPage Instance = new();

    /// <summary>
    /// Private constructor to enforce singleton pattern. Use <see cref="Instance"/> instead.
    /// </summary>
    private GoogleSearchResultsPage() { }

    /// <inheritdoc/>
    public override string Name => "Resultados de búsqueda de Google";

    /// <summary>
    /// Results are reached through the search flow (query string), so this
    /// page intentionally has no canonical URL for <c>Open()</c>.
    /// </summary>
    public override string? Url => null;

    /// <summary>Campo de consulta del SearchBox reutilizado en esta página.</summary>
    public Target SearchQueryField => SearchBox.Instance.QueryField with { Page = Name };

    /// <summary>Área donde se listan los resultados de la búsqueda.</summary>
    public Target ResultsContainer => Css("contenedor de resultados", "#search", "Zona de la página con los resultados");

    /// <summary>Títulos de cada resultado listado.</summary>
    public Target ResultTitles => Css("títulos de resultados", "#search h3", "Título (h3) de cada resultado");

    /// <summary>Texto informativo con la cantidad aproximada de resultados.</summary>
    public Target ResultStats => Css("contador de resultados", "#result-stats", "Texto 'Aproximadamente X resultados'");

    /// <summary>Enlace de paginación hacia la siguiente página.</summary>
    public Target NextPageLink => Css("enlace de siguiente página", "#pnnext", "Ir a la siguiente página de resultados");

    /// <inheritdoc/>
    public override IEnumerable<ITarget> Targets =>
        [SearchQueryField, ResultsContainer, ResultTitles, ResultStats, NextPageLink];
}