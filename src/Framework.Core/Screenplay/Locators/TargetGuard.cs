using Framework.Core.Exceptions;
using Framework.Core.ScreenPlay.Interfaces;
using Microsoft.Playwright;

namespace Framework.Core.ScreenPlay.Locators;

/// <summary>
/// Executes engine operations over a target and converts raw engine
/// timeouts into <see cref="TargetFailedException"/> — a human-readable
/// failure that references the actor, the intent and the target's name +
/// location (e.g. <c>'Carlos' could not fill in 'Usuario' (Login)</c>)
/// instead of the raw selector.
///
/// All web Interactions and Questions must route their Playwright calls
/// through this guard so every failure in the report reads like a sentence
/// a person can act on. The original engine error is preserved as the
/// inner exception (traces, Allure and logs still get the technical detail).
/// </summary>
public static class TargetGuard
{
    /// <summary>
    /// Performs an operation over the resolved target on behalf of the actor.
    /// </summary>
    /// <param name="actor">The actor performing the action.</param>
    /// <param name="target">The target being acted upon.</param>
    /// <param name="action">Human-readable verb (click, fill in, press...).</param>
    /// <param name="operation">The engine operation over the resolved locator.</param>
    /// <exception cref="TargetFailedException">
    /// The element was not found / not ready before the timeout.
    /// </exception>
    public static async Task PerformAsync(
        IActor actor,
        ITarget target,
        string action,
        Func<ILocator, Task> operation)
    {
        ArgumentNullException.ThrowIfNull(actor);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(operation);

        try
        {
            var page = actor.AbilityTo<BrowseTheWeb>().Page;
            await operation(target.Resolve(page));
        }
        catch (TimeoutException ex)
        {
            throw new TargetFailedException(actor.Name, action, target, ex);
        }
    }

    /// <summary>
    /// Answers a query over the resolved target on behalf of the actor.
    /// </summary>
    /// <typeparam name="TResult">The value produced by the query.</typeparam>
    /// <param name="actor">The actor asking the question.</param>
    /// <param name="target">The target being read.</param>
    /// <param name="action">Human-readable verb (read, count...).</param>
    /// <param name="query">The engine query over the resolved locator.</param>
    /// <returns>The query result.</returns>
    /// <exception cref="TargetFailedException">
    /// The element was not found / not ready before the timeout.
    /// </exception>
    public static async Task<TResult> AnswerAsync<TResult>(
        IActor actor,
        ITarget target,
        string action,
        Func<ILocator, Task<TResult>> query)
    {
        ArgumentNullException.ThrowIfNull(actor);
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(query);

        try
        {
            var page = actor.AbilityTo<BrowseTheWeb>().Page;
            return await query(target.Resolve(page));
        }
        catch (TimeoutException ex)
        {
            throw new TargetFailedException(actor.Name, action, target, ex);
        }
    }
}