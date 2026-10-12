using Microsoft.Extensions.Configuration;

namespace Framework.Core.Configuration;

/// <summary>
/// Manages framework configuration loading. Configuration ALWAYS comes from
/// Core: the test project only ships appsettings*.json files; nothing in the
/// hooks or steps hard-codes browser/evidence/reporting behaviour.
/// Environment resolution order: FRAMEWORK_ENVIRONMENT → DOTNET_ENVIRONMENT →
/// ASPNETCORE_ENVIRONMENT → "Development".
/// </summary>
public static class SettingsManager
{
    private static IConfigurationRoot? _configuration;
    private static FrameworkSettings? _frameworkSettings;
    private static DeviceSettings? _deviceSettings;
    private static AllureSettings? _allureSettings;
    private static ApiSettings? _apiSettings;

    /// <summary>
    /// Gets the configuration root. Files are read from the test assembly
    /// directory (AppContext.BaseDirectory), where appsettings*.json are
    /// copied at build time.
    /// </summary>
    public static IConfigurationRoot Configuration
    {
        get
        {
            _configuration ??= BuildConfiguration(AppContext.BaseDirectory);
            return _configuration;
        }
    }

    private static string ResolveEnvironment() =>
        Environment.GetEnvironmentVariable("FRAMEWORK_ENVIRONMENT")
        ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
        ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        ?? "Development";

    private static IConfigurationRoot BuildConfiguration(string basePath) =>
        new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{ResolveEnvironment()}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(prefix: "FRAMEWORK_")
            .Build();

    /// <summary>
    /// Gets the framework settings.
    /// </summary>
    public static FrameworkSettings FrameworkSettings
    {
        get
        {
            _frameworkSettings ??= Configuration.GetSection("Framework").Get<FrameworkSettings>() ?? new FrameworkSettings();
            return _frameworkSettings;
        }
    }

    /// <summary>
    /// Gets the device settings.
    /// </summary>
    public static DeviceSettings DeviceSettings
    {
        get
        {
            _deviceSettings ??= Configuration.GetSection("Device").Get<DeviceSettings>() ?? new DeviceSettings();
            return _deviceSettings;
        }
    }

    /// <summary>
    /// Gets the Allure settings.
    /// </summary>
    public static AllureSettings AllureSettings
    {
        get
        {
            _allureSettings ??= Configuration.GetSection("Allure").Get<AllureSettings>() ?? new AllureSettings();
            return _allureSettings;
        }
    }

    /// <summary>
    /// Gets the API settings (base URL and timeout) for the target service.
    /// </summary>
    public static ApiSettings ApiSettings
    {
        get
        {
            _apiSettings ??= Configuration.GetSection("Api").Get<ApiSettings>() ?? new ApiSettings();
            return _apiSettings;
        }
    }

    /// <summary>
    /// Loads configuration from the specified path and resets cached settings.
    /// </summary>
    /// <param name="basePath">The base path for configuration files.</param>
    public static void LoadFrom(string basePath)
    {
        _configuration = BuildConfiguration(basePath);
        _frameworkSettings = null;
        _deviceSettings = null;
        _allureSettings = null;
        _apiSettings = null;
    }
}
