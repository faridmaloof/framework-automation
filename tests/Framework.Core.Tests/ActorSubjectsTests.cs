using FluentAssertions;
using Framework.Core.BDD;
using Xunit;

namespace Framework.Core.Tests;

// ActorSubjects keeps process-wide state (the subject registry and the active
// extractor). Tests that mutate that state MUST run serialized against each
// other, so both ActorSubjects test classes share one xUnit collection
// (spec §5.6: define behaviour under simultaneous operations).
[Collection("ActorSubjects")]
public class ActorSubjectsTests {
    [Theory]
    [InlineData("the user is on the Google home page", "user")]
    [InlineData("an administrator is authenticated", "administrator")]
    [InlineData("the customer submits an order", "customer")]
    [InlineData("the system records the event", "system")]
    [InlineData("an administrator opens the order", "administrator")]
    public void TryGetSubject_ExtractsLeadingRole(string stepText, string expected)
    {
        ActorSubjects.TryGetSubject(stepText, out var canonical).Should().BeTrue();
        canonical.Should().Be(expected);
    }

    [Theory]
    [InlineData("they search for something", "they")]
    [InlineData("I request all posts", "i")]
    [InlineData("we validate the flow", "we")]
    public void TryGetSubject_MapsPronounToCanonical(string stepText, string expected)
    {
        ActorSubjects.TryGetSubject(stepText, out var canonical).Should().BeTrue();
        canonical.Should().Be(expected);
    }

    [Theory]
    // §5.5: case-insensitive canonicalization.
    [InlineData("THE USER", "user")]
    [InlineData("The User", "user")]
    public void Canonicalize_IsCaseInsensitive(string subject, string expected)
        => ActorSubjects.Canonicalize(subject).Should().Be(expected);

    [Fact]
    public void Canonicalize_StripsLeadingArticle()
        => ActorSubjects.Canonicalize("the qa").Should().Be("qa");

    [Fact]
    public void Canonicalize_DoesNotMergeDistinctNominalActors()
        // "username" must NOT collapse into "user".
        => ActorSubjects.Canonicalize("username").Should().Be("username");

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void TryGetSubject_IsSafeForNullOrBlank(string? stepText)
        => ActorSubjects.TryGetSubject(stepText!, out _).Should().BeFalse();

    [Fact]
    public void Register_AliasMapsToExplicitCanonical()
    {
        // §5.5: "the qa" as an alias of "user" must be explicit and verifiable.
        ActorSubjects.Register("the qa", "user");
        ActorSubjects.IsPronoun("the qa").Should().BeTrue();
        ActorSubjects.Canonicalize("the qa").Should().Be("user");
        // After registration, a step starting with "the qa" resolves to "user".
        ActorSubjects.TryGetSubject("the qa reviews the report", out var canonical).Should().BeTrue();
        canonical.Should().Be("user");
    }

    [Fact]
    public void DisplayName_CapitalizesCanonical()
    {
        ActorSubjects.DisplayName("user").Should().Be("User");
        ActorSubjects.DisplayName("i").Should().Be("I");
    }
}

