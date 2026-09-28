namespace AutoAssert;

/// <summary>
/// Assertions for <see cref="IDictionary{TKey, TValue}"/> collections.
/// </summary>
public readonly struct DictionaryAssertions<TKey, TValue> where TKey : notnull
{
    private readonly IDictionary<TKey, TValue>? _subject;

    internal DictionaryAssertions(IDictionary<TKey, TValue>? subject) => _subject = subject;

    public AndConstraint<DictionaryAssertions<TKey, TValue>> BeEmpty(string because = "", params object[] becauseArgs)
    {
        if (_subject is null || _subject.Count != 0)
        {
            AssertionHelpers.Fail("Expected dictionary to be empty, but it was not.", because, becauseArgs);
        }

        return new AndConstraint<DictionaryAssertions<TKey, TValue>>(this);
    }

    public AndConstraint<DictionaryAssertions<TKey, TValue>> NotBeEmpty(string because = "", params object[] becauseArgs)
    {
        if (_subject is null || _subject.Count == 0)
        {
            AssertionHelpers.Fail("Expected dictionary not to be empty, but it was.", because, becauseArgs);
        }

        return new AndConstraint<DictionaryAssertions<TKey, TValue>>(this);
    }

    public AndConstraint<DictionaryAssertions<TKey, TValue>> HaveCount(int expected, string because = "", params object[] becauseArgs)
    {
        var actual = _subject?.Count ?? 0;
        if (actual != expected)
        {
            AssertionHelpers.Fail($"Expected dictionary to have {expected} item(s), but found {actual}.", because, becauseArgs);
        }

        return new AndConstraint<DictionaryAssertions<TKey, TValue>>(this);
    }

    public AndConstraint<DictionaryAssertions<TKey, TValue>> ContainKey(TKey expected, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !_subject.ContainsKey(expected))
        {
            AssertionHelpers.Fail($"Expected dictionary to contain key {AssertionHelpers.Format(expected)}, but it did not.", because, becauseArgs);
        }

        return new AndConstraint<DictionaryAssertions<TKey, TValue>>(this);
    }

    public AndConstraint<DictionaryAssertions<TKey, TValue>> NotContainKey(TKey unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject is not null && _subject.ContainsKey(unexpected))
        {
            AssertionHelpers.Fail($"Expected dictionary not to contain key {AssertionHelpers.Format(unexpected)}, but it did.", because, becauseArgs);
        }

        return new AndConstraint<DictionaryAssertions<TKey, TValue>>(this);
    }

    public AndConstraint<DictionaryAssertions<TKey, TValue>> ContainValue(TValue expected, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !_subject.Values.Contains(expected))
        {
            AssertionHelpers.Fail($"Expected dictionary to contain value {AssertionHelpers.Format(expected)}, but it did not.", because, becauseArgs);
        }

        return new AndConstraint<DictionaryAssertions<TKey, TValue>>(this);
    }

    public AndConstraint<DictionaryAssertions<TKey, TValue>> NotContainValue(TValue unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject is not null && _subject.Values.Contains(unexpected))
        {
            AssertionHelpers.Fail($"Expected dictionary not to contain value {AssertionHelpers.Format(unexpected)}, but it did.", because, becauseArgs);
        }

        return new AndConstraint<DictionaryAssertions<TKey, TValue>>(this);
    }

    public AndConstraint<DictionaryAssertions<TKey, TValue>> ContainKeyAndValue(TKey key, TValue expectedValue, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !_subject.TryGetValue(key, out var actualValue))
        {
            AssertionHelpers.Fail($"Expected dictionary to contain key {AssertionHelpers.Format(key)}, but it did not.", because, becauseArgs);
        }
        else if (!Equals(actualValue, expectedValue))
        {
            AssertionHelpers.Fail(
                $"Expected dictionary key {AssertionHelpers.Format(key)} to have value {AssertionHelpers.Format(expectedValue)}, but found {AssertionHelpers.Format(actualValue)}.",
                because, becauseArgs);
        }

        return new AndConstraint<DictionaryAssertions<TKey, TValue>>(this);
    }
}
