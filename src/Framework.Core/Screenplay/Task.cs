using Framework.Core.ScreenPlay.Interfaces;

namespace Framework.Core.Screenplay;

/// <summary>
/// Base class for tasks providing common functionality.
/// Tasks represent business-level actions that an actor can perform.
/// </summary>
public abstract class Task : ITask
{
    /// <inheritdoc/>
    public virtual string TaskName => GetType().Name;

    /// <inheritdoc/>
    public virtual string Description => $"Perform: {TaskName}";

    /// <inheritdoc/>
    public abstract System.Threading.Tasks.Task PerformAsAsync(IActor actor);
}
