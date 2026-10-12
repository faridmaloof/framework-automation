using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Framework.Core.BDD.Extraction;

namespace Framework.Core.BDD;

/// <summary>
/// Registry + normalizer + canonicalizer for Gherkin step "subjects" (spec
/// §5.4). Its single responsibility is identity: it registers roles/aliases,
/// canonicalizes text and maps pronouns to a canonical persona.
/// <para>
/// The actual extraction of the subject from raw step text is delegated to a
/// pluggable <see cref="ISubjectExtractor"/> (default:
/// <see cref="DefaultSubjectExtractor"/>), keeping extraction and identity
/// resolution as separate, testable concerns. Replace the strategy per project
/// with <see cref="SetExtractor"/> without touching the framework.
/// </para>
/// </summary>
public static partial class ActorSubjects {
    // Canonical subject/alias → canonical persona. Concurrent so per-step reads
    // never take a global lock (§5.6). Seeded with base pronouns AND base roles
    // (§5.1: the core provides useful initial aliases; roles are subjects, not
    // a rigid verb list, and are extensible via Register).
    private static readonly ConcurrentDictionary<string, string> Subjects = new(StringComparer.OrdinalIgnoreCase)
    {
        // Pronouns.
        ["i"] = "i", ["me"] = "i", ["my"] = "i", ["mine"] = "i", ["myself"] = "i",
        ["you"] = "you", ["your"] = "you", ["yourself"] = "you",
        ["he"] = "he", ["him"] = "he", ["his"] = "he", ["himself"] = "he",
        ["she"] = "she", ["her"] = "she", ["hers"] = "she", ["herself"] = "she",
        ["it"] = "it", ["its"] = "it", ["itself"] = "it",
        ["we"] = "we", ["us"] = "we", ["our"] = "we", ["ours"] = "we", ["ourselves"] = "we",
        ["they"] = "they", ["them"] = "they", ["their"] = "they", ["theirs"] = "they", ["themselves"] = "they",
        // Base roles (canonical → themselves). Each is its OWN identity: we do
        // NOT auto-merge distinct nominal actors (§5.5). Synonyms like
        // admin→administrator must be registered explicitly by the project.
        ["user"] = "user",
        ["administrator"] = "administrator",
        ["admin"] = "admin",
        ["customer"] = "customer",
        ["system"] = "system",
        ["api client"] = "api client",
        ["guest"] = "guest",
        ["operator"] = "operator",
        ["manager"] = "manager",
        ["tester"] = "tester",
        ["qa"] = "qa",
    };

    [GeneratedRegex(@"^(a|an|the)\s+", RegexOptions.IgnoreCase, "en-US")]
    private static partial Regex ArticlePatternRegex();

    [GeneratedRegex(@"\s+", RegexOptions.None, "en-US")]
    private static partial Regex WhitespacePatternRegex();

    // Pluggable extraction strategy. Volatile so a runtime SetExtractor swap is
    // immediately visible to concurrent readers.
    private static volatile ISubjectExtractor _extractor = new DefaultSubjectExtractor(() => Subjects.Keys);

    /// <summary>
    /// Gets the subject-extraction strategy currently in effect. Useful for
    /// diagnostics and for tests that need to save/restore the strategy.
    /// </summary>
    public static ISubjectExtractor CurrentExtractor => _extractor;

    /// <summary>
    /// Replaces the subject-extraction strategy for the whole process. Use it to
    /// plug a language- or project-specific extractor (spec §5.4). Pass
    /// <c>null</c> to restore the <see cref="DefaultSubjectExtractor"/>.
    /// </summary>
    /// <param name="extractor">The new strategy, or <c>null</c> for the default.</param>
    public static void SetExtractor(ISubjectExtractor? extractor)
        => _extractor = extractor ?? new DefaultSubjectExtractor(() => Subjects.Keys);

    /// <summary>
    /// Registers an additional role/alias and the canonical persona it maps to.
    /// Extensible per project: <c>ActorSubjects.Register("the qa", "user");</c>
    /// </summary>
    /// <param name="alias">The role or alias to map (e.g. "the qa").</param>
    /// <param name="canonical">The canonical persona it refers to (e.g. "user").</param>
    public static void Register(string alias, string canonical)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(alias);
        ArgumentException.ThrowIfNullOrWhiteSpace(canonical);
        Subjects[Normalize(alias)] = Normalize(canonical);
    }

    /// <summary>Returns true when the word is a known pronoun/alias/role.</summary>
    public static bool IsPronoun(string word)
        => Subjects.ContainsKey(Normalize(word));

    /// <summary>
    /// Extracts the subject of a step via the configured
    /// <see cref="ISubjectExtractor"/> ("the user is on..." → "user",
    /// "they search..." → "they") and returns its canonical form.
    /// </summary>
    /// <param name="stepText">The full step text from Reqnroll.</param>
    /// <param name="canonical">The canonical subject when found.</param>
    /// <returns>True when a subject could be extracted.</returns>
    public static bool TryGetSubject(string stepText, out string canonical)
    {
        canonical = string.Empty;
        var raw = _extractor.Extract(stepText);
        if (string.IsNullOrWhiteSpace(raw)) return false;

        canonical = Canonicalize(raw);
        return canonical.Length > 0;
    }

    /// <summary>
    /// Canonicalizes a subject: strips leading articles, collapses spaces and
    /// maps pronouns/aliases to their canonical form ("the user" → "user",
    /// "him" → "he").
    /// </summary>
    public static string Canonicalize(string subject)
    {
        var cleaned = Normalize(subject);
        return Subjects.TryGetValue(cleaned, out var canonical) ? canonical : cleaned;
    }

    /// <summary>
    /// Human display name for a canonical subject ("user" → "User", "i" → "I").
    /// </summary>
    public static string DisplayName(string canonical)
        => canonical == "i" ? "I"
         : char.ToUpperInvariant(canonical[0]) + canonical[1..];

    /// <summary>
    /// Strips a leading article and collapses whitespace. This is used to normalize
    /// the subject of a step.
    /// </summary>
    /// <param name="value">The value to normalize.</param>
    /// <returns>The normalized value.</returns>
    private static string Normalize(string value)
    {
        var cleaned = ArticlePatternRegex().Replace(value.Trim().ToLowerInvariant(), string.Empty);
        return WhitespacePatternRegex().Replace(cleaned, " ").Trim();
    }
}
