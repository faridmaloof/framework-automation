using System.Text.Json;
using FluentAssertions;
using Framework.Core.Lifecycle;
using Framework.Core.Tools;
using Xunit;

namespace Framework.Core.Tests;

/// <summary>
/// Unit tests for the native Allure writer + path resolution: results must be
/// valid Allure-2 JSON, environment.properties must exist, and relative paths
/// must resolve from the repo root (never bin/Debug).
/// </summary>
public class AllureReportTests : IDisposable
{
    private readonly string _dir;

    public AllureReportTests()
        => _dir = Path.Combine(Path.GetTempPath(), "allure-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public void WriteResult_ProducesValidAllureJson()
    {
        var path = AllureReportHelper.WriteResult(
            _dir, "Feature X", "Scenario Y", passed: true, durationMs: 120,
            tags: ["smoke", "@web"]);

        File.Exists(path).Should().BeTrue();
        var json = JsonDocument.Parse(File.ReadAllText(path)).RootElement;
        json.GetProperty("name").GetString().Should().Be("Scenario Y");
        json.GetProperty("status").GetString().Should().Be("passed");
        json.GetProperty("stage").GetString().Should().Be("finished");
    }

    [Fact]
    public void WriteResult_Failed_IncludesErrorDetails()
    {
        var path = AllureReportHelper.WriteResult(
            _dir, "Feature X", "Scenario Z", passed: false, durationMs: 50,
            errorMessage: "boom", errorTrace: "boom\n at X()");

        var json = JsonDocument.Parse(File.ReadAllText(path)).RootElement;
        json.GetProperty("status").GetString().Should().Be("failed");
        json.GetProperty("statusDetails").GetProperty("message").GetString().Should().Be("boom");
    }

    [Fact]
    public void WriteResult_AttachesExistingFiles_AndSkipsMissing()
    {
        var shot = Path.Combine(_dir, "shots", "s.png");
        Directory.CreateDirectory(Path.GetDirectoryName(shot)!);
        File.WriteAllBytes(shot, [1, 2, 3]);

        var path = AllureReportHelper.WriteResult(
            _dir, "F", "S", passed: false, durationMs: 10,
            attachments: [shot, Path.Combine(_dir, "missing.png")]);

        var json = JsonDocument.Parse(File.ReadAllText(path)).RootElement;
        var attachments = json.GetProperty("attachments").EnumerateArray().ToArray();
        attachments.Should().HaveCount(1);
        attachments[0].GetProperty("type").GetString().Should().Be("image/png");
    }

    [Fact]
    public void WriteEnvironment_ProducesPropertiesFile()
    {
        AllureReportHelper.WriteEnvironment(_dir, "Chromium", "Development");
        var props = File.ReadAllText(Path.Combine(_dir, "environment.properties"));
        props.Should().Contain("Browser=Chromium");
        props.Should().Contain("RunId=");
    }

    [Fact]
    public void ResolveRepoPath_Absolute_IsUsedAsIs()
    {
        var abs = Path.GetFullPath(_dir);
        ReportService.ResolveRepoPath(abs).Should().Be(abs);
    }

    [Fact]
    public void WriteResult_SameScenarioTwice_KeepsStableHistoryId()
    {
        // The key to Allure history/retries: repeats of the same test share
        // historyId, so run N+1 appends instead of overwriting run N.
        var p1 = AllureReportHelper.WriteResult(_dir, "F", "S", passed: true, durationMs: 10);
        var p2 = AllureReportHelper.WriteResult(_dir, "F", "S", passed: false, durationMs: 20,
            errorMessage: "flaky boom", errorTrace: "boom");

        var h1 = JsonDocument.Parse(File.ReadAllText(p1)).RootElement.GetProperty("historyId").GetString();
        var h2 = JsonDocument.Parse(File.ReadAllText(p2)).RootElement.GetProperty("historyId").GetString();
        h1.Should().Be(h2);
        p1.Should().NotBe(p2, "each result needs its own uuid file");
    }

    [Fact]
    public void MergeResults_RollsRunIntoCumulative_WithoutDeletingRun()
    {
        var runDir = Path.Combine(_dir, "run-1");
        var cumulative = Path.Combine(_dir, "latest");
        var shot = Path.Combine(runDir, "s.png");
        Directory.CreateDirectory(runDir);
        File.WriteAllBytes(shot, [9]);

        AllureReportHelper.WriteResult(runDir, "F", "S", passed: true, durationMs: 5, attachments: [shot]);
        var merged = AllureReportHelper.MergeResults(runDir, cumulative);

        merged.Should().Be(1);
        Directory.GetFiles(runDir, "*-result.json").Should().HaveCount(1, "per-run folder is immutable");
        Directory.GetFiles(cumulative, "*-result.json").Should().HaveCount(1);
    }

    [Fact]
    public void ListRunDirectories_OldestFirst_ExcludesLatestAndHidden()
    {
        var baseDir = Path.Combine(_dir, "allure");
        var oldest = Directory.CreateDirectory(Path.Combine(baseDir, "run-old"));
        var newest = Directory.CreateDirectory(Path.Combine(baseDir, "run-new"));
        Directory.CreateDirectory(Path.Combine(baseDir, "latest"));
        File.WriteAllText(Path.Combine(baseDir, ".report.lock"), "x");
        // Force distinct timestamps: oldest < newest.
        oldest.LastWriteTimeUtc = DateTime.UtcNow.AddHours(-2);
        newest.LastWriteTimeUtc = DateTime.UtcNow;

        var runs = ReportService.ListRunDirectories(baseDir).Select(d => d.Name).ToArray();
        runs.Should().Equal("run-old", "run-new");
    }

    [Fact]
    public void ResolveBaseResultsDirectory_Absolute_IsUsedAsIs()
    {
        var abs = Path.GetFullPath(_dir);
        var allure = new Framework.Core.Configuration.AllureSettings { ResultsDirectory = abs };
        ReportService.ResolveBaseResultsDirectory(allure).Should().Be(abs);
    }

    [Fact]
    public void UnifiedOutput_Subdirectories_AllLiveInsideBaseResultsDirectory()
    {
        var abs = Path.GetFullPath(_dir);
        var allure = new Framework.Core.Configuration.AllureSettings { ResultsDirectory = abs };
        var fw = new Framework.Core.Configuration.FrameworkSettings();

        var baseDir = ReportService.ResolveBaseResultsDirectory(allure);
        var evidenceDir = ReportService.ResolveEvidenceDirectory(allure, fw.Evidence);
        var historyDir = ReportService.ResolveHistoryDirectory(allure, fw.Reporting);
        var reportDir = ReportService.ResolveReportDirectory(allure);
        var resultsDir = ReportService.ResolveRunResultsDirectory(allure, fw.Reporting);
        var cumulativeDir = ReportService.ResolveCumulativeResultsDirectory(allure);

        baseDir.Should().Be(abs);
        evidenceDir.Should().StartWith(abs);
        historyDir.Should().StartWith(abs);
        reportDir.Should().StartWith(abs);
        resultsDir.Should().StartWith(abs);
        cumulativeDir.Should().StartWith(abs);

        evidenceDir.Should().Be(Path.Combine(abs, "evidence"));
        historyDir.Should().Be(Path.Combine(abs, "history"));
        reportDir.Should().Be(Path.Combine(abs, "report"));
        cumulativeDir.Should().Be(Path.Combine(abs, "results", "latest"));
    }

    public void Dispose()
    {
        try { if (Directory.Exists(_dir)) Directory.Delete(_dir, recursive: true); }
        catch { /* best-effort cleanup */ }
    }
}
