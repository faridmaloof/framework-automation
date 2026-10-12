using System.Text.Json;
using FluentAssertions;
using Framework.Core.Tools;
using Xunit;

namespace Framework.Core.Tests;

/// <summary>
/// Unit tests for <see cref="RunHistory"/> — the append-only JSON-lines store.
/// Focus on the concurrency guarantee (§12): parallel appends must never
/// interleave/corrupt lines, so every persisted line deserializes cleanly.
/// </summary>
public class RunHistoryTests : IDisposable {
    private readonly string _dir;

    public RunHistoryTests()
        => _dir = Path.Combine(Path.GetTempPath(), "runhistory-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public void Append_ThenReadRecent_RoundTripsEntries()
    {
        RunHistory.Append(_dir, Entry("F", "S1", "PASSED"));
        RunHistory.Append(_dir, Entry("F", "S2", "FAILED", "boom"));

        var recent = RunHistory.ReadRecent(_dir, 10);

        recent.Should().HaveCount(2);
        recent[0].Scenario.Should().Be("S1");
        recent[1].Status.Should().Be("FAILED");
        recent[1].Error.Should().Be("boom");
    }

    [Fact]
    public void Append_ConcurrentWrites_ProduceWellFormedLines()
    {
        const int count = 200;
        Parallel.For(0, count, i => RunHistory.Append(_dir, Entry("F", $"S{i}", "PASSED")));

        var file = Path.Combine(_dir, "runs.jsonl");
        var lines = File.ReadAllLines(file).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();

        // No line lost and none corrupted by interleaving.
        lines.Should().HaveCount(count);
        foreach (var line in lines)
        {
            var act = () => JsonSerializer.Deserialize<ScenarioRunEntry>(line,
                new JsonSerializerOptions(JsonSerializerDefaults.Web));
            act.Should().NotThrow($"every line must be valid JSON, got: {line}");
        }
    }

    [Fact]
    public void ReadRecent_RespectsLimitAndKeepsNewest()
    {
        for (var i = 0; i < 10; i++) RunHistory.Append(_dir, Entry("F", $"S{i}", "PASSED"));

        var recent = RunHistory.ReadRecent(_dir, 3);

        recent.Should().HaveCount(3);
        recent[^1].Scenario.Should().Be("S9");
    }

    [Fact]
    public void ReadRecent_MissingFile_ReturnsEmpty()
        => RunHistory.ReadRecent(_dir, 10).Should().BeEmpty();

    private static ScenarioRunEntry Entry(string feature, string scenario, string status, string? error = null)
        => new(DateTime.UtcNow.ToString("o"), feature, scenario, status, 0.1, ["unit"], error);

    public void Dispose()
    {
        try { if (Directory.Exists(_dir)) Directory.Delete(_dir, recursive: true); }
        catch { /* best-effort cleanup */ }
    }
}
