namespace Framework.Core.Configuration;

/// <summary>
/// Timeout settings.
/// </summary>
public class TimeoutSettings {
    /// <summary>
    /// Gets or sets the global timeout in seconds.
    /// Default: 30
    /// </summary>
    public int GlobalTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the navigation timeout in seconds.
    /// Default: 30
    /// </summary>
    public int NavigationTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Gets or sets the action timeout in seconds.
    /// Default: 10
    /// </summary>
    public int ActionTimeoutSeconds { get; set; } = 10;
}

/// <summary>
/// Evidence collection settings.
/// </summary>
public class EvidenceSettings : IEvidenceSettings {
    /// <summary>
    /// Gets or sets the screenshot capture mode.
    /// Default: PreAndPost
    /// </summary>
    public ScreenshotMode Screenshot { get; set; } = ScreenshotMode.PreAndPost;

    /// <summary>
    /// Gets or sets the video recording mode.
    /// Default: OnFailure
    /// </summary>
    public VideoMode Video { get; set; } = VideoMode.OnFailure;

    /// <summary>
    /// Gets or sets whether to enable trace recording.
    /// Default: true
    /// </summary>
    public bool Trace { get; set; } = true;

    /// <summary>
    /// Gets or sets the evidence storage path.
    /// Default: ./evidence
    /// </summary>
    public string StoragePath { get; set; } = "./evidence";
}

/// <summary>
/// Parallelism settings.
/// </summary>
public class ParallelismSettings {
    /// <summary>
    /// Gets or sets the maximum number of parallel workers.
    /// Default: 4
    /// </summary>
    public int MaxWorkers { get; set; } = 4;

    /// <summary>
    /// Gets or sets whether to enable sharding for CI.
    /// Default: false
    /// </summary>
    public bool EnableSharding { get; set; } = false;
}

/// <summary>
/// Device/browser settings.
/// </summary>
public class DeviceSettings : IDeviceSettings {
    /// <summary>
    /// Gets or sets the browser type.
    /// Default: Chromium
    /// </summary>
    public BrowserType Browser { get; set; } = BrowserType.Chromium;

    /// <summary>
    /// Gets or sets whether to run in headless mode. This is the ONE and only
    /// place in the whole configuration where headless is defined.
    /// Default: true (fastest and CI-standard; set false only to debug locally)
    /// </summary>
    public bool Headless { get; set; } = true;

    /// <summary>
    /// Gets or sets the viewport width. Binds from "ViewportWidth" in appsettings.
    /// Default: 1920
    /// </summary>
    public int ViewportWidth { get; set; } = 1920;

    /// <summary>
    /// Gets or sets the viewport height. Binds from "ViewportHeight" in appsettings.
    /// Default: 1080
    /// </summary>
    public int ViewportHeight { get; set; } = 1080;
}

/// <summary>
/// Reporting settings: clear, human-friendly run output + run history +
/// optional Allure report generation and live report server.
/// </summary>
public class ReportingSettings {
    /// <summary>
    /// Gets or sets whether every scenario result is appended to a run-history
    /// file (JSON lines) so behaviour can be tracked over time.
    /// Default: true
    /// </summary>
    public bool HistoryEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets how history is stored: <see cref="HistoryMode.Shared"/>
    /// (one file for all runs — default) or <see cref="HistoryMode.PerRun"/>
    /// (a folder/file per execution, keyed by run id, never overwritten).
    /// Default: Shared
    /// </summary>
    public HistoryMode HistoryMode { get; set; } = HistoryMode.Shared;

    /// <summary>
    /// Gets or sets the run-history directory.
    /// Default: ./history
    /// </summary>
    public string HistoryDirectory { get; set; } = "./history";

    /// <summary>
    /// Gets or sets how many historical runs are kept in memory when reading
    /// the history (older entries remain on disk).
    /// Default: 50
    /// </summary>
    public int HistoryReadLimit { get; set; } = 50;

    /// <summary>
    /// Gets or sets how the Allure results of consecutive executions are
    /// accumulated. <see cref="AllureHistoryMode.Cumulative"/> (default) keeps
    /// every execution in its own folder plus a cumulative rollup, so re-running
    /// 20 tests 30 minutes later appends the new results and Allure shows the
    /// previous vs new outcome per test (history/trends/retries).
    /// <see cref="AllureHistoryMode.Overwrite"/> replaces previous results.
    /// Default: Cumulative.
    /// </summary>
    public AllureHistoryMode AllureHistory { get; set; } = AllureHistoryMode.Cumulative;

    /// <summary>
    /// Gets or sets how many past per-run result folders are kept on disk.
    /// Older folders are pruned at the end of each run (the cumulative rollup
    /// and the JSON-lines history are never pruned). Default: 20.
    /// </summary>
    public int KeepPastRuns { get; set; } = 20;

    /// <summary>
    /// Gets or sets when the Allure HTML report is produced.
    /// <see cref="ReportGeneration.OnceAtEnd"/> builds it once after the whole
    /// run; <see cref="ReportGeneration.None"/> skips it. Default: OnceAtEnd.
    /// </summary>
    public ReportGeneration GenerateReport { get; set; } = ReportGeneration.OnceAtEnd;

    /// <summary>
    /// Legacy alias of <see cref="AllureSettings.AutoOpenReport"/>: when true the
    /// report is opened with the Allure CLI at the end of the run. Default: false.
    /// </summary>
    public bool ServeReport { get; set; }

    /// <summary>
    /// Legacy; the server port is taken from <see cref="AllureSettings.ReportPort"/>.
    /// Default: 56789.
    /// </summary>
    public int ReportPort { get; set; } = 56789;
}

/// <summary>
/// Main framework settings (single source of truth, section "Framework").
/// Browser identity lives in <see cref="DeviceSettings"/> ("Device" section);
/// headless mode is defined ONLY there to avoid conflicting flags.
/// </summary>
public class FrameworkSettings : IFrameworkSettings {
    /// <summary>
    /// Gets or sets the environment name.
    /// Default: Development
    /// </summary>
    public string Environment { get; set; } = "Development";

    /// <summary>
    /// Gets or sets the culture.
    /// Default: en-US
    /// </summary>
    public string Culture { get; set; } = "en-US";

    /// <summary>
    /// Gets or sets the timeout settings.
    /// </summary>
    public TimeoutSettings Timeouts { get; set; } = new();

    /// <summary>
    /// Gets or sets the evidence settings (screenshots, video, traces).
    /// </summary>
    public EvidenceSettings Evidence { get; set; } = new();

    /// <summary>
    /// Gets or sets the reporting settings (run history and output).
    /// </summary>
    public ReportingSettings Reporting { get; set; } = new();

    /// <summary>
    /// Gets or sets the parallelism settings.
    /// </summary>
    public ParallelismSettings Parallelism { get; set; } = new();

    /// <summary>
    /// Gets or sets the observability settings.
    /// </summary>
    public ObservabilitySettings Observability { get; set; } = new();
}
