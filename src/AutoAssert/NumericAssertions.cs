namespace AutoAssert;

/// <summary>
/// Assertions for numeric primitive types (int, long, short, byte, double, float, decimal).
/// </summary>
public readonly struct NumericAssertions<T> where T : IComparable<T>
{
    private readonly T _subject;

    internal NumericAssertions(T subject) => _subject = subject;

    internal T Subject() => _subject;

    public void Be(T expected, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(expected) != 0)
        {
            AssertionHelpers.Fail($"Expected {expected}, but found {_subject}.", because, becauseArgs);
        }
    }

    public void NotBe(T unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(unexpected) == 0)
        {
            AssertionHelpers.Fail($"Expected value not to be {unexpected}, but it was.", because, becauseArgs);
        }
    }

    public void BeGreaterThan(T expected, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(expected) <= 0)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be greater than {expected}.", because, becauseArgs);
        }
    }

    public void BeGreaterOrEqualTo(T expected, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(expected) < 0)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be greater than or equal to {expected}.", because, becauseArgs);
        }
    }

    public void BeLessThan(T expected, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(expected) >= 0)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be less than {expected}.", because, becauseArgs);
        }
    }

    public void BeLessOrEqualTo(T expected, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(expected) > 0)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be less than or equal to {expected}.", because, becauseArgs);
        }
    }

    public void BeInRange(T minimum, T maximum, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(minimum) < 0 || _subject.CompareTo(maximum) > 0)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be between {minimum} and {maximum}.", because, becauseArgs);
        }
    }
}

/// <summary>
/// Extension methods providing floating-point specific assertions (BeApproximately)
/// without widening the generic <see cref="NumericAssertions{T}"/> constraint.
/// </summary>
public static class FloatingPointAssertionExtensions
{
    public static void BeApproximately(this NumericAssertions<double> assertions, double expected, double precision,
        string because = "", params object[] becauseArgs)
        => BeApproximatelyCore(assertions.Subject(), expected, precision, because, becauseArgs);

    public static void BeApproximately(this NumericAssertions<float> assertions, float expected, float precision,
        string because = "", params object[] becauseArgs)
        => BeApproximatelyCore(assertions.Subject(), expected, precision, because, becauseArgs);

    public static void BeApproximately(this NumericAssertions<decimal> assertions, decimal expected, decimal precision,
        string because = "", params object[] becauseArgs)
        => BeApproximatelyCore(assertions.Subject(), expected, precision, because, becauseArgs);

    private static void BeApproximatelyCore(double subject, double expected, double precision, string because, object[] becauseArgs)
    {
        if (Math.Abs(subject - expected) > precision)
        {
            AssertionHelpers.Fail($"Expected {subject} to be approximately {expected} (+/- {precision}).", because, becauseArgs);
        }
    }

    private static void BeApproximatelyCore(decimal subject, decimal expected, decimal precision, string because, object[] becauseArgs)
    {
        if (Math.Abs(subject - expected) > precision)
        {
            AssertionHelpers.Fail($"Expected {subject} to be approximately {expected} (+/- {precision}).", because, becauseArgs);
        }
    }
}
