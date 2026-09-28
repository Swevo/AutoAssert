namespace AutoAssert;

/// <summary>
/// Assertions for <see cref="bool"/> values.
/// </summary>
public readonly struct BooleanAssertions
{
    private readonly bool _subject;

    internal BooleanAssertions(bool subject) => _subject = subject;

    public AndConstraint<BooleanAssertions> BeTrue(string because = "", params object[] becauseArgs)
    {
        if (!_subject)
        {
            AssertionHelpers.Fail("Expected value to be true, but found false.", because, becauseArgs);
        }

        return new AndConstraint<BooleanAssertions>(this);
    }

    public AndConstraint<BooleanAssertions> BeFalse(string because = "", params object[] becauseArgs)
    {
        if (_subject)
        {
            AssertionHelpers.Fail("Expected value to be false, but found true.", because, becauseArgs);
        }

        return new AndConstraint<BooleanAssertions>(this);
    }

    public AndConstraint<BooleanAssertions> Be(bool expected, string because = "", params object[] becauseArgs)
    {
        if (_subject != expected)
        {
            AssertionHelpers.Fail($"Expected value to be {expected}, but found {_subject}.", because, becauseArgs);
        }

        return new AndConstraint<BooleanAssertions>(this);
    }
}
