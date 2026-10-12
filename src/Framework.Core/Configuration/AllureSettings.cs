namespace Framework.Core.Configuration;

/// <summary>
/// Allure reporting configuration.
/// </summary>
public class AllureSettings {
    private string _resultsDirectory = "allure-results";

    /// <summary>
    /// Gets or sets the Allure results directory. Supports relative and absolute paths.
    /// In CI pipelines, can also be supplied via the ALLURE_RESULTS_DIR environment variable.
    /// Default: allure-results
    /// </summary>
    public string ResultsDirectory
    {
        get
        {
            var env = Environment.GetEnvironmentVariable("ALLURE_RESULTS_DIR");
            return !string.IsNullOrWhiteSpace(env) && _resultsDirectory == "allure-results"
                ? env
                : _resultsDirectory;
        }
        set => _resultsDirectory = value;
    }
    
    /// <summary>
    /// Gets or sets the project name.
    /// Default: Enterprise Automation Framework
    /// </summary>
    public string ProjectName { get; set; } = "Enterprise Automation Framework";
    
    /// <summary>
    /// Gets or sets whether to open the report at the end of the run with the
    /// Allure CLI: <c>allure open allure-report</c> when the HTML was generated
    /// (<see cref="ReportingSettings.GenerateReport"/> = OnceAtEnd), otherwise
    /// <c>allure serve {results}</c>. The server window is left running.
    /// Default: false
    /// </summary>
    public bool AutoOpenReport { get; set; } = false;
    
    /// <summary>
    /// Gets or sets the Allure report port.
    /// Default: 56789
    /// </summary>
    public int ReportPort { get; set; } = 56789;
    
    /// <summary>
    /// Gets or sets the Allure CLI path (if not in PATH).
    /// </summary>
    public string? AllureCliPath { get; set; }
}
