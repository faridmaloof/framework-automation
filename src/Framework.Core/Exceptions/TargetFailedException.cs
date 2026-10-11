using Framework.Core.ScreenPlay.Locators;

namespace Framework.Core.Exceptions;

/// <summary>
/// Thrown when an actor cannot complete an interaction with a UI target
/// (element not found, not visible, not interactive, timeout, ...).
///
/// The message is intentionally <b>human-readable</b>: it references the
/// actor, the intent (verb) and the target's <em>name + location</em>
/// (e.g. <c>Carlos → click 'Botón ingresar' (Login)</c>) instead of the raw
/// selector. The technical details (selector, Playwright timeout) are kept
/// in <see cref="Exception.InnerException"/> for debugging and traces.
/// </summary>
public class TargetFailedException : FrameworkException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TargetFailedException"/> class.
    /// </summary>
    /// <param name="actorName">Name of the actor that attempted the action.</param>
    /// <param name="action">Human-readable verb/intent (click, fill in, read...).</param>
    /// <param name="target">The target that could not be used.</param>
    /// <param name="innerException">The technical exception (timeout, Playwright error...).</param>
    public TargetFailedException(string actorName, string action, ITarget target, Exception innerException)
        : base(BuildMessage(actorName, action, target), innerException)
    {
        ActorName = actorName;
        Action = action;
        TargetName = target.Name;
        Location = target.Location;
        Selector = target.Selector;
    }

    /// <summary>Name of the actor that attempted the action.</summary>
    public string ActorName { get; }

    /// <summary>Human-readable verb/intent of the action.</summary>
    public string Action { get; }

    /// <summary>Human-readable name of the target.</summary>
    public string TargetName { get; }

    /// <summary>Human-readable location (component path + page).</summary>
    public string Location { get; }

    /// <summary>Raw selector — technical detail for debugging only.</summary>
    public string Selector { get; }

    private static string BuildMessage(string actorName, string action, ITarget target)
    {
        var where = string.IsNullOrEmpty(target.Location) ? string.Empty : $" ({target.Location})";
        return $"'{actorName}' could not {action} '{target.Name}'{where}: " +
               "the element was not found or not ready before the timeout.";
    }
}