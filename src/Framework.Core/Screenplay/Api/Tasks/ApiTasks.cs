using Framework.Core.ScreenPlay.Api.Abilities;
using Framework.Core.ScreenPlay.Interfaces;
using RestSharp;

namespace Framework.Core.ScreenPlay.Api.Tasks;

/// <summary>
/// Memory keys used by API tasks to communicate with API questions.
/// Strongly-typed and centralised so callers never hard-code magic strings.
/// </summary>
public static class ApiMemoryKeys
{
    /// <summary>Last HTTP status code (int).</summary>
    public const string LastStatusCode = "api.last.status";

    /// <summary>Last response body (string).</summary>
    public const string LastBody = "api.last.body";

    /// <summary>Last response headers (IDictionary&lt;string,string&gt;).</summary>
    public const string LastHeaders = "api.last.headers";
}

/// <summary>
/// Sends an HTTP GET to the configured endpoint and stores the response
/// in the actor's memory for later inspection by API questions.
/// </summary>
public sealed class GetResource : ScreenTask
{
    private readonly string _endpoint;

    private GetResource(string endpoint) => _endpoint = endpoint;

    /// <inheritdoc/>
    public override string TaskName => $"GET {_endpoint}";

    /// <inheritdoc/>
    public override string Description => $"Send GET request to '{_endpoint}'";

    /// <summary>Static factory.</summary>
    public static GetResource From(string endpoint) => new(endpoint);

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        var api = actor.AbilityTo<CallAnApi>();
        var request = new RestRequest(_endpoint, Method.Get);
        var response = await api.Client.ExecuteAsync(request);

        actor.Remember(ApiMemoryKeys.LastStatusCode, (int)response.StatusCode);
        actor.Remember(ApiMemoryKeys.LastBody, response.Content ?? string.Empty);
        actor.Remember(ApiMemoryKeys.LastHeaders,
            response.Headers?
                .GroupBy(h => h.Name ?? string.Empty)
                .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(h => h.Value ?? string.Empty)))
                ?? new Dictionary<string, string>());
    }
}

/// <summary>
/// Sends an HTTP POST to the configured endpoint with a JSON body.
/// Stores the response in the actor's memory.
/// </summary>
public sealed class PostResource : ScreenTask
{
    private readonly string _endpoint;
    private readonly string _jsonBody;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostResource"/> class.
    /// </summary>
    /// <param name="endpoint">The endpoint to send the POST request to.</param>
    /// <param name="jsonBody">The JSON body of the request.</param>
    private PostResource(string endpoint, string jsonBody)
    {
        _endpoint = endpoint;
        _jsonBody = jsonBody;
    }

    /// <inheritdoc/>
    public override string TaskName => $"POST {_endpoint}";

    /// <inheritdoc/>
    public override string Description => $"Send POST request to '{_endpoint}'";

    /// <summary>Static factory.</summary>
    public static PostResource To(string endpoint, string jsonBody) => new(endpoint, jsonBody);

    /// <inheritdoc/>
    public override async Task PerformAsAsync(IActor actor)
    {
        var api = actor.AbilityTo<CallAnApi>();
        var request = new RestRequest(_endpoint, Method.Post)
            .AddStringBody(_jsonBody, DataFormat.Json);
        var response = await api.Client.ExecuteAsync(request);

        actor.Remember(ApiMemoryKeys.LastStatusCode, (int)response.StatusCode);
        actor.Remember(ApiMemoryKeys.LastBody, response.Content ?? string.Empty);
    }
}
