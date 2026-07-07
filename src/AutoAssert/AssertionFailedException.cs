namespace AutoAssert;

/// <summary>
/// Thrown when an AutoAssert assertion fails. Recognized as a test failure by
/// xUnit, NUnit, and MSTest since all three treat any thrown exception as a failing test.
/// </summary>
public sealed class AssertionFailedException : Exception
{
    public AssertionFailedException(string message) : base(message)
    {
    }
}
