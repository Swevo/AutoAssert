namespace AutoAssert;

/// <summary>
/// Assertions for <see cref="bool"/> values.
/// </summary>
public readonly struct BooleanAssertions
{
    private readonly bool _subject;

    internal BooleanAssertions(bool subject) => _subject = subject;

    public void BeTrue(string because = "", params object[] becauseArgs)
    {
        if (!_subject)
        {
            AssertionHelpers.Fail("Expected value to be true, but found false.", because, becauseArgs);
        }
    }

    public void BeFalse(string because = "", params object[] becauseArgs)
    {
        if (_subject)
        {
            AssertionHelpers.Fail("Expected value to be false, but found true.", because, becauseArgs);
        }
    }

    public void Be(bool expected, string because = "", params object[] becauseArgs)
    {
        if (_subject != expected)
        {
            AssertionHelpers.Fail($"Expected value to be {expected}, but found {_subject}.", because, becauseArgs);
        }
    }
}
