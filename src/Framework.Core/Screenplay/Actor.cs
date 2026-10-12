using System.Collections.Concurrent;
using Framework.Core.Exceptions;
using Framework.Core.ScreenPlay.Interfaces;

namespace Framework.Core.ScreenPlay;

/// <summary>
/// Represents an actor in the ScreenPlay Pattern.
/// The actor is the central entity that performs tasks and asks questions.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Actor"/> class.
/// </remarks>
/// <param name="name">The actor's name (e.g., "John", "Admin User", "Customer").</param>
public class Actor(string name) : IActor, IDisposable {
    private readonly string _name = name ?? throw new ArgumentNullException(nameof(name));
    private readonly Guid _correlationId = Guid.NewGuid();
    private readonly ConcurrentDictionary<Type, IAbility> _abilities = new();
    private readonly ConcurrentDictionary<string, object?> _memory = new();
    private bool _disposed;


    /// <inheritdoc/>
    public string Name => _name;

    /// <inheritdoc/>
    public Guid CorrelationId => _correlationId;

    /// <inheritdoc/>
    public void WhoCan<TAbility>(TAbility ability) where TAbility : IAbility {
        ThrowIfDisposed();
        if (ability == null) throw new ArgumentNullException(nameof(ability));
        _abilities.AddOrUpdate(typeof(TAbility), ability, (_, _) => ability);
    }

    /// <summary>
    /// Grants an ability using its <b>runtime</b> type as the lookup key.
    /// <para>
    /// Unlike the generic <c>WhoCan&lt;TAbility&gt;</c> overload (which keys on
    /// the statically inferred ability type), this registers the ability under its
    /// concrete runtime type. Callers that only hold the <see cref="IAbility"/>
    /// interface — such as <see cref="ActorBuilder"/> iterating a
    /// <c>List&lt;IAbility&gt;</c> — therefore produce actors whose abilities
    /// still resolve via <see cref="AbilityTo{TAbility}"/> with the concrete
    /// ability type (e.g. <c>CallAnApi</c>, <c>BrowseTheWeb</c>).
    /// </para>
    /// </summary>
    /// <param name="ability">The ability instance (its runtime type is the key).</param>
    public void WhoCan(IAbility ability) {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(ability);
        _abilities.AddOrUpdate(ability.GetType(), ability, (_, _) => ability);
    }

    /// <inheritdoc/>
    public TAbility AbilityTo<TAbility>() where TAbility : IAbility {
        ThrowIfDisposed();
        if (_abilities.TryGetValue(typeof(TAbility), out var ability))
            return (TAbility)ability;
        throw new AbilityNotFoundException(typeof(TAbility).Name, _name);
    }

    /// <inheritdoc/>
    public bool Can<TAbility>() where TAbility : IAbility {
        ThrowIfDisposed();
        return _abilities.ContainsKey(typeof(TAbility));
    }

    /// <inheritdoc/>
    public Task AttemptsToAsync(params IAction[] actions) {
        ThrowIfDisposed();
        if (actions == null || actions.Length == 0) return Task.CompletedTask;

        async Task RunActionsAsync() {
            foreach (var action in actions) {
                try {
                    await action.PerformAsAsync(this);
                }
                catch (TargetFailedException) {
                    // Already human-readable (actor + target name + location): pass it through.
                    throw;
                }
                catch (TimeoutException ex) {
                    // Raw engine timeout without a target: add actor + intent context.
                    throw new FrameworkException(
                        $"'{_name}' could not complete '{action.Description}' before the timeout.", ex);
                }
                // Any other exception (assertions, programming errors) propagates untouched.
            }
        }

        return RunActionsAsync();
    }

    /// <inheritdoc/>
    public async Task<TResult> AsksForAsync<TResult>(IQuestion<TResult> question) {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(question);
        try {
            return await question.AnsweredByAsync(this);
        }
        catch (TargetFailedException) {
            // Already human-readable: pass it through.
            throw;
        }
        catch (TimeoutException ex) {
            throw new FrameworkException(
                $"'{_name}' could not answer '{question.Description}' before the timeout.", ex);
        }
    }

    /// <inheritdoc/>
    public void Remember(string key, object? value) {
        ThrowIfDisposed();
        _memory.AddOrUpdate(key, value, (_, _) => value);
    }

    /// <inheritdoc/>
    public T? Remembered<T>(string key) {
        ThrowIfDisposed();
        return _memory.TryGetValue(key, out var value) ? (T?)value : default;
    }

    /// <inheritdoc/>
    public bool HasRemembered(string key) {
        ThrowIfDisposed();
        return _memory.ContainsKey(key);
    }

    private void ThrowIfDisposed()
        => ObjectDisposedException.ThrowIf(_disposed, this);

    /// <inheritdoc/>
    public void Dispose() {
        if (_disposed) return;
        _disposed = true;
        foreach (var ability in _abilities.Values)
            if (ability is IDisposable d) d.Dispose();
        _abilities.Clear();
        _memory.Clear();
        GC.SuppressFinalize(this);
    }
}
