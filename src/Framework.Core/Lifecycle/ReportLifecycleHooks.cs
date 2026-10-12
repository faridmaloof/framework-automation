using Framework.Core.Configuration;
using Reqnroll;

namespace Framework.Core.Lifecycle;

/// <summary>
/// Core-owned end-of-run reporting hook. Once every scenario has finished it
/// delegates to <see cref="ReportService.FinalizeReport"/>, which generates
/// (<c>allure generate</c>) and/or opens (<c>allure open</c> / <c>allure serve</c>)
/// the Allure report according to configuration. Reporting must never fail
/// the run, so every step is best-effort.
/// </summary>
[Binding]
public sealed class ReportLifecycleHooks {
    /// <summary>
    /// Runs once at the very end of the test run: generates the report per
    /// <see cref="ReportingSettings.GenerateReport"/> and, if
    /// <see cref="AllureSettings.AutoOpenReport"/> is enabled, opens it with the
    /// Allure CLI.
    /// </summary>
    [AfterTestRun]
    public static void AfterTestRun()
    {
        try
        {
            ReportService.FinalizeReport(SettingsManager.AllureSettings, SettingsManager.FrameworkSettings.Reporting);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[REPORT] Report finalize/open skipped: " + ex.Message);
        }
    }
}