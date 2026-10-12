using Framework.Core.ScreenPlay.Interfaces;
using Framework.Core.ScreenPlay.Locators;
using Microsoft.Playwright;

namespace Framework.Core.ScreenPlay.Web.Interactions;

// =====================================================================
// ScreenPlay atomic Web interactions.
// ---------------------------------------------------------------------
// Every interaction accepts either an <see cref="ITarget"/> (preferred)
// or a raw selector string (kept for backwards-compatibility and quick
// throw-away scripts). The canonical API is the fluent factory methods,
// e.g. <c>Click.On(target)</c> / <c>Enter.TheText("foo").Into(target)</c>.
// =====================================================================

/// <summary>
/// Clicks the resolved element. Waits for actionability per Playwright defaults.
/// </summary>
/// <remarks>
/// Use the fluent factory <see cref="On(ITarget)"/> or <see cref="On(string)"/>.
/// </remarks>
public sealed class Click : ScreenInteraction {
    private readonly ITarget _target;
    private readonly string? _fallbackSelector;
    private readonly string? _fallbackStrategy;

    /// <summary>
    /// Initializes a new instance of the <see cref="Click"/> class.
    /// </summary>
    /// <param name="target">The target element to click.</param>
    /// <param name="fallbackSelector">The fallback selector to use if the target is not found.</param>
    /// <param name="fallbackStrategy">The fallback strategy to use if the target is not found.</param>
    private Click(ITarget target, string? fallbackSelector = null, string? fallbackStrategy = null) {
        _target = target;
        _fallbackSelector = fallbackSelector;
        _fallbackStrategy = fallbackStrategy;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Click"/> class with a raw selector.
    /// </summary>
    /// <param name="selector">The selector to use.</param>
    /// <param name="strategy">The strategy to use.</param>
    private Click(string selector, string? strategy = null) {
        // Ad-hoc target: the selector IS the only available name, so it is used
        // as the human reference too. Prefer On(ITarget) with targets from a PageObject.
        _target = Target.Css(selector, selector, description: "");
        _fallbackSelector = selector;
        _fallbackStrategy = strategy;
    }

    /// <inheritdoc/>
    public override string Description => $"Click '{_target}'";

    /// <summary>Click on the given typed target.</summary>
    public static Click On(ITarget target) => new(target);

    /// <summary>
    /// Click on a raw selector (ad-hoc use only — prefer <see cref="On(ITarget)"/>).
    /// </summary>
    public static Click On(string selector) => new(selector);

    /// <inheritdoc/>
    public override Task PerformAsAsync(IActor actor)
        => TargetGuard.PerformAsync(actor, _target, "click", locator => locator.ClickAsync());
}

/// <summary>
/// Clears an input element before typing (Playwright's <c>fill</c> semantics).
/// </summary>
public sealed class Clear : ScreenInteraction {
    private readonly ITarget _target;

    private Clear(ITarget target) => _target = target;

    /// <inheritdoc/>
    public override string Description => $"Clear '{_target}'";

    /// <summary>Clears the given typed target input.</summary>
    public static Clear Field(ITarget target) => new(target);

    /// <inheritdoc/>
    public override Task PerformAsAsync(IActor actor)
        => TargetGuard.PerformAsync(actor, _target, "clear", locator => locator.ClearAsync());
}

/// <summary>
/// Fluent builder returned by <see cref="Enter.TheText(string)"/>.
/// Configure the destination target with <see cref="Into(ITarget)"/>
/// and finally execute with <see cref="Now()"/> or by handing the
/// interaction to the actor via <c>AttemptsToAsync</c>.
/// </summary>
public sealed class Enter : ScreenInteraction {
    private readonly string _text;
    private ITarget? _target;

    /// <summary>
    /// Initializes a new instance of the <see cref="Enter"/> class.
    /// </summary>
    /// <param name="text">The text to enter.</param>
    private Enter(string text) => _text = text;

    /// <inheritdoc/>
    public override string Description =>
        _target is null
            ? $"Enter '{_text}' (no target set)"
            : $"Enter '{_text}' into '{_target}'";

    /// <summary>Start building an Enter interaction with the given text.</summary>
    public static Enter TheText(string text) => new(text);

    /// <summary>Bind the typed destination target.</summary>
    public Enter Into(ITarget target) {
        _target = target;
        return this;
    }

    /// <summary>
    /// Terminal operator — yields the configured interaction so it can be
    /// passed to <c>Actor.AttemptsToAsync(...)</c>.
    /// </summary>
    public Enter Now() {
        if (_target is null)
            throw new InvalidOperationException(
                "Enter requires a target — call Into(target) before Now().");
        return this;
    }

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor) {
        if (_target is null)
            throw new InvalidOperationException(
                "Enter requires a target — call Into(target) before performing.");
        await TargetGuard.PerformAsync(actor, _target, "fill in", locator => locator.FillAsync(_text));
    }
}

/// <summary>
/// Presses a key on the resolved element (defaults to <c>Enter</c>).
/// </summary>
public sealed class PressKey : ScreenInteraction {
    private readonly ITarget _target;
    private readonly string _key;

    /// <summary>
    /// Initializes a new instance of the <see cref="PressKey"/> class.
    /// </summary>
    /// <param name="target">The target element to press the key on.</param>
    /// <param name="key">The key to press.</param>
    private PressKey(ITarget target, string key) {
        _target = target;
        _key = key;
    }

    /// <inheritdoc/>
    public override string Description => $"Press '{_key}' on '{_target}'";

    /// <summary>Press <c>Enter</c> on the target.</summary>
    public static PressKey EnterOn(ITarget target) => new(target, "Enter");

    /// <summary>Press an arbitrary key on the target.</summary>
    public static PressKey Key(ITarget target, string key) => new(target, key);

    /// <inheritdoc/>
    public override Task PerformAsAsync(IActor actor)
        => TargetGuard.PerformAsync(actor, _target, $"press '{_key}' on",
            locator => locator.PressAsync(_key));
}

/// <summary>
/// Hovers over the resolved element.
/// </summary>
public sealed class Hover : ScreenInteraction {
    private readonly ITarget _target;

    /// <summary>
    /// Initializes a new instance of the <see cref="Hover"/> class.
    /// </summary>
    /// <param name="target">The target element to hover over.</param>
    private Hover(ITarget target) => _target = target;

    /// <inheritdoc/>
    public override string Description => $"Hover '{_target}'";

    /// <summary>Hover the given typed target.</summary>
    public static Hover Over(ITarget target) => new(target);

    /// <inheritdoc/>
    public override Task PerformAsAsync(IActor actor)
        => TargetGuard.PerformAsync(actor, _target, "hover over", locator => locator.HoverAsync());
}

/// <summary>
/// Waits for the resolved element to reach the configured Playwright state.
/// </summary>
public sealed class WaitFor : ScreenInteraction {
    private readonly ITarget _target;
    private readonly WaitForSelectorState _state;

    /// <summary>
    /// Initializes a new instance of the <see cref="WaitFor"/> class.
    /// </summary>
    /// <param name="target">The target element to wait for.</param>
    /// <param name="state">The state to wait for.</param>
    private WaitFor(ITarget target, WaitForSelectorState state) {
        _target = target;
        _state = state;
    }

    /// <inheritdoc/>
    public override string Description => $"Wait for '{_target}' to be {_state}";

    /// <summary>Wait for the element to become visible.</summary>
    public static WaitFor Visible(ITarget target) => new(target, WaitForSelectorState.Visible);

    /// <summary>Wait for the element to become hidden.</summary>
    public static WaitFor Hidden(ITarget target) => new(target, WaitForSelectorState.Hidden);

    /// <summary>Wait for the element to be attached to the DOM.</summary>
    public static WaitFor Attached(ITarget target) => new(target, WaitForSelectorState.Attached);

    /// <inheritdoc/>
    public override Task PerformAsAsync(IActor actor)
        => TargetGuard.PerformAsync(actor, _target, "wait for",
            locator => locator.WaitForAsync(new LocatorWaitForOptions { State = _state }));
}
