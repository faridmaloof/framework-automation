using Framework.Core.ScreenPlay.Locators;
using Framework.Core.ScreenPlay.Pages;

namespace Scaffolding.Tests.Pages.Google;

/// <summary>
/// Locator catalogue for the Google home page — pure Screenplay page object:
/// page identity + targets only, <b>no behaviour</b>. Behaviour lives in the
/// page's Tasks (<see cref="SearchFor"/>), which consume these targets.
/// </summary>
public sealed class GoogleHomePage : PageObject {
    /// <summary>Stateless page object, shared by tasks, steps and questions.</summary>
    public static readonly GoogleHomePage Instance = new();

    /// <summary>
    /// Private constructor to enforce singleton pattern. Use <see cref="Instance"/> instead.
    /// </summary>
    private GoogleHomePage() { }

    /// <inheritdoc/>
    public override string Name => "Página principal de Google";

    /// <inheritdoc/>
    public override string Url => "https://www.google.com";

    /// <summary>Campo de consulta del SearchBox, con el contexto de esta página.</summary>
    public Target SearchQueryField => SearchBox.Instance.QueryField with { Page = Name };

    /// <summary>Botón de envío del SearchBox, con el contexto de esta página.</summary>
    public Target SearchSubmitButton => SearchBox.Instance.SubmitButton with { Page = Name };

    /// <inheritdoc/>
    public override IEnumerable<ITarget> Targets => [SearchBox.Instance.Root, SearchQueryField, SearchSubmitButton];
}