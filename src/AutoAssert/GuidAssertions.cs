namespace AutoAssert;

/// <summary>
/// Assertions for <see cref="Guid"/> values.
/// </summary>
public readonly struct GuidAssertions
{
    private readonly Guid _subject;

    internal GuidAssertions(Guid subject) => _subject = subject;

    public AndConstraint<GuidAssertions> Be(Guid expected, string because = "", params object[] becauseArgs)
    {
        if (_subject != expected)
        {
            AssertionHelpers.Fail($"Expected {expected}, but found {_subject}.", because, becauseArgs);
        }

        return new AndConstraint<GuidAssertions>(this);
    }

    public AndConstraint<GuidAssertions> NotBe(Guid unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject == unexpected)
        {
            AssertionHelpers.Fail($"Expected value not to be {unexpected}, but it was.", because, becauseArgs);
        }

        return new AndConstraint<GuidAssertions>(this);
    }

    public AndConstraint<GuidAssertions> BeEmpty(string because = "", params object[] becauseArgs)
    {
        if (_subject != Guid.Empty)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be Guid.Empty.", because, becauseArgs);
        }

        return new AndConstraint<GuidAssertions>(this);
    }

    public AndConstraint<GuidAssertions> NotBeEmpty(string because = "", params object[] becauseArgs)
    {
        if (_subject == Guid.Empty)
        {
            AssertionHelpers.Fail("Expected value not to be Guid.Empty, but it was.", because, becauseArgs);
        }

        return new AndConstraint<GuidAssertions>(this);
    }
}
