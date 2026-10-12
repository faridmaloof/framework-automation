using System.Text.Json;

namespace Framework.Core.Tools;

/// <summary>
/// One recorded scenario execution. Appended as a JSON line so runs can be
/// tracked over time (pass rate, duration trends, flaky detection).
/// </summary>
/// <param name="TimestampUtc">UTC timestamp of the execution.</param>
/// <param name="Feature">Feature/scenario title.</param>
/// <param name="Scenario">Scenario title.</param>
/// <param name="Status">PASSED / FAILED / SKIPPED.</param>
/// <param name="DurationSeconds">Scenario duration in seconds.</param>
/// <param name="Tags">Scenario tags (@web, @smoke...).</param>
/// <param name="Error">Failure summary when the scenario failed.</param>
public sealed record ScenarioRunEntry(
    string TimestampUtc,
    string Feature,
    string Scenario,
    string Status,
    double DurationSeconds,
    string[] Tags,
    string? Error);

/// <summary>
/// Append-only run history (JSON lines). Zero-dependency, fast, and easy to
/// feed into dashboards or simple scripts to see how the suite behaves over time.
/// </summary>
public static class RunHistory
{
    /// <summary>Default history file name (used by <see cref="Configuration.HistoryMode.Shared"/>).</summary>
    public const string DefaultFileName = "runs.jsonl";

    /// <summary>
    /// JSON serializer options for the run-history entries. Uses the Web defaults
    /// (camelCase, ISO 8601 timestamps, no comments, no trailing commas).
    /// </summary>
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    // Serializes concurrent appends (parallel scenarios) so JSON lines are
    // never interleaved/corrupted on disk. Cheap: only guards the short write.
    private static readonly object AppendLock = new();

    /// <summary>
    /// Appends one scenario entry to <c>{directory}/{fileName}</c>.
    /// Thread-safe: concurrent scenarios are serialized so each entry is a
    /// single, well-formed line.
    /// </summary>
    /// <param name="directory">History directory (created if missing).</param>
    /// <param name="entry">The scenario entry to record.</param>
    /// <param name="fileName">History file name (defaults to <c>runs.jsonl</c>).</param>
    public static void Append(string directory, ScenarioRunEntry entry, string fileName = DefaultFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        ArgumentNullException.ThrowIfNull(entry);

        Directory.CreateDirectory(directory);
        var file = Path.Combine(directory, fileName);
        var line = JsonSerializer.Serialize(entry, JsonOptions) + Environment.NewLine;

        lock (AppendLock)
        {
            File.AppendAllText(file, line);
        }
    }

    /// <summary>
    /// Reads the most recent entries (last <paramref name="limit"/> lines).
    /// </summary>
    /// <param name="directory">History directory.</param>
    /// <param name="limit">Maximum entries to return.</param>
    /// <param name="fileName">History file name (defaults to <c>runs.jsonl</c>).</param>
    /// <returns>Entries from oldest to newest.</returns>
    public static IReadOnlyList<ScenarioRunEntry> ReadRecent(string directory, int limit = 50, string fileName = DefaultFileName)
    {
        var file = Path.Combine(directory, fileName);
        if (!File.Exists(file)) return [];

        var lines = File.ReadLines(file);
        var entries = new List<ScenarioRunEntry>();
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            if (JsonSerializer.Deserialize<ScenarioRunEntry>(line, JsonOptions) is { } entry)
                entries.Add(entry);
        }
        return entries.Count <= limit ? entries : [.. entries.Skip(entries.Count - limit)];
    }
}