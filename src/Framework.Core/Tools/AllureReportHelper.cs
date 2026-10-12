using System.Diagnostics;
using System.Text.Json;

namespace Framework.Core.Tools;

/// <summary>
/// Allure report helper: writes native Allure-2 results (result JSON +
/// attachments + environment.properties + categories.json + executor.json)
/// and drives the Allure CLI to generate / serve / open the HTML report.
/// </summary>
public static class AllureReportHelper {
    /// <summary>
    /// Executable names probed in PATH. Scoop/choco expose the CLI as
    /// <c>allure.cmd</c>/<c>allure.bat</c> on Windows and <c>allure</c> on Unix.
    /// </summary>
    private static readonly string[] AllureExecutableNames = OperatingSystem.IsWindows()
        ? ["allure.cmd", "allure.bat", "allure.exe"]
        : ["allure"];

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Writes one Allure-2 test-result file for a finished scenario.
    /// Thread-safe: each scenario writes its own uuid-named file.
    /// </summary>
    public static string WriteResult(string resultsDirectory,
                                     string feature,
                                     string scenario,
                                     bool passed,
                                     long durationMs,
                                     IEnumerable<string>? tags = null,
                                     string? errorMessage = null,
                                     string? errorTrace = null,
                                     IEnumerable<string>? attachments = null,
                                     IEnumerable<AllureStep>? steps = null,
                                     IEnumerable<AllureCategory>? categories = null,
                                     string? testFramework = null) {
        Directory.CreateDirectory(resultsDirectory);
        var uuid = Guid.NewGuid().ToString("N");
        var tagList = (tags ?? []).Select(t => t.TrimStart('@')).ToArray();

        // Filter out invalid attachment paths (empty or missing files)
        var validAttachments = new List<string>();
        if (attachments is not null)
            foreach (var file in attachments) {
                if (string.IsNullOrWhiteSpace(file) || !File.Exists(file)) continue;
                try {
                    var info = new FileInfo(file);
                    if (info.Length > 0) validAttachments.Add(file);
                }
                catch { /* skip unreadable file */ }
            }

        var attachmentEntries = new List<object>();
        if (validAttachments.Count > 0)
            foreach (var file in validAttachments) {
                var destName = $"{uuid}-attachment-{Path.GetFileName(file)}";
                try {
                    File.Copy(file, Path.Combine(resultsDirectory, destName), overwrite: true);
                    attachmentEntries.Add(new { name = ResolveScenarioAttachmentName(file), source = destName, type = GuessMime(file) });
                }
                catch { /* attachment failure must never fail the run */ }
            }

        // Step attachments are copied into the results directory too, so the
        // report references them by their (uuid-prefixed) file name instead of
        // an absolute path that Allure cannot resolve. The uuid keeps names
        // unique across steps/scenarios.
        var stepEntries = new List<object>();
        if (steps is not null)
            foreach (var s in steps) {
                var stepAttachmentEntries = new List<object>();
                if (s.Attachments is not null) {
                    var attachmentIndex = 0;
                    foreach (var a in s.Attachments) {
                        if (string.IsNullOrWhiteSpace(a.Source) || !File.Exists(a.Source)) continue;
                        try {
                            var info = new FileInfo(a.Source);
                            if (info.Length == 0) continue;
                            var destName = $"{uuid}-step{stepEntries.Count:D3}-{attachmentIndex:D2}-{Path.GetFileName(a.Source)}";
                            File.Copy(a.Source, Path.Combine(resultsDirectory, destName), overwrite: true);
                            stepAttachmentEntries.Add(new { a.Name, source = destName, a.Type });
                            attachmentIndex++;
                        }
                        catch { /* attachment failure must never fail the run */ }
                    }
                }

                stepEntries.Add(new {
                    name = s.Name,
                    status = s.Status,
                    start = s.Start,
                    stop = s.Stop,
                    parameters = s.Parameters ?? Array.Empty<object>(),
                    statusDetails = s.StatusDetail ?? new AllureStepDetail(null, null),
                    attachments = stepAttachmentEntries.ToArray(),
                });
            }
        var stepsArray = stepEntries.ToArray();

        var categoriesArray = categories?.Select(c => new { name = c.Name, value = c.Value }).ToArray() ?? Array.Empty<object>();

        // Rich labels + links so Allure renders severity, epic/story, owner,
        // host/thread and issue/tms links — maximum detail for triage.
        var severity = "normal";
        string? owner = null;
        var links = new List<object>();
        foreach (var t in tagList) {
            var lt = t.ToLowerInvariant();
            if (lt is "blocker" or "critical" or "normal" or "minor" or "trivial") severity = lt;
            if (t.StartsWith("owner:", StringComparison.OrdinalIgnoreCase)) owner = t[6..];
            else if (t.StartsWith("issue:", StringComparison.OrdinalIgnoreCase)) links.Add(new { name = t, type = "issue", url = (string?)null });
            else if (t.StartsWith("tms:", StringComparison.OrdinalIgnoreCase)) links.Add(new { name = t, type = "tms", url = (string?)null });
        }
        var labels = new List<object> {
            new { name = "suite", value = feature },
            new { name = "feature", value = feature },
            new { name = "story", value = scenario },
            new { name = "epic", value = feature },
            new { name = "severity", value = severity },
            new { name = "package", value = feature },
            new { name = "host", value = Environment.MachineName },
            new { name = "thread", value = Environment.CurrentManagedThreadId.ToString() },
            new { name = "framework", value = testFramework ?? "reqnroll" },
            new { name = "language", value = "csharp" },
        };
        if (owner is not null) labels.Add(new { name = "owner", value = owner });

        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var result = new {
            uuid,
            historyId = $"{feature}::{scenario}",
            fullName = $"{feature}: {scenario}",
            labels = labels.ToArray(),
            links = links.Count > 0 ? links.ToArray() : Array.Empty<object>(),
            name = scenario,
            status = passed ? "passed" : "failed",
            statusDetails = passed
                ? new { message = (string?)null, trace = (string?)null }
                : new { message = (string?)(errorMessage ?? "Scenario failed"), trace = (string?)(errorTrace ?? errorMessage ?? "Scenario failed") },
            stage = "finished",
            description = $"Feature: {feature}",
            steps = stepsArray,
            attachments = attachmentEntries,
            parameters = Array.Empty<object>(),
            categories = categoriesArray,
            start = now - Math.Max(durationMs, 0),
            stop = now,
            tags = tagList.Select(t => new { name = t }).ToArray(),
        };
        var path = Path.Combine(resultsDirectory, $"{uuid}-result.json");
        File.WriteAllText(path, JsonSerializer.Serialize(result, JsonOptions));
        return path;
    }

    /// <summary>
    /// Writes environment.properties so the Allure overview shows the full run
    /// context (OS, runtime, machine, browser, environment, run id).
    /// </summary>
    public static void WriteEnvironment(string resultsDirectory, string? browser = null, string? environment = null) {
        Directory.CreateDirectory(resultsDirectory);
        File.WriteAllLines(Path.Combine(resultsDirectory, "environment.properties"),
        [
            $"OS={Environment.OSVersion}",
            $"OS.Platform={System.Runtime.InteropServices.RuntimeInformation.OSDescription}",
            $"Architecture={System.Runtime.InteropServices.RuntimeInformation.OSArchitecture}",
            $"Runtime=.NET {Environment.Version}",
            $"Framework=Framework.Automation",
            $"Machine={Environment.MachineName}",
            $"User={Environment.UserName}",
            $"ProcessorCount={Environment.ProcessorCount}",
            $"Browser={browser ?? "Chromium"}",
            $"Environment={environment ?? "Development"}",
            $"RunId={Lifecycle.RunContext.RunId}",
            $"Generated={DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC",
        ]);
    }

    /// <summary>
    /// Writes categories.json so Allure classifies test outcomes into meaningful
    /// defect categories on the overview dashboard.
    /// </summary>
    public static void WriteCategories(string resultsDirectory) {
        try {
            Directory.CreateDirectory(resultsDirectory);
            var categories = new object[] {
                new {
                    name = "Defectos de Producto / Fallos de Validación",
                    matchedStatuses = new[] { "failed" },
                    messageRegex = ".*(expected|assert|should|equal|target|fail).*|.*TargetFailedException.*"
                },
                new {
                    name = "Errores de Infraestructura / Excepciones",
                    matchedStatuses = new[] { "broken" },
                    traceRegex = ".*Timeout.*|.*Exception.*"
                }
            };
            var path = Path.Combine(resultsDirectory, "categories.json");
            File.WriteAllText(path, JsonSerializer.Serialize(categories, JsonOptions));
        }
        catch { /* best-effort */ }
    }

    /// <summary>
    /// Writes executor.json so the Allure Overview renders the execution environment.
    /// </summary>
    public static void WriteExecutor(string resultsDirectory, string? projectName = null) {
        try {
            Directory.CreateDirectory(resultsDirectory);
            var executor = new {
                name = projectName ?? "Enterprise Automation Framework",
                type = "custom",
                buildName = $"Run #{Lifecycle.RunContext.RunId}",
                buildUrl = (string?)null
            };
            var path = Path.Combine(resultsDirectory, "executor.json");
            File.WriteAllText(path, JsonSerializer.Serialize(executor, JsonOptions));
        }
        catch { /* best-effort */ }
    }

    private static string ResolveScenarioAttachmentName(string file) {
        var fileName = Path.GetFileName(file);
        var ext = Path.GetExtension(file).ToLowerInvariant();
        if (ext is ".webm" or ".mp4") return "Video de Ejecución";
        if (ext is ".zip") return "Trace de Playwright";
        if (fileName.Equals("http-log.txt", StringComparison.OrdinalIgnoreCase)) return "Log HTTP de Escenario";
        if (fileName.Contains("failure", StringComparison.OrdinalIgnoreCase)) return "Captura de Fallo (Escenario)";
        return fileName;
    }

    /// <summary>
    /// Copies the previous report's <c>history/*.json</c> into
    /// <c>{results}/history/</c> — the only place Allure reads it from — so
    /// trends and per-test history survive across generations. Best-effort.
    /// </summary>
    public static void CopyHistory(string resultsDirectory, string reportDirectory = "allure-report") {
        try {
            var historyDir = Path.Combine(reportDirectory, "history");
            if (!Directory.Exists(historyDir)) return;
            var target = Path.Combine(resultsDirectory, "history");
            Directory.CreateDirectory(target);
            foreach (var file in Directory.GetFiles(historyDir, "*.json"))
                File.Copy(file, Path.Combine(target, Path.GetFileName(file)), overwrite: true);
        }
        catch { /* history is best-effort */ }
    }

    /// <summary>
    /// Rolls one execution's results into the cumulative folder.
    /// </summary>
    public static int MergeResults(string runDirectory, string cumulativeDirectory) {
        var merged = 0;
        try {
            if (!Directory.Exists(runDirectory)) return 0;
            Directory.CreateDirectory(cumulativeDirectory);
            foreach (var file in Directory.GetFiles(runDirectory)) {
                var name = Path.GetFileName(file);
                if (name.StartsWith(".", StringComparison.Ordinal)) continue;
                try {
                    File.Copy(file, Path.Combine(cumulativeDirectory, name), overwrite: true);
                    if (name.EndsWith("-result.json", StringComparison.OrdinalIgnoreCase)) merged++;
                }
                catch { /* one bad file must not block the rest */ }
            }
        }
        catch { /* merge is best-effort */ }
        return merged;
    }

    private static string GuessMime(string path) =>
        Path.GetExtension(path).ToLowerInvariant() switch {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".zip" => "application/zip",
            ".webm" => "video/webm",
            ".mp4" => "video/mp4",
            _ => "text/plain",
        };

    /// <summary>
    /// Opens an already GENERATED report with <c>allure open {report} --port N</c>
    /// (Allure serves it and launches the browser). The server is detached and
    /// stays up after the test run ends.
    /// </summary>
    public static void OpenReport(string reportDirectory, int port = 56789, string? cliPath = null)
        => StartDetached($"open \"{reportDirectory}\" --port {port}", port, cliPath);

    /// <summary>
    /// Generates the static HTML report: <c>allure generate {results} -o {report} --clean</c>.
    /// Waits for the CLI and returns whether it succeeded.
    /// </summary>
    public static bool GenerateReport(string resultsDirectory, string reportDirectory = "allure-report", string? cliPath = null) {
        Console.WriteLine($"[ALLURE] Generating report from {resultsDirectory}...");
        try {
            var psi = BuildStartInfo(ResolveExecutable(cliPath),
                $"generate \"{resultsDirectory}\" -o \"{reportDirectory}\" --clean");
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            using var process = Process.Start(psi)!;
            var stdout = process.StandardOutput.ReadToEndAsync();
            var stderr = process.StandardError.ReadToEndAsync();
            if (!process.WaitForExit(TimeSpan.FromMinutes(3))) {
                try { process.Kill(entireProcessTree: true); } catch { }
                Console.WriteLine("[ALLURE] 'allure generate' timed out after 3 minutes.");
                return false;
            }

            if (process.ExitCode != 0 || !File.Exists(Path.Combine(reportDirectory, "index.html"))) {
                Console.WriteLine($"[ALLURE] 'allure generate' failed (exit code {process.ExitCode}).");
                Console.WriteLine(stdout.Result);
                Console.WriteLine(stderr.Result);
                return false;
            }

            Console.WriteLine($"[ALLURE] Report generated in {reportDirectory}");
            return true;
        }
        catch (Exception ex) {
            Console.WriteLine($"[ALLURE] Error generating report: {ex.Message}");
            Console.WriteLine("[ALLURE] Make sure Allure CLI (and Java) are installed and in PATH, or set Allure.AllureCliPath.");
            return false;
        }
    }

    /// <summary>
    /// Serves raw RESULTS with <c>allure serve {results} --port N</c>.
    /// </summary>
    public static void ServeReport(string resultsDirectory, int port = 56789, string? cliPath = null)
        => StartDetached($"serve \"{resultsDirectory}\" --port {port}", port, cliPath);

    private static void StartDetached(string arguments, int port, string? cliPath) {
        var url = $"http://localhost:{port}";
        try {
            if (IsListening(port)) {
                Console.WriteLine($"[ALLURE] Port {port} already served; opening {url}");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                return;
            }

            Console.WriteLine($"[ALLURE] allure {arguments}");
            var psi = BuildStartInfo(ResolveExecutable(cliPath), arguments);
            psi.UseShellExecute = true;
            psi.CreateNoWindow = false;
            Process.Start(psi);
            Console.WriteLine($"[ALLURE] Report server left running at {url} (close its window to stop it).");
        }
        catch (Exception ex) {
            Console.WriteLine($"[ALLURE] Error starting report server: {ex.Message}");
            Console.WriteLine("[ALLURE] Make sure Allure CLI (and Java) are installed and in PATH, or set Allure.AllureCliPath.");
        }
    }

    private static string ResolveExecutable(string? cliPath) {
        if (!string.IsNullOrWhiteSpace(cliPath) && File.Exists(cliPath)) return Path.GetFullPath(cliPath);

        var paths = (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries);
        foreach (var dir in paths)
            foreach (var name in AllureExecutableNames) {
                var candidate = Path.Combine(dir.Trim('"'), name);
                if (File.Exists(candidate)) return candidate;
            }
        return "allure";
    }

    private static ProcessStartInfo BuildStartInfo(string executable, string arguments) {
        var ext = Path.GetExtension(executable).ToLowerInvariant();
        return OperatingSystem.IsWindows() && ext is ".cmd" or ".bat"
            ? new ProcessStartInfo("cmd.exe", $"/d /s /c \"\"{executable}\" {arguments}\"")
            : new ProcessStartInfo(executable, arguments);
    }

    private static bool IsListening(int port) {
        try {
            using var client = new System.Net.Sockets.TcpClient();
            return client.ConnectAsync(System.Net.IPAddress.Loopback, port).Wait(300) && client.Connected;
        }
        catch { return false; }
    }

    /// <summary>
    /// Represents a single step of an Allure test.
    /// </summary>
    public sealed record AllureStep(
        string Name,
        string Status,
        long Start,
        long Stop,
        object? Parameters = null,
        AllureStepDetail? StatusDetail = null,
        IEnumerable<AllureAttachmentRef>? Attachments = null);

    /// <summary>
    /// Optional detail attached to a step.
    /// </summary>
    public sealed record AllureStepDetail(string? Message, string? Trace);

    /// <summary>
    /// A name/value parameter rendered inline on a step.
    /// </summary>
    public sealed record AllureParameter(string Name, string Value);

    /// <summary>
    /// A reference to a single attachment (screenshot, trace, video).
    /// </summary>
    public sealed record AllureAttachmentRef(string Name, string Source, string Type);

    /// <summary>
    /// A custom category for the test.
    /// </summary>
    public sealed record AllureCategory(string Name, string Value);
}
