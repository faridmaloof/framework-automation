using Framework.Core.ScreenPlay;

namespace Framework.Core.Testing;

/// <summary>
/// Convenient base class for non-BDD / classical / TDD API tests (xUnit, NUnit, MSTest).
/// Manages an <see cref="ApiTestSession"/> with an Actor endowed with <see cref="Framework.Core.ScreenPlay.Api.Abilities.CallAnApi"/>.
/// </summary>
public abstract class ApiTestBase : IDisposable {
    private ApiTestSession? _session;

    /// <summary>Gets the current ApiTestSession.</summary>
    protected ApiTestSession Session => _session ?? throw new InvalidOperationException("Session not initialized. Call Initialize() first.");

    /// <summary>Gets the primary Screenplay Actor.</summary>
    protected Actor Actor => Session.Actor;

    /// <summary>
    /// Initializes the API test session.
    /// </summary>
    /// <param name="testName">Name of the test.</param>
    /// <param name="suiteName">Name of the test suite (defaults to class name).</param>
    /// <param name="baseUrl">Optional custom base URL.</param>
    protected virtual void Initialize(string? testName = null, string? suiteName = null, string? baseUrl = null) {
        suiteName ??= GetType().Name;
        _session = ApiTestSession.Start(testName, suiteName, baseUrl);
    }

    /// <summary>
    /// Completes the session, recording the result in Allure.
    /// </summary>
    protected void Complete(bool passed, string? errorMessage = null, string? errorTrace = null, string framework = "classical")
        => Session.Complete(passed, errorMessage, errorTrace, framework);

    /// <inheritdoc/>
    public virtual void Dispose() {
        if (_session is not null) {
            _session.Dispose();
            _session = null;
        }
        GC.SuppressFinalize(this);
    }
}
