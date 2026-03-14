using Framework.Core.ScreenPlay.Interfaces;

namespace Framework.Core.Screenplay;

/// <summary>
/// Base class for questions providing common functionality.
/// Questions query the state of the system.
/// </summary>
public abstract class Question<TResult> : IQuestion<TResult>
{
    /// <inheritdoc/>
    public abstract string Description { get; }
    
    /// <inheritdoc/>
    public abstract Task<TResult> AnsweredByAsync(IActor actor);
}
