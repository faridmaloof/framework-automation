using Framework.Core.ScreenPlay.Interfaces;
using Framework.Core.ScreenPlay;
using Framework.Core.Screenplay;

namespace Scaffolding.Tests.ScreenPlay.Web.Questions;

/// <summary>
/// Gets the current page title.
/// This is a ScreenPlay Question that queries system state.
/// </summary>
public class PageTitle : Question<string>
{
    /// <inheritdoc/>
    public override string Description => "page title";

    /// <summary>
    /// Creates a new PageTitle question.
    /// </summary>
    /// <returns>A new PageTitle question.</returns>
    public static PageTitle Value() => new();

    /// <inheritdoc/>
    public override async System.Threading.Tasks.Task<string> AnsweredByAsync(IActor actor)
    {
        var ability = actor.AbilityTo<BrowseTheWeb>();
        return await ability.Page.TitleAsync();
    }
}

/// <summary>
/// Gets the count of elements matching a selector.
/// This is a ScreenPlay Question that queries system state.
/// </summary>
public class ElementCount : Question<int>
{
    private readonly string _selector;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElementCount"/> class.
    /// </summary>
    /// <param name="selector">The element selector.</param>
    private ElementCount(string selector) => _selector = selector;

    /// <inheritdoc/>
    public override string Description => $"count of '{_selector}'";

    /// <summary>
    /// Creates a new ElementCount question for the specified selector.
    /// </summary>
    /// <param name="selector">The element selector.</param>
    /// <returns>A new ElementCount question.</returns>
    public static ElementCount Of(string selector) => new(selector);

    /// <inheritdoc/>
    public override async System.Threading.Tasks.Task<int> AnsweredByAsync(IActor actor)
    {
        var ability = actor.AbilityTo<BrowseTheWeb>();
        return await ability.Page.Locator(_selector).CountAsync();
    }
}

/// <summary>
/// Gets the text content of an element.
/// This is a ScreenPlay Question that queries system state.
/// </summary>
public class ElementText : Question<string>
{
    private readonly string _selector;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElementText"/> class.
    /// </summary>
    /// <param name="selector">The element selector.</param>
    private ElementText(string selector) => _selector = selector;

    /// <inheritdoc/>
    public override string Description => $"text of '{_selector}'";

    /// <summary>
    /// Creates a new ElementText question for the specified selector.
    /// </summary>
    /// <param name="selector">The element selector.</param>
    /// <returns>A new ElementText question.</returns>
    public static ElementText Of(string selector) => new(selector);

    /// <inheritdoc/>
    public override async System.Threading.Tasks.Task<string> AnsweredByAsync(IActor actor)
    {
        var ability = actor.AbilityTo<BrowseTheWeb>();
        return await ability.Page.Locator(_selector).TextContentAsync() ?? string.Empty;
    }
}
