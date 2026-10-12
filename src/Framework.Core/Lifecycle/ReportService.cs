using Framework.Core.Configuration;
using Framework.Core.Tools;

namespace Framework.Core.Lifecycle;

/// <summary>
/// Report + history + evidence path resolution driven by Core configuration (spec §10).
/// Unifies ALL execution outputs (Allure results, HTML report, evidence, history)
/// under a single base directory configured in <see cref="AllureSettings.ResultsDirectory"/>,
/// so nothing is scattered across the repository root.
/// Supports both relative (e.g. "allure-results") and absolute (e.g. "D:/TestRuns") paths.
/// </summary>
public static class ReportService {
    /// <summary>
    /// Resolves an arbitrary path: absolute paths are used as-is; relative paths
    /// resolve from the REPO ROOT (detected by looking for .git or walk-up).
    /// </summary>
    public static string ResolveRepoPath(string configured)
    {
        if (Path.IsPathRooted(configured)) return Path.GetFullPath(configured);
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir is not null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, ".git"))) break;
            dir = dir.Parent;
        }
        var root = dir?.FullName ?? Directory.GetCurrentDirectory();
        return Path.GetFullPath(Path.Combine(root, configured));
    }

    /// <summary>
    /// Resolves the unified root output directory for all execution artifacts.
    /// Points to <see cref="AllureSettings.ResultsDirectory"/> (relative to repo root or absolute).
    /// </summary>
    public static string ResolveBaseResultsDirectory(AllureSettings? allure = null)
    {
        allure ??= SettingsManager.AllureSettings;
        return ResolveRepoPath(allure.ResultsDirectory);
    }

    /// <summary>
    /// Resolves the directory for browser evidence (screenshots, videos, traces, scenario steps).
    /// If <see cref="EvidenceSettings.StoragePath"/> is an absolute path, it is used as-is.
    /// Otherwise it is placed inside <see cref="ResolveBaseResultsDirectory(AllureSettings)"/>/evidence.
    /// </summary>
    public static string ResolveEvidenceDirectory(AllureSettings? allure = null, EvidenceSettings? evidence = null)
    {
        evidence ??= SettingsManager.FrameworkSettings.Evidence;
        if (Path.IsPathRooted(evidence.StoragePath))
            return Path.GetFullPath(evidence.StoragePath);

        var baseDir = ResolveBaseResultsDirectory(allure);
        var rel = evidence.StoragePath.TrimStart('.', '/', '\\');
        if (string.IsNullOrWhiteSpace(rel)) rel = "evidence";
        return Path.GetFullPath(Path.Combine(baseDir, rel));
    }

    /// <summary>
    /// Resolves the directory for execution run history (.jsonl files).
    /// If <see cref="ReportingSettings.HistoryDirectory"/> is an absolute path, it is used as-is.
    /// Otherwise it is placed inside <see cref="ResolveBaseResultsDirectory(AllureSettings)"/>/history.
    /// </summary>
    public static string ResolveHistoryDirectory(AllureSettings? allure, ReportingSettings reporting)
    {
        string historyBase;
        if (Path.IsPathRooted(reporting.HistoryDirectory))
        {
            historyBase = Path.GetFullPath(reporting.HistoryDirectory);
        }
        else
        {
            var baseDir = ResolveBaseResultsDirectory(allure);
            var rel = reporting.HistoryDirectory.TrimStart('.', '/', '\\');
            if (string.IsNullOrWhiteSpace(rel)) rel = "history";
            historyBase = Path.GetFullPath(Path.Combine(baseDir, rel));
        }

        return reporting.HistoryMode == HistoryMode.PerRun
            ? Path.Combine(historyBase, RunContext.RunId)
            : historyBase;
    }

    /// <summary>
    /// Overload for backwards compatibility with single-parameter calls.
    /// </summary>
    public static string ResolveHistoryDirectory(ReportingSettings reporting)
        => ResolveHistoryDirectory(null, reporting);

    /// <summary>
    /// Resolves the effective history file name.
    /// </summary>
    public static string ResolveHistoryFileName(ReportingSettings reporting)
        => reporting.HistoryMode == HistoryMode.PerRun
            ? $"runs-{RunContext.RunId}.jsonl"
            : RunHistory.DefaultFileName;

    /// <summary>
    /// Resolves the base folder for raw Allure result files:
    /// <c>{ResultsDirectory}/results/</c>.
    /// </summary>
    public static string ResolveResultsBaseDirectory(AllureSettings? allure = null)
        => Path.Combine(ResolveBaseResultsDirectory(allure), "results");

    /// <summary>
    /// Resolves the Allure results directory for the CURRENT execution.
    /// In <see cref="AllureHistoryMode.Cumulative"/> mode, writes to <c>{ResultsDirectory}/results/{runId}/</c>.
    /// In <see cref="AllureHistoryMode.Overwrite"/> mode, writes directly to <c>{ResultsDirectory}/results/</c>.
    /// </summary>
    public static string ResolveRunResultsDirectory(AllureSettings allure, ReportingSettings reporting)
    {
        var resultsBase = ResolveResultsBaseDirectory(allure);
        return reporting.AllureHistory == AllureHistoryMode.Cumulative
            ? Path.Combine(resultsBase, RunContext.RunId)
            : resultsBase;
    }

    /// <summary>
    /// Resolves the cumulative rollup directory: <c>{ResultsDirectory}/results/latest/</c>.
    /// The HTML report is generated from this directory in cumulative mode.
    /// </summary>
    public static string ResolveCumulativeResultsDirectory(AllureSettings? allure = null)
        => Path.Combine(ResolveResultsBaseDirectory(allure), "latest");

    /// <summary>
    /// Resolves the generated Allure HTML report directory:
    /// <c>{ResultsDirectory}/report/</c>.
    /// </summary>
    public static string ResolveReportDirectory(AllureSettings? allure = null)
        => Path.Combine(ResolveBaseResultsDirectory(allure), "report");

    /// <summary>
    /// Prints a one-line summary of the run history (pass count) and where it lives.
    /// </summary>
    public static void PrintHistorySummary(ReportingSettings reporting)
    {
        if (!reporting.HistoryEnabled) return;

        var dir = ResolveHistoryDirectory(reporting);
        var file = Path.Combine(dir, ResolveHistoryFileName(reporting));
        var recent = RunHistory.ReadRecent(dir, reporting.HistoryReadLimit, ResolveHistoryFileName(reporting));
        var passed = recent.Count(r => r.Status == "PASSED");
        Console.WriteLine($"[FRAMEWORK] Recorded history — {passed}/{recent.Count} passed in the last {recent.Count} entries.");
        Console.WriteLine($"[FRAMEWORK] Run history file: {file}");
    }

    /// <summary>
    /// Produces and/or opens the Allure report according to configuration,
    /// always through the Allure CLI:
    /// <list type="bullet">
    ///   <item><description><see cref="ReportingSettings.GenerateReport"/> = OnceAtEnd →
    ///   <c>allure generate {results} -o {ResultsDirectory}/report --clean</c>.</description></item>
    ///   <item><description><see cref="AllureSettings.AutoOpenReport"/> (or legacy
    ///   <see cref="ReportingSettings.ServeReport"/>) → <c>allure open</c> (if generated)
    ///   or <c>allure serve</c> (if not generated). Leaves server running.</description></item>
    /// </list>
    /// Failures are reported, never thrown, so they cannot mask the real test result.
    /// </summary>
    public static void FinalizeReport(AllureSettings allure, ReportingSettings reporting)
    {
        var baseDir = ResolveBaseResultsDirectory(allure);
        var resultsBase = ResolveResultsBaseDirectory(allure);
        var runDir = ResolveRunResultsDirectory(allure, reporting);
        var outDir = ResolveReportDirectory(allure);
        var generated = false;

        if (reporting.GenerateReport == ReportGeneration.OnceAtEnd)
        {
            try
            {
                Directory.CreateDirectory(baseDir);
                using (AcquireReportLock(Path.Combine(baseDir, ".report.lock")))
                    generated = GenerateUnderLock(allure, reporting, runDir, resultsBase, outDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[REPORT] Report generation skipped: {ex.Message}");
            }
        }

        if (!allure.AutoOpenReport && !reporting.ServeReport) return;

        if (generated)
            AllureReportHelper.OpenReport(outDir, allure.ReportPort, allure.AllureCliPath);
        else
            AllureReportHelper.ServeReport(runDir, allure.ReportPort, allure.AllureCliPath);
    }

    /// <summary>
    /// Waits (up to 3 minutes) for the cross-process report lock. On timeout it
    /// proceeds without the lock: a possible race is better than no report.
    /// </summary>
    private static IDisposable? AcquireReportLock(string lockFile)
    {
        var deadline = DateTime.UtcNow.AddMinutes(3);
        while (true)
        {
            try
            {
                return new FileStream(lockFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 1, FileOptions.DeleteOnClose);
            }
            catch (IOException) when (DateTime.UtcNow < deadline)
            {
                Thread.Sleep(1000);
            }
            catch (IOException)
            {
                Console.WriteLine("[REPORT] Report lock timed out; generating without it.");
                return null;
            }
        }
    }

    /// <summary>
    /// Rollup + history + <c>allure generate</c>, executed while holding the
    /// cross-process report lock. Returns whether the HTML report was produced.
    /// </summary>
    private static bool GenerateUnderLock(
        AllureSettings allure,
        ReportingSettings reporting,
        string runDir,
        string resultsBase,
        string outDir)
    {
        var resultsDir = runDir;

        if (reporting.AllureHistory == AllureHistoryMode.Cumulative)
        {
            resultsDir = ResolveCumulativeResultsDirectory(allure);
            // 1. Drop folders beyond the retention window FIRST so stale runs
            //    can never leak back into the rollup rebuilt below.
            PruneOldRuns(resultsBase, reporting.KeepPastRuns);
            // 2. Rebuild the rollup from the retained per-run folders.
            RebuildCumulativeResults(resultsBase, resultsDir);
        }

        // Seed Allure's own history (results/history/*.json) so trends render.
        AllureReportHelper.CopyHistory(resultsDir, outDir);
        AllureReportHelper.WriteEnvironment(resultsDir,
            SettingsManager.DeviceSettings.Browser.ToString(),
            SettingsManager.FrameworkSettings.Environment);
        AllureReportHelper.WriteCategories(resultsDir);
        AllureReportHelper.WriteExecutor(resultsDir, allure.ProjectName);

        var ok = AllureReportHelper.GenerateReport(resultsDir, outDir, allure.AllureCliPath);
        if (ok) Console.WriteLine($"[REPORT] Allure HTML report: {outDir}{Path.DirectorySeparatorChar}index.html");
        Console.WriteLine($"[REPORT] This execution results: {runDir}");
        if (reporting.AllureHistory == AllureHistoryMode.Cumulative)
            Console.WriteLine($"[REPORT] Cumulative results (all runs): {resultsDir}");
        return ok;
    }

    /// <summary>
    /// Rebuilds the cumulative rollup from scratch using only the retained
    /// per-run folders.
    /// </summary>
    private static void RebuildCumulativeResults(string baseDir, string cumulativeDir)
    {
        Directory.CreateDirectory(cumulativeDir);
        foreach (var file in Directory.GetFiles(cumulativeDir))
            try { File.Delete(file); } catch { /* best-effort */ }

        foreach (var run in ListRunDirectories(baseDir))
            AllureReportHelper.MergeResults(run.FullName, cumulativeDir);
    }

    /// <summary>
    /// Lists per-run Allure result folders (excludes <c>latest/</c> and hidden
    /// lock files), oldest first. Used for retention and deterministic rebuilds.
    /// </summary>
    public static IReadOnlyList<DirectoryInfo> ListRunDirectories(string allureBaseDir)
    {
        if (!Directory.Exists(allureBaseDir)) return [];
        return Directory.GetDirectories(allureBaseDir)
            .Select(d => new DirectoryInfo(d))
            .Where(d => !d.Name.Equals("latest", StringComparison.OrdinalIgnoreCase)
                && !d.Name.StartsWith(".", StringComparison.Ordinal))
            .OrderBy(d => d.LastWriteTimeUtc)
            .ThenBy(d => d.Name, StringComparer.Ordinal)
            .ToArray();
    }

    /// <summary>
    /// Deletes per-run result folders beyond the <paramref name="keep"/> most
    /// recent. Never touches <c>latest/</c>, the lock file or loose files.
    /// </summary>
    private static void PruneOldRuns(string baseDir, int keep)
    {
        try
        {
            if (!Directory.Exists(baseDir) || keep < 1) return;
            var stale = ListRunDirectories(baseDir);
            if (stale.Count <= keep) return;
            foreach (var old in stale.Take(stale.Count - keep))
                try { old.Delete(recursive: true); }
                catch { /* best-effort */ }
        }
        catch { /* pruning must never fail the run */ }
    }
}
