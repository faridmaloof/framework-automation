namespace Framework.Core.Lifecycle;

/// <summary>
/// Identifies the current test-run execution. A single, process-wide id is
/// generated lazily on first access and shared by all hooks, so run history,
/// Allure results and reports can be attributed to one execution — and, when
/// configured, isolated into their own folder/file instead of overwriting
/// previous runs (spec §10.2).
/// </summary>
public static class RunContext {
    /// <summary>
    /// Gets the unique id of the current execution, lazily generated on first access.
    /// Format: yyyyMMdd-HHmmss-XXXXXXXX (UTC timestamp + 8-char GUID suffix).
    /// </summary>
    private static readonly Lazy<string> RunIdLazy =
        new(() => $"{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString("N")[..8]}",
            LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>Gets the unique id of the current execution.</summary>
    public static string RunId => RunIdLazy.Value;

    /// <summary>Gets the UTC timestamp when this run started.</summary>
    public static DateTimeOffset StartedAtUtc { get; } = DateTimeOffset.UtcNow;
}
