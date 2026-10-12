namespace Framework.Core.BDD.Extraction;

/// <summary>
/// Strategy that extracts the leading "subject" of a Gherkin step — the actor
/// the step is about — from the raw step text (e.g. <c>"the user"</c>,
/// <c>"an administrator"</c>, <c>"they"</c>, <c>"the system"</c>).
/// <para>
/// Extraction and identity resolution are deliberately separate concerns
/// (spec §5.4): this returns a raw subject; <see cref="ActorSubjects"/> is the
/// component that canonicalizes it and resolves it to a persona. A project can
/// replace the default strategy via <c>ActorSubjects.SetExtractor(...)</c>
/// without touching the framework.
/// </para>
/// </summary>
public interface ISubjectExtractor
{
    /// <summary>
    /// Extracts the raw subject at the start of the step, or <c>null</c> when no
    /// subject can be identified. The result is NOT canonicalized — leading
    /// articles may be stripped, but pronoun/alias mapping is applied later by
    /// <see cref="ActorSubjects"/>.
    /// </summary>
    /// <param name="stepText">The full step text from Reqnroll.</param>
    /// <returns>The raw subject, or <c>null</c> if none is found.</returns>
    string? Extract(string? stepText);
}
