namespace AutoAssert;

/// <summary>
/// Collects assertion failures within its scope instead of throwing on the first one, then
/// throws a single combined <see cref="AssertionFailedException"/> listing every failure when
/// the scope is disposed. Mirrors FluentAssertions' <c>AssertionScope</c>.
/// </summary>
/// <example>
/// <code>
/// using (new AssertionScope())
/// {
///     1.Should().Be(2);
///     "a".Should().Be("b");
/// } // throws one exception listing both failures
/// </code>
/// </example>
public sealed class AssertionScope : IDisposable
{
    [ThreadStatic]
    private static AssertionScope? _current;

    private readonly AssertionScope? _parent;
    private readonly List<string> _failures = [];
    private bool _disposed;

    public AssertionScope()
    {
        _parent = _current;
        _current = this;
    }

    internal static AssertionScope? Current => _current;

    internal void AddFailure(string message) => _failures.Add(message);

    /// <summary>The failure messages recorded so far in this scope, without throwing.</summary>
    public IReadOnlyList<string> Failures => _failures;

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _current = _parent;

        if (_failures.Count == 0)
        {
            return;
        }

        var message = _failures.Count == 1
            ? _failures[0]
            : string.Join(Environment.NewLine, _failures.Select((failure, index) => $"{index + 1}) {failure}"));

        throw new AssertionFailedException(message);
    }
}
