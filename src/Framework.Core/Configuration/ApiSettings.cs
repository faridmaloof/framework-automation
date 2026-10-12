namespace Framework.Core.Configuration;

/// <summary>
/// API client settings interface (section "Api").
/// </summary>
public interface IApiSettings
{
    /// <summary>Gets the API base URL for the target service.</summary>
    string BaseUrl { get; }

    /// <summary>Gets the request timeout in seconds.</summary>
    int TimeoutSeconds { get; }
}

/// <summary>
/// API client configuration: base URL and request timeout.
/// Bound from the "Api" section of appsettings so test projects never
/// hard-code endpoints (single source of truth lives in Core).
/// </summary>
public class ApiSettings : IApiSettings
{
    /// <summary>Gets or sets the API base URL. Default: empty (must be configured).</summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>Gets or sets the request timeout in seconds. Default: 30.</summary>
    public int TimeoutSeconds { get; set; } = 30;
}
