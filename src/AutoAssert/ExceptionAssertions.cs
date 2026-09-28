namespace AutoAssert;

/// <summary>
/// Assertions for synchronous actions expected to throw (or not throw) exceptions.
/// </summary>
public readonly struct ActionAssertions
{
    private readonly Action _action;

    internal ActionAssertions(Action action) => _action = action;

    internal TimeSpan MeasureExecutionTime()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        _action();
        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

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
        return default!; // unreachable outside an AssertionScope: AssertionHelpers.Fail always throws there
    }

    public AndConstraint<ActionAssertions> NotThrow(string because = "", params object[] becauseArgs)
    {
        try
        {
            _action();
        }
        catch (Exception ex)
        {
            AssertionHelpers.Fail($"Expected no exception to be thrown, but found {ex.GetType().Name}: \"{ex.Message}\".", because, becauseArgs);
        }

        return new AndConstraint<ActionAssertions>(this);
    }

    /// <summary>Asserts a specific exception type is not thrown; any other exception (or none) is fine.</summary>
    public AndConstraint<ActionAssertions> NotThrow<TException>(string because = "", params object[] becauseArgs)
        where TException : Exception
    {
        try
        {
            _action();
        }
        catch (TException ex)
        {
            AssertionHelpers.Fail($"Expected no {typeof(TException).Name} to be thrown, but found: \"{ex.Message}\".", because, becauseArgs);
        }

        return new AndConstraint<ActionAssertions>(this);
    }
}

/// <summary>
/// Assertions for synchronous, value-returning functions expected to throw (or not throw).
/// Constrained to value types to avoid resolving ambiguously against the <c>Func&lt;Task&gt;</c>/
/// <c>Func&lt;Task&lt;TResult&gt;&gt;</c> overloads (which are reference types); a function
/// returning a reference type can be asserted for throwing by wrapping it as an <see cref="Action"/>,
/// e.g. <c>((Action)(() => function())).Should().Throw&lt;T&gt;()</c>.
/// </summary>
public readonly struct ValueFuncAssertions<TResult> where TResult : struct
{
    private readonly Func<TResult> _function;

    internal ValueFuncAssertions(Func<TResult> function) => _function = function;

    public ExceptionAssertions<TException> Throw<TException>(string because = "", params object[] becauseArgs)
        where TException : Exception
    {
        try
        {
            _function();
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
        return default!;
    }

    /// <summary>Asserts the function does not throw, and returns its resolved value.</summary>
    public TResult NotThrow(string because = "", params object[] becauseArgs)
    {
        try
        {
            return _function();
        }
        catch (Exception ex)
        {
            AssertionHelpers.Fail($"Expected no exception to be thrown, but found {ex.GetType().Name}: \"{ex.Message}\".", because, becauseArgs);
            return default;
        }
    }
}

/// <summary>
/// Assertions for asynchronous, void-returning operations expected to throw (or not throw) exceptions.
/// </summary>
public readonly struct FuncAssertions
{
    private readonly Func<Task> _action;

    internal FuncAssertions(Func<Task> action) => _action = action;

    internal Task InvokeAsync() => _action();

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
        return default!;
    }

    public async Task<AndConstraint<FuncAssertions>> NotThrowAsync(string because = "", params object[] becauseArgs)
    {
        try
        {
            await _action().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            AssertionHelpers.Fail($"Expected no exception to be thrown, but found {ex.GetType().Name}: \"{ex.Message}\".", because, becauseArgs);
        }

        return new AndConstraint<FuncAssertions>(this);
    }
}

/// <summary>
/// Assertions for asynchronous, value-returning operations expected to throw (or not throw) exceptions.
/// </summary>
public readonly struct FuncAssertions<TResult>
{
    private readonly Func<Task<TResult>> _function;

    internal FuncAssertions(Func<Task<TResult>> function) => _function = function;

    public async Task<ExceptionAssertions<TException>> ThrowAsync<TException>(string because = "", params object[] becauseArgs)
        where TException : Exception
    {
        try
        {
            await _function().ConfigureAwait(false);
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
        return default!;
    }

    /// <summary>Asserts the function does not throw, and returns its resolved value.</summary>
    public async Task<TResult> NotThrowAsync(string because = "", params object[] becauseArgs)
    {
        try
        {
            return await _function().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            AssertionHelpers.Fail($"Expected no exception to be thrown, but found {ex.GetType().Name}: \"{ex.Message}\".", because, becauseArgs);
            return default!;
        }
    }
}

/// <summary>
/// Chained assertions on a caught exception (message, inner exception, predicate, etc.).
/// </summary>
public readonly struct ExceptionAssertions<TException> where TException : Exception
{
    public TException Exception { get; }

    internal ExceptionAssertions(TException exception) => Exception = exception;

    /// <summary>Continuation property allowing <c>.And.WithXxx(...)</c> for readability; equivalent to chaining directly.</summary>
    public ExceptionAssertions<TException> And => this;

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

    /// <summary>
    /// Asserts the exception message matches a wildcard pattern (<c>*</c> matches any run of
    /// characters, <c>?</c> matches any single character) — mirrors FluentAssertions'
    /// <c>WithMessage</c> wildcard behavior for message assertions that shouldn't require an
    /// exact string (e.g. matching a message containing a dynamic id).
    /// </summary>
    public ExceptionAssertions<TException> WithMessageMatching(string wildcardPattern, string because = "", params object[] becauseArgs)
    {
        if (!WildcardMatcher.IsMatch(Exception.Message, wildcardPattern))
        {
            AssertionHelpers.Fail(
                $"Expected exception message to match wildcard pattern \"{wildcardPattern}\", but found \"{Exception.Message}\".",
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

    /// <summary>Asserts the caught exception satisfies an arbitrary predicate (e.g. checking a custom property).</summary>
    public ExceptionAssertions<TException> Where(Func<TException, bool> predicate, string because = "", params object[] becauseArgs)
    {
        if (!predicate(Exception))
        {
            AssertionHelpers.Fail($"Expected exception to match the given predicate, but it did not. Exception: \"{Exception.Message}\".", because, becauseArgs);
        }

        return this;
    }

    /// <summary>Asserts <see cref="ArgumentException.ParamName"/> matches the given parameter name (only applicable when the caught exception is an <see cref="ArgumentException"/>).</summary>
    public ExceptionAssertions<TException> WithParameterName(string expectedParameterName, string because = "", params object[] becauseArgs)
    {
        if (Exception is not ArgumentException argumentException || argumentException.ParamName != expectedParameterName)
        {
            var actual = (Exception as ArgumentException)?.ParamName ?? "<n/a>";
            AssertionHelpers.Fail($"Expected exception parameter name to be \"{expectedParameterName}\", but found \"{actual}\".", because, becauseArgs);
        }

        return this;
    }
}
