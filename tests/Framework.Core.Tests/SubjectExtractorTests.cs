using FluentAssertions;
using Framework.Core.BDD;
using Framework.Core.BDD.Extraction;
using Xunit;

namespace Framework.Core.Tests;

// Shares the "ActorSubjects" xUnit collection with ActorSubjectsTests so tests
// that swap the global extractor never run concurrently with subject lookups.
[Collection("ActorSubjects")]
public class SubjectExtractorTests
{
    [Fact]
    public void DefaultExtractor_ResolvesRole_WithConjugatedVerb()
    {
        // Verb not in any blacklist ("opens"/"records"/"submits") must not be
        // captured as part of the subject (§5.3: no rigid verb list).
        ActorSubjects.TryGetSubject("an administrator opens the order", out var s1).Should().BeTrue();
        s1.Should().Be("administrator");

        ActorSubjects.TryGetSubject("the system records the event", out var s2).Should().BeTrue();
        s2.Should().Be("system");
    }

    [Fact]
    public void DefaultExtractor_BoundsNounPhraseAtAuxiliary()
    {
        // "should" is an auxiliary → subject ends at "search results count".
        ActorSubjects.TryGetSubject("the search results count should be greater than 5", out var s).Should().BeTrue();
        s.Should().Be("search results count");
    }

    [Fact]
    public void DefaultExtractor_ReturnsNull_WhenNoLeadingSubject()
        => ActorSubjects.TryGetSubject("is displayed", out _).Should().BeFalse();

    [Fact]
    public void SetExtractor_ReplacesStrategy_AndNullRestoresDefault()
    {
        var original = ActorSubjects.CurrentExtractor;
        try
        {
            ActorSubjects.SetExtractor(new FixedExtractor("custom-actor"));
            ActorSubjects.TryGetSubject("literally anything", out var custom).Should().BeTrue();
            custom.Should().Be("custom-actor");

            ActorSubjects.SetExtractor(null);
            ActorSubjects.TryGetSubject("the user is here", out var restored).Should().BeTrue();
            restored.Should().Be("user");
        }
        finally
        {
            ActorSubjects.SetExtractor(original);
        }
    }

    private sealed class FixedExtractor(string subject) : ISubjectExtractor
    {
        public string? Extract(string? stepText) => subject;
    }
}
