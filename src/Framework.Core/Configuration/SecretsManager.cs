using Microsoft.Extensions.Configuration;

namespace Framework.Core.Configuration;

/// <summary>
/// Global secrets accessor for test automation.
/// Resolves secrets in priority order:
/// <list type="number">
///   <item><description>Custom <see cref="ISecretsProvider"/> (if configured via <see cref="SetProvider"/>).</description></item>
///   <item><description>Environment variables: <c>SECRET_{key}</c>, <c>SECRETS__{key}</c>, <c>{key}</c>.</description></item>
///   <item><description>Configuration section <c>Secrets:{key}</c>.</description></item>
///   <item><description>Direct configuration key <c>{key}</c>.</description></item>
/// </list>
/// </summary>
public static class SecretsManager {
    private static ISecretsProvider? _customProvider;

    /// <summary>
    /// Sets a custom secrets provider (e.g. Azure Key Vault, AWS Secrets Manager).
    /// </summary>
    /// <param name="provider">The custom secrets provider.</param>
    public static void SetProvider(ISecretsProvider provider) => _customProvider = provider;

    /// <summary>
    /// Resets the provider back to the default configuration/environment provider.
    /// </summary>
    public static void ResetProvider() => _customProvider = null;

    /// <summary>
    /// Gets the currently active secrets provider.
    /// </summary>
    public static ISecretsProvider Current => _customProvider ?? DefaultSecretsProvider.Instance;

    /// <summary>
    /// Gets a secret string by key. Returns null if not found.
    /// </summary>
    /// <param name="key">The secret name or key path.</param>
    /// <returns>The secret value, or null if not found.</returns>
    public static string? Get(string key) => Current.GetSecret(key);

    /// <summary>
    /// Gets a required secret. Throws an <see cref="InvalidOperationException"/> if missing.
    /// </summary>
    /// <param name="key">The secret name or key path.</param>
    /// <returns>The secret value.</returns>
    /// <exception cref="InvalidOperationException">The secret was not found.</exception>
    public static string GetRequired(string key)
    {
        var secret = Get(key);
        if (string.IsNullOrWhiteSpace(secret))
        {
            throw new InvalidOperationException(
                $"Required secret '{key}' was not found. " +
                "Configure it via environment variables (SECRET_{key} or {key}), " +
                "appsettings.Secrets.json (gitignored), or via SecretsManager.SetProvider().");
        }
        return secret;
    }

    /// <summary>
    /// Binds a strongly-typed secrets section.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="sectionName">The section name (default: "Secrets").</param>
    /// <returns>The model instance or null.</returns>
    public static T? GetSection<T>(string sectionName = "Secrets") where T : class, new()
        => Current.GetSecretsSection<T>(sectionName);
}

/// <summary>
/// Default secrets provider backed by Microsoft.Extensions.Configuration and environment variables.
/// </summary>
internal sealed class DefaultSecretsProvider : ISecretsProvider {
    public static readonly DefaultSecretsProvider Instance = new();

    private DefaultSecretsProvider() { }

    public string? GetSecret(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return null;

        // 1. Direct environment variables
        var envSecret = Environment.GetEnvironmentVariable($"SECRET_{key}")
            ?? Environment.GetEnvironmentVariable($"SECRETS__{key}")
            ?? Environment.GetEnvironmentVariable(key);

        if (!string.IsNullOrWhiteSpace(envSecret)) return envSecret;

        // 2. Configuration under "Secrets:{key}"
        var fromSecretsSection = SettingsManager.Configuration[$"Secrets:{key}"];
        if (!string.IsNullOrWhiteSpace(fromSecretsSection)) return fromSecretsSection;

        // 3. Fallback to direct key in configuration
        return SettingsManager.Configuration[key];
    }

    public T? GetSecretsSection<T>(string sectionName) where T : class, new()
    {
        var section = SettingsManager.Configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            section = SettingsManager.Configuration.GetSection($"Secrets:{sectionName}");
        }
        return section.Exists() ? section.Get<T>() : null;
    }
}
