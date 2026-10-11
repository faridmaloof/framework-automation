using FluentAssertions;
using Framework.Core.ScreenPlay.Interfaces;
using Framework.Core.ScreenPlay.Locators;
using Framework.Core.ScreenPlay.Web.Questions;

namespace Framework.Core.ScreenPlay.Web.Tasks;

// =====================================================================
// Ensure* tasks — ScreenPlay-friendly assertion builders.
// ---------------------------------------------------------------------
// Why not IQuestion<Task>?
//   * A Question must answer with data; it must NOT throw assertions.
//   * Putting assertions inside a Question breaks composability and
//     hides the failure under an "AnsweredBy" call.
// Ensure* tasks are regular ITasks that:
//   1) delegate to a pure Question,
//   2) apply FluentAssertions on the result,
//   3) throw a descriptive exception on failure.
//
// They are reusable across pages, can be combined inside larger Tasks,
// and surface their intent in reports and logs through Description.
// =====================================================================

/// <summary>
/// Ensures the URL of the page contains the expected substring.
/// Throws on failure with a domain-readable message.
/// </summary>
public sealed class EnsureUrlContains : ScreenTask
{
    private readonly string _expected;
    private readonly string _because;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnsureUrlContains"/> class.
    /// </summary>
    /// <param name="expected">The expected substring in the URL. </param>
    /// <param name="because">The reason for the assertion. </param>
    private EnsureUrlContains(string expected, string because)
    {
        _expected = expected;
        _because = because;
    }

    /// <inheritdoc/>
    public override string TaskName => $"Ensure URL contains '{_expected}'";

    /// <inheritdoc/>
    public override string Description =>
        $"{TaskName}{(string.IsNullOrEmpty(_because) ? string.Empty : $": {_because}")}";

    /// <summary>Static factory.</summary>
    public static EnsureUrlContains Text(string expected, string because = "")
        => new(expected, because);

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        var ok = await actor.AsksForAsync(UrlContains.Text(_expected));
        ok.Should().BeTrue(_because);
    }
}

/// <summary>
/// Ensures the element (resolved by target) is visible.
/// </summary>
public sealed class EnsureVisible : ScreenTask
{
    private readonly ITarget _target;
    private readonly string _because;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnsureVisible"/> class.
    /// </summary>
    /// <param name="target">The target element to check for visibility.</param>
    /// <param name="because">The reason for the assertion.</param>
    private EnsureVisible(ITarget target, string because)
    {
        _target = target;
        _because = because;
    }

    /// <inheritdoc/>
    public override string TaskName => $"Ensure '{_target}' is visible";

    /// <inheritdoc/>
    public override string Description =>
        $"{TaskName}{(string.IsNullOrEmpty(_because) ? string.Empty : $": {_because}")}";

    /// <summary>Static factory.</summary>
    public static EnsureVisible Of(ITarget target, string because = "")
        => new(target, because);

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        var ok = await actor.AsksForAsync(IsVisible.Of(_target));
        ok.Should().BeTrue(string.IsNullOrEmpty(_because)
            ? $"'{_target}' should be visible"
            : _because);
    }
}

/// <summary>
/// Ensures the element (resolved by target) is NOT visible (best-effort;
/// it considers DOM presence + display + opacity per Playwright).
/// </summary>
public sealed class EnsureHidden : ScreenTask
{
    private readonly ITarget _target;
    private readonly string _because;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnsureHidden"/> class.
    /// </summary>
    /// <param name="target">The target element to check for hiddenness.</param>
    /// <param name="because">The reason for the assertion.</param>
    private EnsureHidden(ITarget target, string because)
    {
        _target = target;
        _because = because;
    }

    /// <inheritdoc/>
    public override string TaskName => $"Ensure '{_target}' is hidden";

    /// <inheritdoc/>
    public override string Description =>
        $"{TaskName}{(string.IsNullOrEmpty(_because) ? string.Empty : $": {_because}")}";

    /// <summary>Static factory.</summary>
    public static EnsureHidden Of(ITarget target, string because = "")
        => new(target, because);

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        var visible = await actor.AsksForAsync(IsVisible.Of(_target));
        visible.Should().BeFalse(string.IsNullOrEmpty(_because)
            ? $"'{_target}' should be hidden"
            : _because);
    }
}

/// <summary>
/// Ensures the count of elements matching the target is greater than the
/// supplied minimum.
/// </summary>
public sealed class EnsureCountGreaterThan : ScreenTask
{
    private readonly ITarget _target;
    private readonly int _minimum;
    private readonly string _because;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnsureCountGreaterThan"/> class.
    /// </summary>
    /// <param name="target">The target element to check the count of.</param>
    /// <param name="minimum">The minimum count required.</param>
    /// <param name="because">The reason for the assertion.</param>
    private EnsureCountGreaterThan(ITarget target, int minimum, string because)
    {
        _target = target;
        _minimum = minimum;
        _because = because;
    }

    /// <inheritdoc/>
    public override string TaskName => $"Ensure count of '{_target}' > {_minimum}";

    /// <inheritdoc/>
    public override string Description =>
        $"{TaskName}{(string.IsNullOrEmpty(_because) ? string.Empty : $": {_because}")}";

    /// <summary>Static factory.</summary>
    public static EnsureCountGreaterThan Of(ITarget target, int minimum, string because = "")
        => new(target, minimum, because);

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        var count = await actor.AsksForAsync(Count.Of(_target));
        count.Should().BeGreaterThan(_minimum, _because);
    }
}

/// <summary>
/// Ensures the inner text of an element matches the expected value (or contains it).
/// </summary>
public sealed class EnsureTextOf : ScreenTask
{
    private readonly ITarget _target;
    private readonly string _expected;
    private readonly TextMatchMode _mode;
    private readonly string _because;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnsureTextOf"/> class.
    /// </summary>
    /// <param name="target">The target element to check the text of.</param>
    /// <param name="expected">The expected text.</param>
    /// <param name="mode">The text match mode.</param>
    /// <param name="because">The reason for the assertion.</param>
    private EnsureTextOf(ITarget target, string expected, TextMatchMode mode, string because)
    {
        _target = target;
        _expected = expected;
        _mode = mode;
        _because = because;
    }

    /// <inheritdoc/>
    public override string TaskName => $"Ensure text of '{_target}' {_mode} '{_expected}'";

    /// <inheritdoc/>
    public override string Description =>
        $"{TaskName}{(string.IsNullOrEmpty(_because) ? string.Empty : $": {_because}")}";

    /// <summary>Strict equality match.</summary>
    public static EnsureTextOf Equals(ITarget target, string expected, string because = "")
        => new(target, expected, TextMatchMode.Equals, because);

    /// <summary>Case-insensitive "contains" match.</summary>
    public static EnsureTextOf Contains(ITarget target, string expected, string because = "")
        => new(target, expected, TextMatchMode.Contains, because);

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        var actual = await actor.AsksForAsync(TextOf.On(_target));
        switch (_mode)
        {
            case TextMatchMode.Equals:
                actual.Should().Be(_expected, _because);
                break;
            case TextMatchMode.Contains:
                actual.Should().Contain(_expected, _because);
                break;
        }
    }

    /// <summary>
    /// Defines the text match mode for <see cref="EnsureTextOf"/>.
    /// </summary>
    private enum TextMatchMode { Equals, Contains }
}
