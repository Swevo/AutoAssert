namespace AutoAssert;

/// <summary>
/// Assertions for <see cref="string"/> values.
/// </summary>
public readonly struct StringAssertions
{
    private readonly string? _subject;

    internal StringAssertions(string? subject) => _subject = subject;

    public void Be(string? expected, string because = "", params object[] becauseArgs)
    {
        if (!string.Equals(_subject, expected, StringComparison.Ordinal))
        {
            AssertionHelpers.Fail(
                $"Expected string to be {AssertionHelpers.Format(expected)}, but found {AssertionHelpers.Format(_subject)}.",
                because, becauseArgs);
        }
    }

    public void NotBe(string? unexpected, string because = "", params object[] becauseArgs)
    {
        if (string.Equals(_subject, unexpected, StringComparison.Ordinal))
        {
            AssertionHelpers.Fail($"Expected string not to be {AssertionHelpers.Format(unexpected)}, but it was.", because, becauseArgs);
        }
    }

    public void BeNull(string because = "", params object[] becauseArgs)
    {
        if (_subject is not null)
        {
            AssertionHelpers.Fail($"Expected string to be null, but found {AssertionHelpers.Format(_subject)}.", because, becauseArgs);
        }
    }

    public void NotBeNull(string because = "", params object[] becauseArgs)
    {
        if (_subject is null)
        {
            AssertionHelpers.Fail("Expected string not to be null, but it was.", because, becauseArgs);
        }
    }

    public void BeEmpty(string because = "", params object[] becauseArgs)
    {
        if (_subject is not { Length: 0 })
        {
            AssertionHelpers.Fail($"Expected string to be empty, but found {AssertionHelpers.Format(_subject)}.", because, becauseArgs);
        }
    }

    public void NotBeEmpty(string because = "", params object[] becauseArgs)
    {
        if (_subject is { Length: 0 })
        {
            AssertionHelpers.Fail("Expected string not to be empty.", because, becauseArgs);
        }
    }

    public void BeNullOrEmpty(string because = "", params object[] becauseArgs)
    {
        if (!string.IsNullOrEmpty(_subject))
        {
            AssertionHelpers.Fail($"Expected string to be null or empty, but found {AssertionHelpers.Format(_subject)}.", because, becauseArgs);
        }
    }

    public void NotBeNullOrEmpty(string because = "", params object[] becauseArgs)
    {
        if (string.IsNullOrEmpty(_subject))
        {
            AssertionHelpers.Fail("Expected string not to be null or empty.", because, becauseArgs);
        }
    }

    public void Contain(string expected, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || _subject.IndexOf(expected, StringComparison.Ordinal) < 0)
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} to contain {AssertionHelpers.Format(expected)}.", because, becauseArgs);
        }
    }

    public void NotContain(string unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject is not null && _subject.IndexOf(unexpected, StringComparison.Ordinal) >= 0)
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} not to contain {AssertionHelpers.Format(unexpected)}.", because, becauseArgs);
        }
    }

    public void StartWith(string expected, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !_subject.StartsWith(expected, StringComparison.Ordinal))
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} to start with {AssertionHelpers.Format(expected)}.", because, becauseArgs);
        }
    }

    public void EndWith(string expected, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !_subject.EndsWith(expected, StringComparison.Ordinal))
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} to end with {AssertionHelpers.Format(expected)}.", because, becauseArgs);
        }
    }

    public void HaveLength(int expected, string because = "", params object[] becauseArgs)
    {
        var actual = _subject?.Length ?? 0;
        if (actual != expected)
        {
            AssertionHelpers.Fail($"Expected string to have length {expected}, but found {actual}.", because, becauseArgs);
        }
    }

    public void MatchRegex(string pattern, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !System.Text.RegularExpressions.Regex.IsMatch(_subject, pattern))
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} to match pattern {AssertionHelpers.Format(pattern)}.", because, becauseArgs);
        }
    }
}
