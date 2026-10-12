using Framework.Core.ScreenPlay.Interfaces;
using Reqnroll;

namespace Framework.Core.BDD;

/// <summary>
/// Helper for accessing Actors from the Reqnroll ScenarioContext.
/// Provides type-safe, per-scenario actor retrieval for step definitions.
/// No static state: every lookup reads the current scenario's context, so
/// parallel scenarios never share or leak actors.
/// </summary>
public static class ScenarioActor
{
    /// <summary>
    /// Gets the primary actor from the scenario context.
    /// Throws if the actor is not initialized (BeforeScenario hook must run first).
    /// </summary>
    /// <param name="scenarioContext">The Reqnroll scenario context.</param>
    /// <returns>The initialized actor.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the actor is not initialized.</exception>
    public static IActor Get(ScenarioContext scenarioContext)
    {
        if (!scenarioContext.TryGetValue("Actor", out var actorFromContext) || actorFromContext is not IActor actor)
        {
            throw new InvalidOperationException("Actor not initialized. BeforeScenario hook must run first.");
        }
        return actor;
    }

    /// <summary>
    /// Gets an API actor from the scenario context.
    /// Creates if not exists.
    /// </summary>
    /// <param name="scenarioContext">The Reqnroll scenario context.</param>
    /// <param name="actorKey">The key to store/retrieve the actor.</param>
    /// <param name="actorFactory">Factory to create the actor if not exists.</param>
    /// <returns>The initialized actor.</returns>
    public static IActor GetOrCreate(ScenarioContext scenarioContext, string actorKey, Func<IActor> actorFactory)
    {
        if (!scenarioContext.TryGetValue(actorKey, out var actorFromContext))
        {
            var actor = actorFactory();
            scenarioContext[actorKey] = actor;
            return actor;
        }
        return (IActor)actorFromContext;
    }

    /// <summary>
    /// Resolves the actor for the CURRENT step from its subject ("the user",
    /// "they", "an administrator"...). Pronouns reference the same persona;
    /// roles get-or-create their own actor. Personas are per-scenario (stored
    /// in the ScenarioContext), so there is no cross-scenario memory leakage.
    /// Hooks must register a factory via <see cref="RegisterPersonaFactory"/>.
    /// </summary>
    /// <param name="scenarioContext">The Reqnroll scenario context.</param>
    /// <returns>The actor for the step's subject.</returns>
    public static IActor FromStep(ScenarioContext scenarioContext)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);

        var stepText = scenarioContext.StepContext.StepInfo.Text ?? string.Empty;
        var subject = ActorSubjects.TryGetSubject(stepText, out var canonical) ? canonical : "user";
        var personaKey = $"persona:{subject}";
        var isPronoun = ActorSubjects.IsPronoun(subject);

        // Persona already known in this scenario → reuse the SAME actor.
        if (scenarioContext.TryGetValue(personaKey, out var known) && known is IActor existing)
        {
            scenarioContext[LastPersonaKey] = subject;
            return existing;
        }

        // Pronoun without a prior mapping → refer to the currently active persona.
        if (isPronoun &&
            scenarioContext.TryGetValue(LastPersonaKey, out var lastKey) &&
            scenarioContext.TryGetValue($"persona:{lastKey}", out var lastActor) &&
            lastActor is IActor referred)
        {
            scenarioContext[personaKey] = referred;
            return referred;
        }

        // Descriptive subject with an active persona (e.g. "the search results
        // should..." in Then/And) → observed by the current persona, not a new actor.
        // Secondary personas are requested explicitly via Persona(...).
        if (!isPronoun &&
            scenarioContext.TryGetValue(LastPersonaKey, out var activeKey) &&
            scenarioContext.TryGetValue($"persona:{activeKey}", out var activeActor) &&
            activeActor is IActor active)
        {
            scenarioContext[personaKey] = active;
            return active;
        }

        var factory = scenarioContext.TryGetValue(PersonaFactoryKey, out var f) ? f as Func<string, IActor> : null;
        if (factory == null)
        {
            if (scenarioContext.TryGetValue("Actor", out var primary) && primary is IActor p)
            {
                scenarioContext[personaKey] = p;
                scenarioContext[LastPersonaKey] = subject;
                return p;
            }
            throw new InvalidOperationException(
                "No persona factory registered. Register one in a BeforeScenario hook with ScenarioActor.RegisterPersonaFactory(...).");
        }

        var actor = factory(ActorSubjects.DisplayName(subject));
        scenarioContext[personaKey] = actor;
        scenarioContext[LastPersonaKey] = subject;
        return actor;
    }

    /// <summary>
    /// Gets or creates an EXPLICIT persona by name — use it for secondary
    /// personas in multi-actor scenarios (e.g. <c>ScenarioActor.Persona(ctx, "administrator")</c>).
    /// Pronouns ("they", "he"...) keep referencing the active persona via <see cref="FromStep"/>.
    /// For web personas that need an ISOLATED browser session, prefer
    /// <see cref="PersonaAsync"/> (creating a session is asynchronous).
    /// </summary>
    /// <param name="scenarioContext">The Reqnroll scenario context.</param>
    /// <param name="subject">The role name (any language; articles are stripped).</param>
    /// <returns>The actor for that persona.</returns>
    public static IActor Persona(ScenarioContext scenarioContext, string subject)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);
        var canonical = ActorSubjects.Canonicalize(subject);
        var personaKey = $"persona:{canonical}";

        if (scenarioContext.TryGetValue(personaKey, out var known) && known is IActor existing)
        {
            scenarioContext[LastPersonaKey] = canonical;
            return existing;
        }

        var factory = (scenarioContext.TryGetValue(PersonaFactoryKey, out var f) ? f as Func<string, IActor> : null) ?? 
            throw new InvalidOperationException(
                "No persona factory registered. Register one in a BeforeScenario hook with ScenarioActor.RegisterPersonaFactory(...).");
        var actor = factory(ActorSubjects.DisplayName(canonical));
        scenarioContext[personaKey] = actor;
        scenarioContext[LastPersonaKey] = canonical;
        return actor;
    }

    /// <summary>
    /// Async variant of <see cref="Persona"/> for multi-actor scenarios where a
    /// secondary persona needs its OWN isolated session (its own browser
    /// context/cookies), so two actors can act on the same resource through
    /// different sessions (spec §6). Requires an async factory registered via
    /// <see cref="RegisterPersonaFactoryAsync"/>.
    /// </summary>
    /// <param name="scenarioContext">The Reqnroll scenario context.</param>
    /// <param name="subject">The role name (articles are stripped).</param>
    /// <returns>The actor for that persona.</returns>
    public static async Task<IActor> PersonaAsync(ScenarioContext scenarioContext, string subject)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);
        var canonical = ActorSubjects.Canonicalize(subject);
        var personaKey = $"persona:{canonical}";

        if (scenarioContext.TryGetValue(personaKey, out var known) && known is IActor existing)
        {
            scenarioContext[LastPersonaKey] = canonical;
            return existing;
        }

        var factory = (scenarioContext.TryGetValue(PersonaFactoryAsyncKey, out var f)
            ? f as Func<string, Task<IActor>>
            : null) ?? throw new InvalidOperationException(
                "No async persona factory registered. Register one in a BeforeScenario hook with ScenarioActor.RegisterPersonaFactoryAsync(...).");
        var actor = await factory(ActorSubjects.DisplayName(canonical)).ConfigureAwait(false);
        scenarioContext[personaKey] = actor;
        scenarioContext[LastPersonaKey] = canonical;
        return actor;
    }

    /// <summary>
    /// Registers the factory used to create new persona actors (abilities included).
    /// Called once per scenario by the hooks.
    /// </summary>
    /// <param name="scenarioContext">The Reqnroll scenario context.</param>
    /// <param name="factory">Factory that builds an actor for a display name.</param>
    public static void RegisterPersonaFactory(ScenarioContext scenarioContext, Func<string, IActor> factory)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);
        ArgumentNullException.ThrowIfNull(factory);
        scenarioContext[PersonaFactoryKey] = factory;
    }

    /// <summary>
    /// Registers an ASYNC factory for personas that need their own isolated
    /// session (e.g. a fresh browser context per actor). Called once per
    /// scenario by the hooks. Used by <see cref="PersonaAsync"/>.
    /// </summary>
    /// <param name="scenarioContext">The Reqnroll scenario context.</param>
    /// <param name="factory">Async factory that builds an actor for a display name.</param>
    public static void RegisterPersonaFactoryAsync(ScenarioContext scenarioContext, Func<string, Task<IActor>> factory)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);
        ArgumentNullException.ThrowIfNull(factory);
        scenarioContext[PersonaFactoryAsyncKey] = factory;
    }

    /// <summary>
    /// Registers a known persona (e.g. the primary actor created by the hooks)
    /// under its canonical subject so <see cref="FromStep"/> reuses it.
    /// </summary>
    /// <param name="scenarioContext">The Reqnroll scenario context.</param>
    /// <param name="canonicalSubject">Canonical subject (e.g. "user").</param>
    /// <param name="actor">The actor instance for that persona.</param>
    public static void RegisterPersona(ScenarioContext scenarioContext, string canonicalSubject, IActor actor)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);
        ArgumentNullException.ThrowIfNull(actor);
        var canonical = ActorSubjects.Canonicalize(canonicalSubject);
        scenarioContext[$"persona:{canonical}"] = actor;
        scenarioContext[LastPersonaKey] = canonical;
    }

    private const string PersonaFactoryKey = "persona:factory";
    private const string PersonaFactoryAsyncKey = "persona:factory:async";
    private const string LastPersonaKey = "persona:last";
}
