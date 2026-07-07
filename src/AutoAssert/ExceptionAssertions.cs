namespace AutoAssert;

/// <summary>
/// Assertions for synchronous actions expected to throw (or not throw) exceptions.
/// </summary>
public readonly struct ActionAssertions
{
    private readonly Action _action;

    internal ActionAssertions(Action action) => _action = action;

    public ExceptionAssertions<TException> Throw<TException>(string because = "", params object[] becauseArgs)
        where TException : Exception
    {
        try
        {
            _action();
        }
        catch (TException ex)
        {
            return new ExceptionAssertions<TException>(ex);
        }
        catch (Exception ex)
        {
            AssertionHelpers.Fail(
                $"Expected a {typeof(TException).Name} to be thrown, but found {ex.GetType().Name}: \"{ex.Message}\".",
                because, becauseArgs);
        }

        AssertionHelpers.Fail($"Expected a {typeof(TException).Name} to be thrown, but no exception was thrown.", because, becauseArgs);
        return default!; // unreachable: AssertionHelpers.Fail always throws
    }

    public void NotThrow(string because = "", params object[] becauseArgs)
    {
        try
        {
            _action();
        }
        catch (Exception ex)
        {
            AssertionHelpers.Fail($"Expected no exception to be thrown, but found {ex.GetType().Name}: \"{ex.Message}\".", because, becauseArgs);
        }
    }
}

/// <summary>
/// Assertions for asynchronous operations expected to throw (or not throw) exceptions.
/// </summary>
public readonly struct FuncAssertions
{
    private readonly Func<Task> _action;

    internal FuncAssertions(Func<Task> action) => _action = action;

    public async Task<ExceptionAssertions<TException>> ThrowAsync<TException>(string because = "", params object[] becauseArgs)
        where TException : Exception
    {
        try
        {
            await _action().ConfigureAwait(false);
        }
        catch (TException ex)
        {
            return new ExceptionAssertions<TException>(ex);
        }
        catch (Exception ex)
        {
            AssertionHelpers.Fail(
                $"Expected a {typeof(TException).Name} to be thrown, but found {ex.GetType().Name}: \"{ex.Message}\".",
                because, becauseArgs);
        }

        AssertionHelpers.Fail($"Expected a {typeof(TException).Name} to be thrown, but no exception was thrown.", because, becauseArgs);
        return default!; // unreachable: AssertionHelpers.Fail always throws
    }

    public async Task NotThrowAsync(string because = "", params object[] becauseArgs)
    {
        try
        {
            await _action().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            AssertionHelpers.Fail($"Expected no exception to be thrown, but found {ex.GetType().Name}: \"{ex.Message}\".", because, becauseArgs);
        }
    }
}

/// <summary>
/// Chained assertions on a caught exception (message, inner exception, etc.).
/// </summary>
public readonly struct ExceptionAssertions<TException> where TException : Exception
{
    public TException Exception { get; }

    internal ExceptionAssertions(TException exception) => Exception = exception;

    public ExceptionAssertions<TException> WithMessage(string expectedMessage, string because = "", params object[] becauseArgs)
    {
        if (!string.Equals(Exception.Message, expectedMessage, StringComparison.Ordinal))
        {
            AssertionHelpers.Fail(
                $"Expected exception message to be \"{expectedMessage}\", but found \"{Exception.Message}\".",
                because, becauseArgs);
        }

        return this;
    }

    public ExceptionAssertions<TException> WithMessageContaining(string expectedSubstring, string because = "", params object[] becauseArgs)
    {
        if (Exception.Message.IndexOf(expectedSubstring, StringComparison.Ordinal) < 0)
        {
            AssertionHelpers.Fail(
                $"Expected exception message to contain \"{expectedSubstring}\", but found \"{Exception.Message}\".",
                because, becauseArgs);
        }

        return this;
    }

    public ExceptionAssertions<TException> WithInnerException<TInner>(string because = "", params object[] becauseArgs)
        where TInner : Exception
    {
        if (Exception.InnerException is not TInner)
        {
            var actual = Exception.InnerException?.GetType().Name ?? "null";
            AssertionHelpers.Fail($"Expected inner exception to be {typeof(TInner).Name}, but found {actual}.", because, becauseArgs);
        }

        return this;
    }
}
