using Framework.Core.ScreenPlay;
using Microsoft.Playwright;

namespace Framework.Core.Testing;

/// <summary>
/// Convenient base class for non-BDD / classical / TDD web tests (xUnit, NUnit, MSTest).
/// Manages a <see cref="WebTestSession"/> with automatic Actor and Page provisioning.
/// Subclasses can call <see cref="InitializeAsync(string?, string?)"/> and <see cref="DisposeAsync()"/>.
/// </summary>
public abstract class WebTestBase : IAsyncDisposable {
    private WebTestSession? _session;

    /// <summary>Gets the current WebTestSession.</summary>
    protected WebTestSession Session => _session ?? throw new InvalidOperationException("Session not initialized. Call InitializeAsync() first.");

    /// <summary>Gets the primary Screenplay Actor.</summary>
    protected Actor Actor => Session.Actor;

    /// <summary>Gets the Playwright IPage.</summary>
    protected IPage Page => Session.Page;

    /// <summary>
    /// Initializes the web test session.
    /// </summary>
    /// <param name="testName">Name of the test.</param>
    /// <param name="suiteName">Name of the test suite (defaults to class name).</param>
    /// <returns>A task representing the initialization.</returns>
    protected virtual async Task InitializeAsync(string? testName = null, string? suiteName = null) {
        suiteName ??= GetType().Name;
        _session = await WebTestSession.StartAsync(testName, suiteName);
    }

    /// <summary>
    /// Creates a secondary persona with its own isolated browser session.
    /// </summary>
    protected Task<Actor> CreatePersonaAsync(string name) => Session.CreatePersonaAsync(name);

    /// <summary>
    /// Completes the session, recording the result and generating evidence.
    /// </summary>
    protected Task CompleteAsync(bool passed, string? errorMessage = null, string? errorTrace = null, string framework = "classical")
        => Session.CompleteAsync(passed, errorMessage, errorTrace, framework);

    /// <inheritdoc/>
    public virtual async ValueTask DisposeAsync() {
        if (_session is not null) {
            await _session.DisposeAsync();
            _session = null;
        }
        GC.SuppressFinalize(this);
    }
}
