using System.Text.RegularExpressions;

namespace AutoAssert;

/// <summary>
/// Assertions for <see cref="string"/> values.
/// </summary>
public readonly struct StringAssertions
{
    private readonly string? _subject;

    internal StringAssertions(string? subject) => _subject = subject;

    /// <summary>The underlying string being asserted against (used by companion packages such as AutoAssert.Json).</summary>
    public string? Subject => _subject;

    public AndConstraint<StringAssertions> Be(string? expected, string because = "", params object[] becauseArgs)
    {
        if (!string.Equals(_subject, expected, StringComparison.Ordinal))
        {
            AssertionHelpers.Fail(
                $"Expected string to be {AssertionHelpers.Format(expected)}, but found {AssertionHelpers.Format(_subject)}.",
                because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> NotBe(string? unexpected, string because = "", params object[] becauseArgs)
    {
        if (string.Equals(_subject, unexpected, StringComparison.Ordinal))
        {
            AssertionHelpers.Fail($"Expected string not to be {AssertionHelpers.Format(unexpected)}, but it was.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    /// <summary>Case-insensitive equality check.</summary>
    public AndConstraint<StringAssertions> BeEquivalentTo(string? expected, string because = "", params object[] becauseArgs)
    {
        if (!string.Equals(_subject, expected, StringComparison.OrdinalIgnoreCase))
        {
            AssertionHelpers.Fail(
                $"Expected string to be equivalent to {AssertionHelpers.Format(expected)}, but found {AssertionHelpers.Format(_subject)}.",
                because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> BeNull(string because = "", params object[] becauseArgs)
    {
        if (_subject is not null)
        {
            AssertionHelpers.Fail($"Expected string to be null, but found {AssertionHelpers.Format(_subject)}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> NotBeNull(string because = "", params object[] becauseArgs)
    {
        if (_subject is null)
        {
            AssertionHelpers.Fail("Expected string not to be null, but it was.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> BeEmpty(string because = "", params object[] becauseArgs)
    {
        if (_subject is not { Length: 0 })
        {
            AssertionHelpers.Fail($"Expected string to be empty, but found {AssertionHelpers.Format(_subject)}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> NotBeEmpty(string because = "", params object[] becauseArgs)
    {
        if (_subject is { Length: 0 })
        {
            AssertionHelpers.Fail("Expected string not to be empty.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> BeNullOrEmpty(string because = "", params object[] becauseArgs)
    {
        if (!string.IsNullOrEmpty(_subject))
        {
            AssertionHelpers.Fail($"Expected string to be null or empty, but found {AssertionHelpers.Format(_subject)}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> NotBeNullOrEmpty(string because = "", params object[] becauseArgs)
    {
        if (string.IsNullOrEmpty(_subject))
        {
            AssertionHelpers.Fail("Expected string not to be null or empty.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> Contain(string expected, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || _subject.IndexOf(expected, StringComparison.Ordinal) < 0)
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} to contain {AssertionHelpers.Format(expected)}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    /// <summary>Case-insensitive contains check.</summary>
    public AndConstraint<StringAssertions> ContainEquivalentOf(string expected, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || _subject.IndexOf(expected, StringComparison.OrdinalIgnoreCase) < 0)
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} to contain the equivalent of {AssertionHelpers.Format(expected)}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> NotContain(string unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject is not null && _subject.IndexOf(unexpected, StringComparison.Ordinal) >= 0)
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} not to contain {AssertionHelpers.Format(unexpected)}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> StartWith(string expected, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !_subject.StartsWith(expected, StringComparison.Ordinal))
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} to start with {AssertionHelpers.Format(expected)}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> NotStartWith(string unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject is not null && _subject.StartsWith(unexpected, StringComparison.Ordinal))
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} not to start with {AssertionHelpers.Format(unexpected)}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> EndWith(string expected, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !_subject.EndsWith(expected, StringComparison.Ordinal))
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} to end with {AssertionHelpers.Format(expected)}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> NotEndWith(string unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject is not null && _subject.EndsWith(unexpected, StringComparison.Ordinal))
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} not to end with {AssertionHelpers.Format(unexpected)}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> HaveLength(int expected, string because = "", params object[] becauseArgs)
    {
        var actual = _subject?.Length ?? 0;
        if (actual != expected)
        {
            AssertionHelpers.Fail($"Expected string to have length {expected}, but found {actual}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> MatchRegex(string pattern, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !Regex.IsMatch(_subject, pattern))
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} to match pattern {AssertionHelpers.Format(pattern)}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> NotMatchRegex(string pattern, string because = "", params object[] becauseArgs)
    {
        if (_subject is not null && Regex.IsMatch(_subject, pattern))
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} not to match pattern {AssertionHelpers.Format(pattern)}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    /// <summary>Wildcard match where <c>*</c> matches any run of characters and <c>?</c> matches a single character (e.g. <c>"W*ld"</c>).</summary>
    public AndConstraint<StringAssertions> Match(string wildcardPattern, string because = "", params object[] becauseArgs)
    {
        var regexPattern = "^" + Regex.Escape(wildcardPattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        if (_subject is null || !Regex.IsMatch(_subject, regexPattern, RegexOptions.Singleline))
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} to match wildcard pattern {AssertionHelpers.Format(wildcardPattern)}.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> BeUpperCased(string because = "", params object[] becauseArgs)
    {
        if (_subject is null || _subject != _subject.ToUpperInvariant())
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} to be all upper-cased.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }

    public AndConstraint<StringAssertions> BeLowerCased(string because = "", params object[] becauseArgs)
    {
        if (_subject is null || _subject != _subject.ToLowerInvariant())
        {
            AssertionHelpers.Fail($"Expected string {AssertionHelpers.Format(_subject)} to be all lower-cased.", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(this);
    }
}
