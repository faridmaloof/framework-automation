using Framework.Core.Configuration;
using Microsoft.Playwright;

namespace Framework.Core.Lifecycle;

/// <summary>
/// Evidence policy for web scenarios (screenshots, traces, video), extracted
/// from the hooks so the evidence strategy lives in Core and can be extended
/// or swapped without touching the test project. <see cref="WebLifecycleHooks"/>
/// only calls <see cref="CollectAsync"/>; it decides nothing.
/// </summary>
public static class EvidenceCollector
{
    /// <summary>
    /// Artifact paths produced for one scenario, so the caller can register
    /// them as scenario-scoped (test-level) attachments without scanning
    /// shared folders. Paths are only present when the file was actually
    /// written (failure screenshot, kept trace).
    /// </summary>
    /// <param name="ScreenshotPath">Full-page screenshot captured on failure.</param>
    /// <param name="TracePath">Playwright trace zip kept on failure.</param>
    public sealed record EvidenceArtifacts(string? ScreenshotPath, string? TracePath);

    /// <summary>
    /// Captures and finalizes evidence for a finished web scenario according to
    /// the configured <see cref="EvidenceSettings"/>: screenshot on failure,
    /// trace kept only on failure, and video discarded on pass when set to
    /// <see cref="VideoMode.OnFailure"/>. Returns the paths of the artifacts
    /// that exist on disk (null when nothing was produced), so the caller can
    /// attach them to the scenario without scanning shared folders.
    /// </summary>
    /// <param name="page">The scenario page.</param>
    /// <param name="context">The scenario browser context (owns tracing).</param>
    /// <param name="settings">The effective evidence settings.</param>
    /// <param name="scenarioTitle">Scenario title (used for artifact names).</param>
    /// <param name="failed">Whether the scenario failed.</param>
    public static async Task<EvidenceArtifacts> CollectAsync(
        IPage page,
        IBrowserContext context,
        EvidenceSettings settings,
        string scenarioTitle,
        bool failed) {
        string? screenshotPath = null;
        string? tracePath = null;

        if (failed && settings.Screenshot != ScreenshotMode.Disabled)
        {
            var dir = Lifecycle.ReportService.ResolveEvidenceDirectory();
            Directory.CreateDirectory(Path.Combine(dir, "screenshots"));
            var file = Path.Combine(dir, "screenshots", $"{Safe(scenarioTitle)}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.png");
            try { await page.ScreenshotAsync(new PageScreenshotOptions { Path = file, FullPage = true }); screenshotPath = file; }
            catch { /* the page may already be closed */ }
            Console.WriteLine($"[EVIDENCE] Screenshot: {file}");
        }

        if (settings.Trace)
        {
            var dir = Lifecycle.ReportService.ResolveEvidenceDirectory();
            Directory.CreateDirectory(Path.Combine(dir, "traces"));
            var file = Path.Combine(dir, "traces", $"{Safe(scenarioTitle)}-{DateTime.UtcNow:yyyyMMdd-HHmmss}.zip");
            try { await context.Tracing.StopAsync(new TracingStopOptions { Path = file }); }
            catch { /* tracing may not have started */ }
            if (failed)
            {
                tracePath = file;
                Console.WriteLine($"[EVIDENCE] Trace: {file}");
            }
            else TryDelete(file);
        }

        // Video handling: OnFailure + !failed discards the file (keeps the
        // report lean); OnFailure + failed keeps it; Always keeps it
        // unconditionally (not blocking the run, the browser recorded it in the
        // background). The video path is resolved earlier (in BeforeScenario)
        // and registered by the caller, so no scanning happens here.
        if (settings.Video == VideoMode.OnFailure && !failed)
            try { TryDelete(await page.Video!.PathAsync()); } catch { /* no video recorded */ }

        return new EvidenceArtifacts(screenshotPath, tracePath);
    }

    private static string Safe(string value)
        => new([.. value.Select(c => char.IsLetterOrDigit(c) || c is '-' or '_' ? c : '_')]);

    private static void TryDelete(string? path)
    {
        if (path != null && File.Exists(path)) File.Delete(path);
    }
}
