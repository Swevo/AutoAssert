namespace AutoAssert;

/// <summary>
/// Runtime assertion utilities for extension packages.
/// </summary>
public static class AssertionRuntime
{
    /// <summary>
    /// Records an assertion failure in the current <see cref="AssertionScope"/> when present,
    /// otherwise throws <see cref="AssertionFailedException"/>.
    /// </summary>
    public static void Fail(string message, string because = "", params object[] becauseArgs)
    {
        AssertionHelpers.Fail(message, because, becauseArgs);
    }
}
