using Framework.Core.Configuration;
using Framework.Core.ScreenPlay.Api.Tasks;
using Framework.Core.ScreenPlay.Interfaces;
using Framework.Core.Tools;
using Microsoft.Playwright;
using Reqnroll;

namespace Framework.Core.Lifecycle;

/// <summary>
/// Core-owned run-history/reporting lifecycle that runs for every scenario.
/// Captures per-step evidence, scenario status, duration and tags, and
/// writes a native Allure-2 result file for the run.
/// </summary>
/// <param name="featureContext">The per-feature feature context.</param>
/// <param name="scenarioContext">The per-scenario scenario context.</param>
[Binding]
public sealed class RunHistoryHooks(FeatureContext featureContext, ScenarioContext scenarioContext) {
    private DateTimeOffset _start;
    private readonly List<AllureReportHelper.AllureStep> _steps = [];

    // Per-step registration state (one step at a time).
    private int _stepIndex;
    private long _stepStartMs;
    private int _requestCountAtStepStart;

    /// <summary>Marks the start time used to compute the scenario duration and
    /// initialises the step counter used by the step hooks.</summary>
    [BeforeScenario(Order = 0)]
    public void BeforeScenario()
    {
        _start = DateTimeOffset.UtcNow;
        _steps.Clear();
        _stepIndex = 0;
        scenarioContext["StepIndex"] = 0;
        try
        {
            Directory.CreateDirectory(ReportService.ResolveRunResultsDirectory(SettingsManager.AllureSettings, SettingsManager.FrameworkSettings.Reporting));
        }
        catch { }
    }

    /// <summary>
    /// Evidence root for the CURRENT scenario (shared with the web hooks via
    /// <see cref="ScenarioContextExtensions.ScenarioEvidenceRoot"/>), so a step
    /// only ever picks up its own artifacts — no cross-scenario leakage.
    /// </summary>
    private string ScenarioEvidenceRoot() => scenarioContext.ScenarioEvidenceRoot();

    /// <summary>
    /// Runs for every scenario after it finishes: disposes the scenario actors,
    /// records timing and appends the run-history entry.
    /// </summary>
    /// <summary>
    /// Runs after each scenario (Order = 0): disposes the scenario actors and
    /// writes the Allure result + run-history entry. Runs AFTER
    /// <c>WebLifecycleHooks.AfterScenario</c> (lower Order runs first for After
    /// hooks), so web evidence paths are already registered in the context.
    /// </summary>
    [AfterScenario(Order = 0)]
    public void AfterScenario()
    {
        foreach (var actor in scenarioContext.Values.OfType<IActor>().Distinct())
            (actor as IDisposable)?.Dispose();

        var fw = SettingsManager.FrameworkSettings;
        var title = scenarioContext.ScenarioInfo.Title;
        var failed = scenarioContext.TestError != null;
        var duration = DateTimeOffset.UtcNow - _start;
        var status = failed ? "FAILED" : "PASSED";

        Console.WriteLine($"[SCENARIO] {status}: {title} ({duration.TotalSeconds:F2}s)");

        var allureDir = ReportService.ResolveRunResultsDirectory(SettingsManager.AllureSettings, fw.Reporting);
        WriteAllureResult(
            allureDir,
            featureContext.FeatureInfo.Title,
            title,
            !failed,
            (long)duration.TotalMilliseconds,
            [.. scenarioContext.ScenarioInfo.Tags],
            failed ? scenarioContext.TestError?.Message : null,
            failed ? scenarioContext.TestError?.ToString() : null,
            _steps.ToArray(),
            BuildCategories());

        if (!fw.Reporting.HistoryEnabled) return;

        RunHistory.Append(
            ReportService.ResolveHistoryDirectory(SettingsManager.AllureSettings, fw.Reporting),
            new ScenarioRunEntry(
                DateTime.UtcNow.ToString("o"),
                featureContext.FeatureInfo.Title,
                title,
                status,
                Math.Round(duration.TotalSeconds, 2),
                [.. scenarioContext.ScenarioInfo.Tags],
                failed ? Truncate(scenarioContext.TestError!.Message, 300) : null
            ),
            ReportService.ResolveHistoryFileName(fw.Reporting));
    }

    /// <summary>
    /// Runs before each step: registers the real Gherkin step text (so
    /// screenshots are named after the step), snapshots the API request
    /// counter and captures the PRE-step screenshot (web only). This hook is
    /// the single owner of step evidence, so there is no cross-class
    /// ordering dependency: the screenshot always lands in THIS step's folder.
    /// </summary>
    [BeforeStep(Order = 1000)]
    public async Task BeforeStep()
    {
        _stepIndex = scenarioContext.GetStepIndex();
        _stepStartMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Real step text (e.g. "When I search for 'playwright'").
        var text = scenarioContext.StepContext?.StepInfo?.Text ?? "step";
        scenarioContext["StepName"] = text;

        // Snapshot the request counter at step start. AfterStep compares it to
        // the post-step value to know whether THIS step issued an HTTP request.
        _requestCountAtStepStart = scenarioContext.Values.OfType<IActor>()
            .Where(a => a.HasRemembered(ApiMemoryKeys.RequestCount))
            .Select(a => a.Remembered<int>(ApiMemoryKeys.RequestCount))
            .DefaultIfEmpty(0)
            .Max();

        // PRE-step screenshot (web scenarios only; API has no "Page").
        await CaptureStepScreenshotAsync(_stepIndex, text, "pre");
    }

    /// <summary>
    /// Runs after each step: captures the POST-step screenshot, writes the
    /// HTTP request/response evidence (when the step issued a call), collects
    /// THIS step's artifacts and registers the step in Allure with its status,
    /// timing, parameters and attachments. Because this class owns the whole
    /// pipeline, artifacts are always complete and correctly matched to the
    /// step before <see cref="ScenarioContextExtensions.GetStepIndex"/> is
    /// advanced (in <c>finally</c>).
    /// </summary>
    [AfterStep(Order = -1000)]
    public async Task AfterStep()
    {
        try
        {
            var name = scenarioContext.GetString("StepName") ?? "step";
            var error = scenarioContext.TestError;
            var status = error != null ? "failed" : "passed";
            var detail = error?.Message;

            // 1) POST-step screenshot BEFORE collecting (web scenarios only),
            //    so it lands in THIS step's folder. A failed step gets a
            //    full-page "failure" screenshot instead, attached to the step
            //    that actually failed (any enabled screenshot mode).
            await CaptureStepScreenshotAsync(_stepIndex, name, error != null ? "failure" : "post");

            // 2) HTTP evidence: only when THIS step issued a request. Resolve
            //    the actor fresh (it may be created inside the step body) and
            //    compare request counters against the pre-step snapshot.
            var parameters = new List<AllureReportHelper.AllureParameter>();
            var actorNow = scenarioContext.Values.OfType<IActor>()
                .FirstOrDefault(a => a.HasRemembered(ApiMemoryKeys.LastRequest));
            var requestCountNow = scenarioContext.Values.OfType<IActor>()
                .Where(a => a.HasRemembered(ApiMemoryKeys.RequestCount))
                .Select(a => a.Remembered<int>(ApiMemoryKeys.RequestCount))
                .DefaultIfEmpty(0)
                .Max();
            if (actorNow != null && requestCountNow > _requestCountAtStepStart)
                BuildHttpEvidence(actorNow, _stepIndex, parameters, ref detail);

            // 3) Collect THIS step's artifacts (folder is now complete).
            var attachments = CollectStepAttachments(_stepIndex).ToList();

            _steps.Add(new AllureReportHelper.AllureStep(
                name,
                status,
                _stepStartMs,
                DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                parameters.Count > 0 ? parameters : null,
                detail != null ? new AllureReportHelper.AllureStepDetail(detail, error?.ToString()) : null,
                attachments));
        }
        catch
        {
            // Reporting must never fail the run: register a minimal step.
            var name = scenarioContext.GetString("StepName") ?? "step";
            var start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            _steps.Add(new AllureReportHelper.AllureStep(name, "passed", start, start));
        }
        finally
        {
            scenarioContext["StepIndex"] = _stepIndex + 1;
        }
    }

    /// <summary>
    /// Picks up evidence files for THIS scenario.
    /// </summary>
    private IEnumerable<AllureReportHelper.AllureAttachmentRef> CollectStepAttachments(int stepIndex) {
        var files = new List<AllureReportHelper.AllureAttachmentRef>();
        try {
            var dir = ScenarioEvidenceRoot();
            if (!Directory.Exists(dir)) return files;

            var stepDir = Path.Combine(dir, "step-evidence", stepIndex.ToString("D3"));
            if (!Directory.Exists(stepDir)) return files;

            // Collect every artifact produced for this step, in creation order:
            // pre/post/failure screenshots (*.png) and the HTTP request/response
            // (*.txt) written by BuildHttpEvidence.
            var stepFiles = Directory.EnumerateFiles(stepDir, "*.*", SearchOption.AllDirectories)
                .OrderBy(File.GetCreationTimeUtc);
            foreach (var file in stepFiles)
            {
                var ext = Path.GetExtension(file).ToLowerInvariant();
                var mime = ext switch {
                    ".png" => "image/png",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".txt" => "text/plain",
                    _ => null,
                };
                if (mime is null) continue;

                try {
                    var info = new FileInfo(file);
                    if (info.Length > 0)
                        files.Add(new AllureReportHelper.AllureAttachmentRef(
                            AttachmentDisplayName(file),
                            file,
                            mime));
                }
                catch { }
            }
        }
        catch { }
        return files;
    }

    /// <summary>
    /// Human-readable attachment name shown in the Allure step.
    /// </summary>
    private static string AttachmentDisplayName(string file)
    {
        var name = Path.GetFileNameWithoutExtension(file);
        if (name.EndsWith("-pre", StringComparison.Ordinal)) return "Screenshot (before step)";
        if (name.EndsWith("-post", StringComparison.Ordinal)) return "Screenshot (after step)";
        if (name.EndsWith("-failure", StringComparison.Ordinal)) return "Screenshot (failure)";
        return name switch {
            "http-request" => "HTTP request",
            "http-response" => "HTTP response",
            _ => Path.GetFileName(file),
        };
    }

    /// <summary>
    /// Reads the last HTTP exchange from the actor's memory, exposes it as step
    /// parameters (method, url, status, elapsed) and writes a readable
    /// request/response dump into the step's evidence folder so it is attached
    /// to the step in the report.
    /// </summary>
    private void BuildHttpEvidence(
        IActor actor,
        int stepIndex,
        List<AllureReportHelper.AllureParameter> parameters,
        ref string? detail) {
        try
        {
            var request = actor.Remembered<string>(ApiMemoryKeys.LastRequest) ?? "?";
            var requestBody = actor.Remembered<string>(ApiMemoryKeys.LastRequestBody) ?? string.Empty;
            var status = actor.Remembered<int>(ApiMemoryKeys.LastStatusCode);
            var elapsed = actor.Remembered<long>(ApiMemoryKeys.LastElapsedMs);
            var body = actor.Remembered<string>(ApiMemoryKeys.LastBody) ?? string.Empty;
            var headers = actor.Remembered<IDictionary<string, string>>(ApiMemoryKeys.LastHeaders)
                          ?? new Dictionary<string, string>();

            var parts = request.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var method = parts.Length > 0 ? parts[0] : "?";
            var url = parts.Length > 1 ? parts[1] : request;

            parameters.Add(new AllureReportHelper.AllureParameter("http.method", method));
            parameters.Add(new AllureReportHelper.AllureParameter("http.url", url));
            parameters.Add(new AllureReportHelper.AllureParameter("http.status", status.ToString()));
            parameters.Add(new AllureReportHelper.AllureParameter("http.elapsed_ms", elapsed.ToString()));
            if (detail is null)
                detail = $"{method} {url} -> {status} ({elapsed} ms)";

            var dir = ScenarioEvidenceRoot();
            var stepDir = Path.Combine(dir, "step-evidence", stepIndex.ToString("D3"));
            Directory.CreateDirectory(stepDir);

            // --- REQUEST attachment (what was sent) ---
            var req = new System.Text.StringBuilder();
            req.AppendLine($"{method} {url}");
            if (!string.IsNullOrWhiteSpace(requestBody))
            {
                req.AppendLine();
                req.AppendLine("--- Request body ---");
                req.AppendLine(requestBody);
            }
            File.WriteAllText(Path.Combine(stepDir, "http-request.txt"), req.ToString());

            // --- RESPONSE attachment (what was received / validated) ---
            var res = new System.Text.StringBuilder();
            res.AppendLine($"{method} {url} -> {status}   ({elapsed} ms)");
            res.AppendLine("--- Response headers ---");
            foreach (var kv in headers)
                res.AppendLine($"{kv.Key}: {kv.Value}");
            res.AppendLine("--- Response body ---");
            res.AppendLine(body);
            File.WriteAllText(Path.Combine(stepDir, "http-response.txt"), res.ToString());

            // --- General log of THIS scenario (all exchanges) ---
            var log = new System.Text.StringBuilder();
            log.AppendLine($"===== {DateTime.UtcNow:HH:mm:ss}  {method} {url} -> {status} ({elapsed} ms) =====");
            log.Append(req);
            log.AppendLine();
            log.Append(res);
            File.AppendAllText(Path.Combine(dir, "http-log.txt"), log.ToString());
        }
        catch { /* evidence must never fail the run */ }
    }

    private static AllureReportHelper.AllureCategory[] BuildCategories()
    {
        return
        [
            new AllureReportHelper.AllureCategory("type", "bdd"),
            new AllureReportHelper.AllureCategory("framework", "reqnroll"),
            new AllureReportHelper.AllureCategory("language", "csharp"),
        ];
    }

    /// <summary>
    /// Captures a screenshot for a single step into the scenario's private
    /// <c>step-evidence/{index}</c> folder (web scenarios only — the page lives
    /// in <c>scenarioContext["Page"]</c>; API scenarios have none, so nothing is
    /// captured). <paramref name="phase"/> is <c>pre</c>, <c>post</c> or
    /// <c>failure</c>: pre/post respect <see cref="ScreenshotMode"/>; a failure
    /// screenshot (full page) is taken for any enabled mode and flagged so the
    /// scenario-level fallback does not duplicate it.
    /// </summary>
    private async Task CaptureStepScreenshotAsync(int stepIndex, string stepName, string phase)
    {
        var mode = SettingsManager.FrameworkSettings.Evidence.Screenshot;
        if (mode == ScreenshotMode.Disabled) return;

        var wanted = phase switch {
            "pre" => mode is ScreenshotMode.PreStep or ScreenshotMode.PreAndPost or ScreenshotMode.Always,
            "post" => mode is ScreenshotMode.PostStep or ScreenshotMode.PreAndPost or ScreenshotMode.Always,
            _ => true,
        };
        if (!wanted) return;

        if (!scenarioContext.TryGetValue("Page", out var pageObj) || pageObj is not IPage page || page is null)
            return;

        try
        {
            var stepDir = Path.Combine(ScenarioEvidenceRoot(), "step-evidence", stepIndex.ToString("D3"));
            Directory.CreateDirectory(stepDir);
            var file = Path.Combine(stepDir, $"{Safe(stepName)}-{phase}.png");
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = file, FullPage = phase == "failure" });
            if (phase == "failure") scenarioContext["StepFailureScreenshot"] = file;
            Console.WriteLine($"[EVIDENCE][STEP#{stepIndex}] {phase}: {file}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EVIDENCE][STEP#{stepIndex}] Could not capture {phase} screenshot: {ex.Message}");
        }
    }

    /// <summary>
    /// Removes characters that are not valid in file names, replacing them
    /// with an underscore.
    /// </summary>
    private static string Safe(string value) =>
        new([.. value.Select(c => char.IsLetterOrDigit(c) || c is '-' or '_' ? c : '_')]);

    /// <summary>
    /// Writes one native Allure-2 result file for the finished scenario.
    /// Best-effort: reporting must never fail the test run.
    /// </summary>
    private void WriteAllureResult(string resultsDir,
                                   string feature,
                                   string scenario,
                                   bool passed,
                                   long durationMs,
                                   string[] tags,
                                   string? errorMessage,
                                   string? errorTrace,
                                   AllureReportHelper.AllureStep[] steps,
                                   AllureReportHelper.AllureCategory[] categories) {
        try
        {
            var attachments = CollectScenarioAttachments();
            AllureReportHelper.WriteResult(
                resultsDir, feature, scenario, passed, durationMs,
                tags, errorMessage, errorTrace, attachments,
                steps, categories);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ALLURE] Could not write result for " + scenario + ": " + ex.Message);
        }
    }

    /// <summary>
    /// Scenario-scoped (test-level) attachments: the general HTTP log of THIS
    /// scenario plus the global web artifacts registered by
    /// <c>WebLifecycleHooks</c> — one video per browser session (primary and
    /// secondary personas), the trace and a failure screenshot only when the
    /// failure happened outside a step. Paths come from the scenario context
    /// and the scenario's private evidence root — no shared folder is scanned,
    /// so evidence can never leak across scenarios.
    /// </summary>
    private IEnumerable<string> CollectScenarioAttachments()
    {
        var files = new List<string>();
        void Add(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            try { if (File.Exists(path) && new FileInfo(path).Length > 0) files.Add(path); }
            catch { /* unreadable file */ }
        }

        // General log of THIS test (all HTTP exchanges of the scenario).
        try { Add(Path.Combine(ScenarioEvidenceRoot(), "http-log.txt")); } catch { }

        // Global web artifacts, registered by WebLifecycleHooks.
        if (scenarioContext.TryGetValue("VideoPaths", out var videos) && videos is IEnumerable<string> videoPaths)
            foreach (var video in videoPaths) Add(video);
        else
            Add(scenarioContext.GetString("VideoPath"));

        // Only attach scenario-level failure screenshot if the failing step didn't already record one
        if (!scenarioContext.ContainsKey("StepFailureScreenshot"))
            Add(scenarioContext.GetString("FailureScreenshotPath"));

        Add(scenarioContext.GetString("TracePath"));

        return files;
    }

    /// <summary>
    /// Truncates a string to a maximum length, appending an ellipsis if it was longer. Used to keep the run-history log concise.
    /// </summary>
    /// <param name="value"> The string to truncate. </param>
    /// <param name="max"> The maximum length of the string. </param>
    /// <returns>The truncated string.</returns>
    private static string Truncate(string value, int max) =>
        value.Length <= max ? value : value[..max] + "…";
}
