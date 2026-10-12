namespace Framework.Core.Configuration;

/// <summary>
/// Contract for retrieving secrets and credentials (API keys, passwords, connection strings, tokens).
/// Allows plugging external secret stores (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault)
/// into test suites without modifying Core.
/// </summary>
public interface ISecretsProvider {
    /// <summary>
    /// Retrieves a secret value by key or path (e.g. "ApiKey" or "Database:Password").
    /// </summary>
    /// <param name="key">The secret name or path.</param>
    /// <returns>The secret string, or null if not found.</returns>
    string? GetSecret(string key);

    /// <summary>
    /// Binds a strongly-typed section of secrets.
    /// </summary>
    /// <typeparam name="T">The type of the secrets model.</typeparam>
    /// <param name="sectionName">Section name (e.g. "PaymentGateway", "Auth").</param>
    /// <returns>The populated model, or null if not present.</returns>
    T? GetSecretsSection<T>(string sectionName) where T : class, new();
}
