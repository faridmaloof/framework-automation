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
}
