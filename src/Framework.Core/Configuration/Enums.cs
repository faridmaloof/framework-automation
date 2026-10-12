namespace Framework.Core.Configuration;

/// <summary>
/// How run history is stored on disk.
/// </summary>
public enum HistoryMode
{
    /// <summary>
    /// Append every execution to the SAME history file (default). Best for
    /// trend/flaky analysis across runs.
    /// </summary>
    Shared = 0,

    /// <summary>
    /// Give every execution its OWN history file/folder (keyed by the run id),
    /// so previous runs are never overwritten or mixed (spec §10.2).
    /// </summary>
    PerRun = 1,
}

/// <summary>
/// How the Allure results of consecutive executions are accumulated, so a test
/// executed twice (or 20 tests per run, re-run 30 minutes later) keeps the
/// previous result visible as history/trend instead of being overwritten.
/// </summary>
public enum AllureHistoryMode
{
    /// <summary>
    /// Overwrite: each execution replaces the previous results (old behaviour).
    /// Only the last run is visible. Default for backwards compatibility.
    /// </summary>
    Overwrite = 0,

    /// <summary>
    /// Per-run folders + cumulative rollup (recommended): every execution writes
    /// to <c>{results}/{runId}/</c> AND to a cumulative <c>{results}/latest/</c>
    /// folder; the HTML report is generated from <c>latest/</c> with the previous
    /// report's <c>history/</c> copied in, so Allure renders trends, retries and
    /// duration charts across runs. Per-run folders are never deleted, so any
    /// past execution can be re-opened individually.
    /// </summary>
    Cumulative = 1,
}

/// <summary>
/// When (and whether) the Allure HTML report is produced at the end of a run.
/// </summary>
public enum ReportGeneration
{
    /// <summary>Do not generate a report.</summary>
    None = 0,

    /// <summary>Generate the report once, at the end of the whole run.</summary>
    OnceAtEnd = 1,
}

/// <summary>
/// Browser type enumeration with full support for all major browsers.
/// </summary>
public enum BrowserType
{
    /// <summary>
    /// Chromium browser (open-source base for Chrome, Edge, etc.).
    /// </summary>
    Chromium = 0,

    /// <summary>
    /// Google Chrome browser.
    /// </summary>
    Chrome = 1,

    /// <summary>
    /// Microsoft Edge browser (Chromium-based).
    /// </summary>
    Edge = 2,

    /// <summary>
    /// Mozilla Firefox browser.
    /// </summary>
    Firefox = 3,

    /// <summary>
    /// Apple Safari browser (WebKit).
    /// </summary>
    Safari = 4,

    /// <summary>
    /// Internet Explorer (legacy, via Edge IE mode).
    /// </summary>
    InternetExplorer = 5,

    /// <summary>
    /// WebKit browser (open-source base for Safari).
    /// </summary>
    WebKit = 6,

    /// <summary>
    /// Thorium browser (optimized Chromium fork).
    /// </summary>
    Thorium = 7,

    /// <summary>
    /// Brave browser (Chromium-based with privacy features).
    /// </summary>
    Brave = 8,

    /// <summary>
    /// Opera browser (Chromium-based).
    /// </summary>
    Opera = 9
}

/// <summary>
/// Screenshot capture mode enumeration.
/// </summary>
public enum ScreenshotMode
{
    /// <summary>
    /// No screenshots.
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// Capture only on failure.
    /// </summary>
    OnFailure = 1,

    /// <summary>
    /// Capture before each step.
    /// </summary>
    PreStep = 2,

    /// <summary>
    /// Capture after each step.
    /// </summary>
    PostStep = 3,

    /// <summary>
    /// Capture before and after each step.
    /// </summary>
    PreAndPost = 4,

    /// <summary>
    /// Always capture (Pre + Post + OnFailure).
    /// </summary>
    Always = 5
}

/// <summary>
/// Video recording mode enumeration.
/// </summary>
public enum VideoMode
{
    /// <summary>
    /// No video recording.
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// Record only on failure.
    /// </summary>
    OnFailure = 1,

    /// <summary>
    /// Always record video.
    /// </summary>
    Always = 2
}

/// <summary>
/// Observability mode for metrics and tracing.
/// </summary>
public enum ObservabilityMode
{
    /// <summary>
    /// Observability disabled.
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// Direct export to Prometheus/OpenTelemetry.
    /// </summary>
    Direct = 1,

    /// <summary>
    /// Queue-based export (aggregated events).
    /// </summary>
    Queue = 2,

    /// <summary>
    /// Console output (development mode).
    /// </summary>
    Console = 3
}
