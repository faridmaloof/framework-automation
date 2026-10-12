using Microsoft.Playwright;

namespace Framework.Core.ScreenPlay.Locators;

/// <summary>
/// <see cref="ITarget"/> extension methods that depend on a concrete
/// automation engine (Playwright). The <see cref="ITarget"/> interface
/// itself stays engine-agnostic so consumers can plug in other engines.
/// </summary>
public static class TargetExtensions {
    /// <summary>
    /// Resolves the target against the given Playwright page using the
    /// target's strategy. When the target declares a <see cref="ITarget.Parent"/>
    /// (component scope), the whole ancestor chain is resolved first — from the
    /// outermost component down to the target itself — so every selector is
    /// interpreted within its parent element.
    /// Falls back to <c>page.Locator(selector)</c> (CSS) for unknown strategies.
    /// </summary>
    /// <param name="target">The target to resolve.</param>
    /// <param name="page">The Playwright page.</param>
    /// <returns>A Playwright <see cref="ILocator"/> for the target.</returns>
    public static ILocator Resolve(this ITarget target, IPage page) {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(page);

        // Build the ancestor chain from the outermost scope down to the target.
        var chain = new List<ITarget>();
        for (var current = target; current is not null; current = current.Parent)
            chain.Insert(0, current);

        // First element resolves against the page; the rest resolve inside their parent.
        var resolved = Apply(page, chain[0]);
        for (var i = 1; i < chain.Count; i++)
            resolved = Apply(resolved, chain[i]);
        return resolved;
    }

    // ---- Strategy application (page scope and parent scope share the mapping) ----

    private static ILocator Apply(IPage scope, ITarget target)
        => ApplyStrategy(
            target,
            text => scope.GetByText(text),
            testId => scope.GetByTestId(testId),
            label => scope.GetByLabel(label),
            placeholder => scope.GetByPlaceholder(placeholder),
            raw => scope.Locator(raw));

    private static ILocator Apply(ILocator scope, ITarget target)
        => ApplyStrategy(
            target,
            text => scope.GetByText(text),
            testId => scope.GetByTestId(testId),
            label => scope.GetByLabel(label),
            placeholder => scope.GetByPlaceholder(placeholder),
            raw => scope.Locator(raw));

    private static ILocator ApplyStrategy(ITarget target,
                                          Func<string, ILocator> byText,
                                          Func<string, ILocator> byTestId,
                                          Func<string, ILocator> byLabel,
                                          Func<string, ILocator> byPlaceholder,
                                          Func<string, ILocator> byRaw) {
        var strategy = (target.Strategy ?? "css").Trim().ToLowerInvariant();
        return strategy switch {
            "text"        => byText(target.Selector),
            "test-id"     => byTestId(target.Selector),
            "label"       => byLabel(target.Selector),
            "placeholder" => byPlaceholder(target.Selector),
            "xpath"       => byRaw($"xpath={target.Selector}"),
            _             => byRaw(target.Selector),
        };
    }
}
