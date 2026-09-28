namespace AutoAssert;

/// <summary>
/// Assertions for <see cref="Enum"/> values.
/// </summary>
public readonly struct EnumAssertions<TEnum> where TEnum : struct, Enum
{
    private readonly TEnum _subject;

    internal EnumAssertions(TEnum subject) => _subject = subject;

    public AndConstraint<EnumAssertions<TEnum>> Be(TEnum expected, string because = "", params object[] becauseArgs)
    {
        if (!_subject.Equals(expected))
        {
            AssertionHelpers.Fail($"Expected {expected}, but found {_subject}.", because, becauseArgs);
        }

        return new AndConstraint<EnumAssertions<TEnum>>(this);
    }

    public AndConstraint<EnumAssertions<TEnum>> NotBe(TEnum unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject.Equals(unexpected))
        {
            AssertionHelpers.Fail($"Expected value not to be {unexpected}, but it was.", because, becauseArgs);
        }

        return new AndConstraint<EnumAssertions<TEnum>>(this);
    }

    /// <summary>Asserts the subject has all bits of <paramref name="expectedFlag"/> set (requires a <c>[Flags]</c> enum).</summary>
    public AndConstraint<EnumAssertions<TEnum>> HaveFlag(TEnum expectedFlag, string because = "", params object[] becauseArgs)
    {
        if (!_subject.HasFlag(expectedFlag))
        {
            AssertionHelpers.Fail($"Expected {_subject} to have flag {expectedFlag}.", because, becauseArgs);
        }

        return new AndConstraint<EnumAssertions<TEnum>>(this);
    }

    public AndConstraint<EnumAssertions<TEnum>> NotHaveFlag(TEnum unexpectedFlag, string because = "", params object[] becauseArgs)
    {
        if (_subject.HasFlag(unexpectedFlag))
        {
            AssertionHelpers.Fail($"Expected {_subject} not to have flag {unexpectedFlag}.", because, becauseArgs);
        }

        return new AndConstraint<EnumAssertions<TEnum>>(this);
    }
}
