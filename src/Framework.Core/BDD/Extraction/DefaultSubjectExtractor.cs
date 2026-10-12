using System.Text.RegularExpressions;

namespace Framework.Core.BDD.Extraction;

/// <summary>
/// Default <see cref="ISubjectExtractor"/>. It does NOT rely on a rigid list of
/// business verbs (spec §5.3). Instead it resolves the subject in three
/// deterministic passes, most-specific first:
/// <list type="number">
///   <item><description>A known pronoun/alias (single token) → returned as-is.</description></item>
///   <item><description>A known role subject registered via <c>ActorSubjects.Register</c>
///   — matched as a whole multi-word phrase with word boundaries, longest first.</description></item>
///   <item><description>A noun phrase: an optional article followed by role words, bounded
///   by auxiliary/modal words (a small, language-level closed set) — NOT by a
///   list of every possible business verb.</description></item>
/// </list>
/// This resolves the spec's examples ("the system records...", "an administrator
/// opens...", "the customer submits...") without a magic token cap or a verb
/// blacklist. Extend the known-subject set per project via <c>Register</c>.
/// </summary>
/// <remarks>Initializes a new instance of the <see cref="DefaultSubjectExtractor"/> class.</remarks>
/// <param name="knownSubjectsProvider">
/// Returns the currently-known subject keys (canonical, lower-case). The
/// extractor is stateless w.r.t. them and re-reads on each call, so runtime
/// registrations are honoured safely.
/// </param>
public sealed partial class DefaultSubjectExtractor(Func<IEnumerable<string>> knownSubjectsProvider) : ISubjectExtractor
{
    // Auxiliary / modal words. This is a small, language-level CLOSED set that
    // marks the end of a noun phrase in English Gherkin ("is", "should",
    // "will"...). It is intentionally NOT an open-ended list of business verbs.
    private static readonly HashSet<string> Auxiliaries = new(StringComparer.OrdinalIgnoreCase)
    {
        "is", "are", "was", "were", "am", "be", "been", "being",
        "has", "have", "had", "do", "does", "did",
        "can", "could", "should", "must", "will", "would", "may", "might", "shall",
    };

    /// <summary>
    /// A regex pattern that matches a leading article ("a", "an", or "the").
    /// </summary>
    /// <returns>A regex pattern that matches a leading article.</returns>
    [GeneratedRegex(@"^(a|an|the)\s+", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex ArticlePatternRegex();

    /// <summary>
    /// A regex pattern that matches one or more whitespace characters.
    /// </summary>
    [GeneratedRegex(@"\s+", RegexOptions.None, "en-US")]
    private static partial Regex WhitespacePatternRegex();

    /// <inheritdoc/>
    public string? Extract(string? stepText)
    {
        if (string.IsNullOrWhiteSpace(stepText)) return null;

        var tokens = Tokenize(stepText);
        if (tokens.Count == 0) return null;

        // Pass 1: a single-token pronoun/alias (e.g. "they", "i").
        if (IsKnown(tokens[0]))
            return tokens[0];

        // Pass 2: a known multi-word role phrase with word boundaries, longest
        // first (e.g. registered "the qa" → "qa", "search manager" → "search manager").
        var known = knownSubjectsProvider().ToList();
        for (var len = tokens.Count; len >= 2; len--)
        {
            var phrase = string.Join(' ', tokens.Take(len));
            var canonical = StripArticle(phrase);
            if (known.Contains(canonical))
                return canonical;
        }

        // Pass 3: noun phrase = optional article + role words, bounded by the
        // first auxiliary/modal word. No arbitrary token cap.
        var collected = new List<string>();
        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (IsArticle(token))
            {
                // Only a leading article is allowed to start the phrase.
                if (collected.Count == 0) continue;
                break;
            }
            if (collected.Count > 0 && Auxiliaries.Contains(token)) break;
            if (Auxiliaries.Contains(token) && collected.Count == 0)
                // A step that starts with an auxiliary has no leading subject.
                return null;
            collected.Add(token);
        }

        return collected.Count == 0 ? null : string.Join(' ', collected);
    }

    /// <summary>
    /// Tokenizes the step text into words, trimming punctuation and whitespace. This is used to identify the leading subject of a Gherkin step.
    /// </summary>
    /// <param name="stepText">The step text to tokenize.</param>
    /// <returns>A list of tokens.</returns>
    private static List<string> Tokenize(string stepText)
        => [.. stepText.Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(t => t.Trim(',', '.', ':', ';', '"', '\'', '?', '!'))
            .Where(t => t.Length > 0)];

    /// <summary>
    /// Returns true when the token is a known pronoun/alias/role. This is used to
    /// resolve the first token of a step to a subject.
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <returns><c>true</c> if the token is a known pronoun/alias/role; otherwise, <c>false</c>.</returns>
    private bool IsKnown(string token)
    {
        var canonical = StripArticle(token);
        return knownSubjectsProvider().Contains(canonical);
    }

    /// <summary>
    /// Returns true when the token is a leading article ("a", "an", "the").
    /// </summary>
    /// <param name="token">The token to check.</param>
    /// <returns><c>true</c> if the token is a leading article; otherwise, <c>false</c>.</returns>
    private static bool IsArticle(string token)
        => token.Equals("a", StringComparison.OrdinalIgnoreCase)
        || token.Equals("an", StringComparison.OrdinalIgnoreCase)
        || token.Equals("the", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Strips a leading article and collapses whitespace. This is used to normalize
    /// the subject of a step.
    /// </summary>
    /// <param name="phrase">The phrase to normalize.</param>
    /// <returns>The normalized phrase.</returns>
    private static string StripArticle(string phrase)
    {
        var cleaned = ArticlePatternRegex().Replace(phrase.Trim().ToLowerInvariant(), string.Empty);
        return WhitespacePatternRegex().Replace(cleaned, " ").Trim();
    }
}
