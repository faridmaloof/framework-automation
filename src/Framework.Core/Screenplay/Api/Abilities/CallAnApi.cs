using Framework.Core.ScreenPlay.Interfaces;
using RestSharp;

namespace Framework.Core.ScreenPlay.Api.Abilities;

/// <summary>
/// Ability that grants an actor the capability to call HTTP APIs
/// using RestSharp. The configured <see cref="RestClient"/> is owned
/// by this ability and disposed alongside the actor.
/// </summary>
public sealed class CallAnApi : IAbility, IDisposable
{
    private readonly RestClient _client;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="CallAnApi"/> ability.
    /// </summary>
    /// <param name="baseUrl">Base URL of the target API.</param>
    /// <param name="timeout">Request timeout (default: 30s).</param>
    public CallAnApi(string baseUrl, TimeSpan? timeout = null)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("Base URL is required.", nameof(baseUrl));
        var options = new RestClientOptions(baseUrl)
        {
            ThrowOnAnyError = false,
            Timeout = timeout ?? TimeSpan.FromSeconds(30),
        };
        _client = new RestClient(options);
    }

    /// <inheritdoc/>
    public string DisplayName => "Call an API";

    /// <summary>Gets the configured RestClient.</summary>
    public RestClient Client => _client;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _client.Dispose();
    }
}
