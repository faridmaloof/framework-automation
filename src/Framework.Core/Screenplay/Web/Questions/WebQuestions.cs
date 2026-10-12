using Framework.Core.ScreenPlay.Interfaces;
using Framework.Core.ScreenPlay.Locators;

namespace Framework.Core.ScreenPlay.Web.Questions;

// =====================================================================
// ScreenPlay Web questions.
// ---------------------------------------------------------------------
// Questions are PURE: they read state and return data. They MUST NOT
// perform assertions. Use Ensure* tasks (see Ensure.cs) to compose a
// question + assertion in a single reusable ScreenPlay task.
// =====================================================================

/// <summary>Gets the current browser URL.</summary>
public sealed class CurrentUrl : ScreenQuestion<string> {
    /// <inheritdoc/>
    public override string Description => "current URL";

    /// <summary>Static factory.</summary>
    public static CurrentUrl Value() => new();

    /// <inheritdoc/>
    public override Task<string> AnsweredByAsync(IActor actor)
        => Task.FromResult(actor.AbilityTo<BrowseTheWeb>().Page.Url);
}

/// <summary>Gets the current page title.</summary>
public sealed class CurrentTitle : ScreenQuestion<string> {
    /// <inheritdoc/>
    public override string Description => "current title";

    /// <summary>Static factory.</summary>
    public static CurrentTitle Value() => new();

    /// <inheritdoc/>
    public override Task<string> AnsweredByAsync(IActor actor)
        => actor.AbilityTo<BrowseTheWeb>().Page.TitleAsync();
}

/// <summary>
/// Reads the visibility of an element (true when visible / present).
/// </summary>
public sealed class IsVisible : ScreenQuestion<bool> {
    private readonly ITarget _target;

    /// <summary>
    /// Initializes a new instance of the <see cref="IsVisible"/> class.
    /// </summary>
    /// <param name="target">The target element to check for visibility.</param>
    private IsVisible(ITarget target) => _target = target;

    /// <inheritdoc/>
    public override string Description => $"visibility of '{_target}'";

    /// <summary>Build against a typed target.</summary>
    public static IsVisible Of(ITarget target) => new(target);

    /// <inheritdoc/>
    public override Task<bool> AnsweredByAsync(IActor actor)
        => TargetGuard.AnswerAsync(actor, _target, "check the visibility of",
            locator => locator.IsVisibleAsync());
}

/// <summary>Counts elements that match the target.</summary>
public sealed class Count : ScreenQuestion<int> {
    private readonly ITarget _target;

    /// <summary>
    /// Initializes a new instance of the <see cref="Count"/> class.
    /// </summary>
    /// <param name="target">The target element to count.</param>
    private Count(ITarget target) => _target = target;

    /// <inheritdoc/>
    public override string Description => $"count of '{_target}'";

    /// <summary>Build against a typed target.</summary>
    public static Count Of(ITarget target) => new(target);

    /// <inheritdoc/>
    public override Task<int> AnsweredByAsync(IActor actor)
        => TargetGuard.AnswerAsync(actor, _target, "count elements matching",
            locator => locator.CountAsync());
}

/// <summary>Reads the inner text (trimmed) of an element.</summary>
public sealed class TextOf : ScreenQuestion<string> {
    private readonly ITarget _target;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextOf"/> class.
    /// </summary>
    /// <param name="target">The target element to read the text of.</param>
    private TextOf(ITarget target) => _target = target;

    /// <inheritdoc/>
    public override string Description => $"text of '{_target}'";

    /// <summary>Build against a typed target.</summary>
    public static TextOf On(ITarget target) => new(target);

    /// <inheritdoc/>
    public override async Task<string> AnsweredByAsync(IActor actor) {
        var text = await TargetGuard.AnswerAsync(actor, _target, "read the text from",
            locator => locator.InnerTextAsync());
        return text.Trim();
    }
}

/// <summary>Predicate-style question: URL contains the expected substring.</summary>
public sealed class UrlContains : ScreenQuestion<bool> {
    private readonly string _expected;

    /// <summary>
    /// Initializes a new instance of the <see cref="UrlContains"/> class.
    /// </summary>
    /// <param name="expected">The expected substring.</param>
    private UrlContains(string expected) => _expected = expected;

    /// <inheritdoc/>
    public override string Description => $"URL contains '{_expected}'";

    /// <summary>Static factory.</summary>
    public static UrlContains Text(string expected) => new(expected);

    /// <inheritdoc/>
    public override async Task<bool> AnsweredByAsync(IActor actor)
        => (await actor.AsksForAsync(CurrentUrl.Value())).Contains(_expected, StringComparison.OrdinalIgnoreCase);
}

/// <summary>Predicate-style question: URL exactly matches the expected value.</summary>
public sealed class UrlEquals : ScreenQuestion<bool> {
    private readonly string _expected;

    /// <summary>
    /// Initializes a new instance of the <see cref="UrlEquals"/> class.
    /// </summary>
    /// <param name="expected">The expected URL.</param>
    private UrlEquals(string expected) => _expected = expected;

    /// <inheritdoc/>
    public override string Description => $"URL equals '{_expected}'";

    /// <summary>Static factory.</summary>
    public static UrlEquals Value(string expected) => new(expected);

    /// <inheritdoc/>
    public override async Task<bool> AnsweredByAsync(IActor actor)
        => string.Equals(await actor.AsksForAsync(CurrentUrl.Value()), _expected, StringComparison.OrdinalIgnoreCase);
}
