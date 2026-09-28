namespace AutoAssert;

/// <summary>
/// Assertions for <see cref="Nullable{T}"/> value types.
/// </summary>
public readonly struct NullableAssertions<T> where T : struct
{
    private readonly T? _subject;

    internal NullableAssertions(T? subject) => _subject = subject;

    public AndConstraint<NullableAssertions<T>> HaveValue(string because = "", params object[] becauseArgs)
    {
        if (!_subject.HasValue)
        {
            AssertionHelpers.Fail("Expected value to have a value, but it was null.", because, becauseArgs);
        }

        return new AndConstraint<NullableAssertions<T>>(this);
    }

    public AndConstraint<NullableAssertions<T>> NotHaveValue(string because = "", params object[] becauseArgs)
    {
        if (_subject.HasValue)
        {
            AssertionHelpers.Fail($"Expected value not to have a value, but found {_subject}.", because, becauseArgs);
        }

        return new AndConstraint<NullableAssertions<T>>(this);
    }

    public AndConstraint<NullableAssertions<T>> Be(T? expected, string because = "", params object[] becauseArgs)
    {
        if (!EqualityComparer<T?>.Default.Equals(_subject, expected))
        {
            AssertionHelpers.Fail($"Expected {AssertionHelpers.Format(expected)}, but found {AssertionHelpers.Format(_subject)}.", because, becauseArgs);
        }

        return new AndConstraint<NullableAssertions<T>>(this);
    }

    public AndConstraint<NullableAssertions<T>> NotBe(T? unexpected, string because = "", params object[] becauseArgs)
    {
        if (EqualityComparer<T?>.Default.Equals(_subject, unexpected))
        {
            AssertionHelpers.Fail($"Expected value not to be {AssertionHelpers.Format(unexpected)}, but it was.", because, becauseArgs);
        }

        return new AndConstraint<NullableAssertions<T>>(this);
    }
}
