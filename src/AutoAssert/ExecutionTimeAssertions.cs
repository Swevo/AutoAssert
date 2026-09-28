using System.Diagnostics;

namespace AutoAssert;

/// <summary>
/// Assertions on how long a synchronous action took to run — captured by
/// <see cref="ExecutionTimeAssertionExtensions.ExecutionTime"/>.
/// </summary>
public readonly struct ExecutionTimeAssertions
{
    private readonly TimeSpan _elapsed;

    internal ExecutionTimeAssertions(TimeSpan elapsed) => _elapsed = elapsed;

    public AndConstraint<ExecutionTimeAssertions> BeLessThan(TimeSpan expected, string because = "", params object[] becauseArgs)
    {
        if (_elapsed >= expected)
        {
            AssertionHelpers.Fail($"Expected execution to take less than {expected}, but it took {_elapsed}.", because, becauseArgs);
        }

        return new AndConstraint<ExecutionTimeAssertions>(this);
    }

    public AndConstraint<ExecutionTimeAssertions> BeLessOrEqualTo(TimeSpan expected, string because = "", params object[] becauseArgs)
    {
        if (_elapsed > expected)
        {
            AssertionHelpers.Fail($"Expected execution to take at most {expected}, but it took {_elapsed}.", because, becauseArgs);
        }

        return new AndConstraint<ExecutionTimeAssertions>(this);
    }

    public AndConstraint<ExecutionTimeAssertions> BeGreaterThan(TimeSpan expected, string because = "", params object[] becauseArgs)
    {
        if (_elapsed <= expected)
        {
            AssertionHelpers.Fail($"Expected execution to take more than {expected}, but it took {_elapsed}.", because, becauseArgs);
        }

        return new AndConstraint<ExecutionTimeAssertions>(this);
    }
}

/// <summary>
/// Entry points for timing-based assertions, e.g. <c>action.ExecutionTime().Should().BeLessThan(...)</c>
/// or the shorthand <c>action.Should().CompleteWithin(...)</c>.
/// </summary>
public static class ExecutionTimeAssertionExtensions
{
    /// <summary>Runs <paramref name="action"/>, measuring its elapsed wall-clock time for further assertions via <c>.Should()</c>.</summary>
    public static ExecutionTimeMeasurement ExecutionTime(this Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return new ExecutionTimeMeasurement(stopwatch.Elapsed);
    }

    /// <summary>Shorthand for asserting a synchronous action completes within a given duration.</summary>
    public static AndConstraint<ActionAssertions> CompleteWithin(this ActionAssertions assertions, TimeSpan maxDuration, string because = "", params object[] becauseArgs)
    {
        var elapsed = assertions.MeasureExecutionTime();
        if (elapsed > maxDuration)
        {
            AssertionHelpers.Fail($"Expected action to complete within {maxDuration}, but it took {elapsed}.", because, becauseArgs);
        }

        return new AndConstraint<ActionAssertions>(assertions);
    }

    /// <summary>Shorthand for asserting an asynchronous operation completes within a given duration.</summary>
    public static async Task<AndConstraint<FuncAssertions>> CompleteWithinAsync(this FuncAssertions assertions, TimeSpan maxDuration, string because = "", params object[] becauseArgs)
    {
        var stopwatch = Stopwatch.StartNew();
        await assertions.InvokeAsync().ConfigureAwait(false);
        stopwatch.Stop();

        if (stopwatch.Elapsed > maxDuration)
        {
            AssertionHelpers.Fail($"Expected operation to complete within {maxDuration}, but it took {stopwatch.Elapsed}.", because, becauseArgs);
        }

        return new AndConstraint<FuncAssertions>(assertions);
    }
}

/// <summary>A captured elapsed duration, ready for assertion via <see cref="Should"/>.</summary>
public readonly struct ExecutionTimeMeasurement
{
    private readonly TimeSpan _elapsed;

    internal ExecutionTimeMeasurement(TimeSpan elapsed) => _elapsed = elapsed;

    public ExecutionTimeAssertions Should() => new(_elapsed);
}
