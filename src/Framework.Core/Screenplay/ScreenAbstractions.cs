using Framework.Core.ScreenPlay.Interfaces;

namespace Framework.Core.ScreenPlay;

// =====================================================================
// ScreenPlay abstractions (single source of truth)
// ---------------------------------------------------------------------
// Naming convention follows the canonical ScreenPlay pattern:
//   - Ability : capability granted to an actor (browser, API client).
//   - Action  : something an actor performs.
//       * Interaction : atomic technical action (click, type, navigate).
//       * Task         : business-level composition of interactions.
//   - Question : a query about system state; pure, returns data.
// =====================================================================

/// <summary>
/// Abstract base for business-level <see cref="ITask"/>s.
/// Tasks compose <see cref="IInteraction"/>s (or other Tasks) to express
/// a meaningful business action an actor can perform.
/// </summary>
public abstract class ScreenTask : ITask {
    /// <inheritdoc/>
    public virtual string TaskName => GetType().Name;

    /// <inheritdoc/>
    public virtual string Description => $"{TaskName}";

    /// <inheritdoc/>
    public abstract Task PerformAsAsync(IActor actor);
}

/// <summary>
/// Abstract base for atomic <see cref="IInteraction"/>s.
/// Interactions represent low-level UI/system actions. They should
/// not contain business logic; compose them into <see cref="ScreenTask"/>s.
/// </summary>
public abstract class ScreenInteraction : IInteraction {
    /// <inheritdoc/>
    public abstract string Description { get; }

    /// <inheritdoc/>
    public abstract Task PerformAsAsync(IActor actor);
}

/// <summary>
/// Abstract base for <see cref="IQuestion{TResult}"/>s.
/// Questions MUST return data, never perform assertions. Validation
/// belongs in dedicated Ensure* tasks or in the step/test code.
/// </summary>
/// <typeparam name="TResult">The kind of value the question produces.</typeparam>
public abstract class ScreenQuestion<TResult> : IQuestion<TResult> {
    /// <inheritdoc/>
    public abstract string Description { get; }

    /// <inheritdoc/>
    public abstract Task<TResult> AnsweredByAsync(IActor actor);
}

/// <summary>
/// Fluent factory for ad-hoc / inline tasks so developers can execute custom
/// logic without having to declare a dedicated class when not needed.
/// </summary>
public static class Perform
{
    /// <summary>Creates an inline Task that has access to the Actor.</summary>
    public static ITask Task(string description, Func<IActor, Task> action)
        => new DelegateTask(description, action);

    /// <summary>Creates an inline Task that executes an asynchronous action.</summary>
    public static ITask Action(string description, Func<Task> action)
        => new DelegateTask(description, _ => action());
}

internal sealed class DelegateTask(string description, Func<IActor, Task> action) : ScreenTask
{
    public override string TaskName => description;
    public override string Description => description;
    public override Task PerformAsAsync(IActor actor) => action(actor);
}
