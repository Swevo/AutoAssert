namespace AutoAssert;

/// <summary>
/// Assertions for <see cref="DateTime"/> values.
/// </summary>
public readonly struct DateTimeAssertions
{
    private readonly DateTime _subject;

    internal DateTimeAssertions(DateTime subject) => _subject = subject;

    public AndConstraint<DateTimeAssertions> Be(DateTime expected, string because = "", params object[] becauseArgs)
    {
        if (_subject != expected)
        {
            AssertionHelpers.Fail($"Expected {expected:O}, but found {_subject:O}.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeAssertions>(this);
    }

    public AndConstraint<DateTimeAssertions> NotBe(DateTime unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject == unexpected)
        {
            AssertionHelpers.Fail($"Expected value not to be {unexpected:O}, but it was.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeAssertions>(this);
    }

    public AndConstraint<DateTimeAssertions> BeBefore(DateTime expected, string because = "", params object[] becauseArgs)
    {
        if (_subject >= expected)
        {
            AssertionHelpers.Fail($"Expected {_subject:O} to be before {expected:O}.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeAssertions>(this);
    }

    public AndConstraint<DateTimeAssertions> BeAfter(DateTime expected, string because = "", params object[] becauseArgs)
    {
        if (_subject <= expected)
        {
            AssertionHelpers.Fail($"Expected {_subject:O} to be after {expected:O}.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeAssertions>(this);
    }

    public AndConstraint<DateTimeAssertions> BeOnOrBefore(DateTime expected, string because = "", params object[] becauseArgs)
    {
        if (_subject > expected)
        {
            AssertionHelpers.Fail($"Expected {_subject:O} to be on or before {expected:O}.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeAssertions>(this);
    }

    public AndConstraint<DateTimeAssertions> BeOnOrAfter(DateTime expected, string because = "", params object[] becauseArgs)
    {
        if (_subject < expected)
        {
            AssertionHelpers.Fail($"Expected {_subject:O} to be on or after {expected:O}.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeAssertions>(this);
    }

    public AndConstraint<DateTimeAssertions> BeCloseTo(DateTime expected, TimeSpan precision, string because = "", params object[] becauseArgs)
    {
        if ((_subject - expected).Duration() > precision)
        {
            AssertionHelpers.Fail($"Expected {_subject:O} to be within {precision} of {expected:O}.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeAssertions>(this);
    }

    public AndConstraint<DateTimeAssertions> BeSameDateAs(DateTime expected, string because = "", params object[] becauseArgs)
    {
        if (_subject.Date != expected.Date)
        {
            AssertionHelpers.Fail($"Expected {_subject:O} to have the same date as {expected:O}.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeAssertions>(this);
    }
}

/// <summary>
/// Assertions for <see cref="DateTimeOffset"/> values.
/// </summary>
public readonly struct DateTimeOffsetAssertions
{
    private readonly DateTimeOffset _subject;

    internal DateTimeOffsetAssertions(DateTimeOffset subject) => _subject = subject;

    public AndConstraint<DateTimeOffsetAssertions> Be(DateTimeOffset expected, string because = "", params object[] becauseArgs)
    {
        if (_subject != expected)
        {
            AssertionHelpers.Fail($"Expected {expected:O}, but found {_subject:O}.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeOffsetAssertions>(this);
    }

    public AndConstraint<DateTimeOffsetAssertions> NotBe(DateTimeOffset unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject == unexpected)
        {
            AssertionHelpers.Fail($"Expected value not to be {unexpected:O}, but it was.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeOffsetAssertions>(this);
    }

    public AndConstraint<DateTimeOffsetAssertions> BeBefore(DateTimeOffset expected, string because = "", params object[] becauseArgs)
    {
        if (_subject >= expected)
        {
            AssertionHelpers.Fail($"Expected {_subject:O} to be before {expected:O}.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeOffsetAssertions>(this);
    }

    public AndConstraint<DateTimeOffsetAssertions> BeAfter(DateTimeOffset expected, string because = "", params object[] becauseArgs)
    {
        if (_subject <= expected)
        {
            AssertionHelpers.Fail($"Expected {_subject:O} to be after {expected:O}.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeOffsetAssertions>(this);
    }

    public AndConstraint<DateTimeOffsetAssertions> BeOnOrBefore(DateTimeOffset expected, string because = "", params object[] becauseArgs)
    {
        if (_subject > expected)
        {
            AssertionHelpers.Fail($"Expected {_subject:O} to be on or before {expected:O}.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeOffsetAssertions>(this);
    }

    public AndConstraint<DateTimeOffsetAssertions> BeOnOrAfter(DateTimeOffset expected, string because = "", params object[] becauseArgs)
    {
        if (_subject < expected)
        {
            AssertionHelpers.Fail($"Expected {_subject:O} to be on or after {expected:O}.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeOffsetAssertions>(this);
    }

    public AndConstraint<DateTimeOffsetAssertions> BeCloseTo(DateTimeOffset expected, TimeSpan precision, string because = "", params object[] becauseArgs)
    {
        if ((_subject - expected).Duration() > precision)
        {
            AssertionHelpers.Fail($"Expected {_subject:O} to be within {precision} of {expected:O}.", because, becauseArgs);
        }

        return new AndConstraint<DateTimeOffsetAssertions>(this);
    }
}
