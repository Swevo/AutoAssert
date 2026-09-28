namespace AutoAssert;

/// <summary>
/// Assertions for numeric primitive types (int, long, short, byte, uint, ulong, ushort, sbyte,
/// double, float, decimal).
/// </summary>
public readonly struct NumericAssertions<T> where T : IComparable<T>
{
    private readonly T _subject;

    internal NumericAssertions(T subject) => _subject = subject;

    internal T Subject() => _subject;

    public AndConstraint<NumericAssertions<T>> Be(T expected, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(expected) != 0)
        {
            AssertionHelpers.Fail($"Expected {expected}, but found {_subject}.", because, becauseArgs);
        }

        return new AndConstraint<NumericAssertions<T>>(this);
    }

    public AndConstraint<NumericAssertions<T>> NotBe(T unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(unexpected) == 0)
        {
            AssertionHelpers.Fail($"Expected value not to be {unexpected}, but it was.", because, becauseArgs);
        }

        return new AndConstraint<NumericAssertions<T>>(this);
    }

    public AndConstraint<NumericAssertions<T>> BeGreaterThan(T expected, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(expected) <= 0)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be greater than {expected}.", because, becauseArgs);
        }

        return new AndConstraint<NumericAssertions<T>>(this);
    }

    public AndConstraint<NumericAssertions<T>> BeGreaterOrEqualTo(T expected, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(expected) < 0)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be greater than or equal to {expected}.", because, becauseArgs);
        }

        return new AndConstraint<NumericAssertions<T>>(this);
    }

    public AndConstraint<NumericAssertions<T>> BeLessThan(T expected, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(expected) >= 0)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be less than {expected}.", because, becauseArgs);
        }

        return new AndConstraint<NumericAssertions<T>>(this);
    }

    public AndConstraint<NumericAssertions<T>> BeLessOrEqualTo(T expected, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(expected) > 0)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be less than or equal to {expected}.", because, becauseArgs);
        }

        return new AndConstraint<NumericAssertions<T>>(this);
    }

    public AndConstraint<NumericAssertions<T>> BeInRange(T minimum, T maximum, string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(minimum) < 0 || _subject.CompareTo(maximum) > 0)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be between {minimum} and {maximum}.", because, becauseArgs);
        }

        return new AndConstraint<NumericAssertions<T>>(this);
    }

    /// <summary>Asserts the value is strictly greater than zero.</summary>
    public AndConstraint<NumericAssertions<T>> BePositive(string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(default!) <= 0)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be positive.", because, becauseArgs);
        }

        return new AndConstraint<NumericAssertions<T>>(this);
    }

    /// <summary>Asserts the value is strictly less than zero.</summary>
    public AndConstraint<NumericAssertions<T>> BeNegative(string because = "", params object[] becauseArgs)
    {
        if (_subject.CompareTo(default!) >= 0)
        {
            AssertionHelpers.Fail($"Expected {_subject} to be negative.", because, becauseArgs);
        }

        return new AndConstraint<NumericAssertions<T>>(this);
    }

    public AndConstraint<NumericAssertions<T>> BeOneOf(params T[] candidates)
    {
        var subject = _subject;
        if (!candidates.Any(candidate => subject.CompareTo(candidate) == 0))
        {
            AssertionHelpers.Fail(
                $"Expected {_subject} to be one of [{string.Join(", ", candidates)}], but it was not.", "", []);
        }

        return new AndConstraint<NumericAssertions<T>>(this);
    }
}

/// <summary>
/// Extension methods providing floating-point specific assertions (BeApproximately, BeNaN)
/// without widening the generic <see cref="NumericAssertions{T}"/> constraint.
/// </summary>
public static class FloatingPointAssertionExtensions
{
    public static AndConstraint<NumericAssertions<double>> BeApproximately(this NumericAssertions<double> assertions, double expected, double precision,
        string because = "", params object[] becauseArgs)
    {
        BeApproximatelyCore(assertions.Subject(), expected, precision, because, becauseArgs);
        return new AndConstraint<NumericAssertions<double>>(assertions);
    }

    public static AndConstraint<NumericAssertions<float>> BeApproximately(this NumericAssertions<float> assertions, float expected, float precision,
        string because = "", params object[] becauseArgs)
    {
        BeApproximatelyCore(assertions.Subject(), expected, precision, because, becauseArgs);
        return new AndConstraint<NumericAssertions<float>>(assertions);
    }

    public static AndConstraint<NumericAssertions<decimal>> BeApproximately(this NumericAssertions<decimal> assertions, decimal expected, decimal precision,
        string because = "", params object[] becauseArgs)
    {
        BeApproximatelyCore(assertions.Subject(), expected, precision, because, becauseArgs);
        return new AndConstraint<NumericAssertions<decimal>>(assertions);
    }

    public static AndConstraint<NumericAssertions<double>> BeNaN(this NumericAssertions<double> assertions, string because = "", params object[] becauseArgs)
    {
        if (!double.IsNaN(assertions.Subject()))
        {
            AssertionHelpers.Fail($"Expected {assertions.Subject()} to be NaN.", because, becauseArgs);
        }

        return new AndConstraint<NumericAssertions<double>>(assertions);
    }

    public static AndConstraint<NumericAssertions<double>> NotBeNaN(this NumericAssertions<double> assertions, string because = "", params object[] becauseArgs)
    {
        if (double.IsNaN(assertions.Subject()))
        {
            AssertionHelpers.Fail("Expected value not to be NaN, but it was.", because, becauseArgs);
        }

        return new AndConstraint<NumericAssertions<double>>(assertions);
    }

    public static AndConstraint<NumericAssertions<float>> BeNaN(this NumericAssertions<float> assertions, string because = "", params object[] becauseArgs)
    {
        if (!float.IsNaN(assertions.Subject()))
        {
            AssertionHelpers.Fail($"Expected {assertions.Subject()} to be NaN.", because, becauseArgs);
        }

        return new AndConstraint<NumericAssertions<float>>(assertions);
    }

    public static AndConstraint<NumericAssertions<float>> NotBeNaN(this NumericAssertions<float> assertions, string because = "", params object[] becauseArgs)
    {
        if (float.IsNaN(assertions.Subject()))
        {
            AssertionHelpers.Fail("Expected value not to be NaN, but it was.", because, becauseArgs);
        }

        return new AndConstraint<NumericAssertions<float>>(assertions);
    }

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
