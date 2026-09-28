namespace AutoAssert;

/// <summary>
/// Assertions for <see cref="TimeSpan"/> values.
/// </summary>
public readonly struct TimeSpanAssertions
{
    private readonly TimeSpan _subject;

    internal TimeSpanAssertions(TimeSpan subject) => _subject = subject;

    public AndConstraint<TimeSpanAssertions> Be(TimeSpan expected, string because = "", params object[] becauseArgs)
    {
        if (_subject != expected)
        {
            AssertionHelpers.Fail($"Expected {expected}, but found {_subject}.", because, becauseArgs);
        }

        return new AndConstraint<TimeSpanAssertions>(this);
    }

    public AndConstraint<TimeSpanAssertions> NotBe(TimeSpan unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject == unexpected)
        {
            AssertionHelpers.Fail($"Expected value not to be {unexpected}, but it was.", because, becauseArgs);
        }

        return new AndConstraint<TimeSpanAssertions>(this);
    }

    public AndConstraint<TimeSpanAssertions> BeGreaterThan(TimeSpan expected, string because = "", params object[] becauseArgs)
    {
        if (_subject <= expected)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be greater than {expected}.", because, becauseArgs);
        }

        return new AndConstraint<TimeSpanAssertions>(this);
    }

    public AndConstraint<TimeSpanAssertions> BeLessThan(TimeSpan expected, string because = "", params object[] becauseArgs)
    {
        if (_subject >= expected)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be less than {expected}.", because, becauseArgs);
        }

        return new AndConstraint<TimeSpanAssertions>(this);
    }

    public AndConstraint<TimeSpanAssertions> BeCloseTo(TimeSpan expected, TimeSpan precision, string because = "", params object[] becauseArgs)
    {
        if ((_subject - expected).Duration() > precision)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be within {precision} of {expected}.", because, becauseArgs);
        }

        return new AndConstraint<TimeSpanAssertions>(this);
    }
}
