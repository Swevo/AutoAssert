namespace AutoAssert;

/// <summary>
/// Assertions for <see cref="object"/> and reference/value types without a more
/// specific assertion type (see <see cref="StringAssertions"/>, <see cref="NumericAssertions{T}"/>,
/// <see cref="CollectionAssertions{TItem}"/>, <see cref="BooleanAssertions"/>).
/// </summary>
public readonly struct ObjectAssertions
{
    private readonly object? _subject;

    internal ObjectAssertions(object? subject) => _subject = subject;

    public void Be(object? expected, string because = "", params object[] becauseArgs)
    {
        if (!Equals(_subject, expected))
        {
            AssertionHelpers.Fail(
                $"Expected {AssertionHelpers.Format(expected)}, but found {AssertionHelpers.Format(_subject)}.",
                because, becauseArgs);
        }
    }

    public void NotBe(object? unexpected, string because = "", params object[] becauseArgs)
    {
        if (Equals(_subject, unexpected))
        {
            AssertionHelpers.Fail(
                $"Expected value not to be {AssertionHelpers.Format(unexpected)}, but it was.",
                because, becauseArgs);
        }
    }

    public void BeNull(string because = "", params object[] becauseArgs)
    {
        if (_subject is not null)
        {
            AssertionHelpers.Fail($"Expected value to be null, but found {AssertionHelpers.Format(_subject)}.", because, becauseArgs);
        }
    }

    public void NotBeNull(string because = "", params object[] becauseArgs)
    {
        if (_subject is null)
        {
            AssertionHelpers.Fail("Expected value not to be null, but it was null.", because, becauseArgs);
        }
    }

    public void BeSameAs(object? expected, string because = "", params object[] becauseArgs)
    {
        if (!ReferenceEquals(_subject, expected))
        {
            AssertionHelpers.Fail("Expected value to be the same instance as the expected object, but it was not.", because, becauseArgs);
        }
    }

    public void NotBeSameAs(object? unexpected, string because = "", params object[] becauseArgs)
    {
        if (ReferenceEquals(_subject, unexpected))
        {
            AssertionHelpers.Fail("Expected value not to be the same instance as the given object, but it was.", because, becauseArgs);
        }
    }

    public void BeOfType<TType>(string because = "", params object[] becauseArgs)
    {
        if (_subject is null || _subject.GetType() != typeof(TType))
        {
            var actualType = _subject?.GetType().Name ?? "null";
            AssertionHelpers.Fail($"Expected type to be {typeof(TType).Name}, but found {actualType}.", because, becauseArgs);
        }
    }

    public void BeAssignableTo<TType>(string because = "", params object[] becauseArgs)
    {
        if (_subject is not TType)
        {
            var actualType = _subject?.GetType().Name ?? "null";
            AssertionHelpers.Fail($"Expected type to be assignable to {typeof(TType).Name}, but found {actualType}.", because, becauseArgs);
        }
    }

    public void Match(Func<object?, bool> predicate, string because = "", params object[] becauseArgs)
    {
        if (!predicate(_subject))
        {
            AssertionHelpers.Fail($"Expected value {AssertionHelpers.Format(_subject)} to match the given predicate, but it did not.", because, becauseArgs);
        }
    }

    public void BeEquivalentTo(object? expected, string because = "", params object[] becauseArgs)
    {
        EquivalencyAssertions.AssertEquivalent(_subject, expected, because, becauseArgs);
    }
}
