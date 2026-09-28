namespace AutoAssert;

/// <summary>
/// Global, process-wide defaults for AutoAssert. Configure once (e.g. in a test assembly's
/// module initializer or fixture setup) to apply the same <see cref="EquivalencyOptions"/> to
/// every <c>BeEquivalentTo</c> call that doesn't specify its own configuration — for example, to
/// always exclude an audit-tracking <c>Id</c>/<c>CreatedAt</c> member across an entire test suite.
/// </summary>
public static class AssertionConfig
{
    private static EquivalencyOptions _equivalencyDefaults = new();

    /// <summary>
    /// Replaces the global default <see cref="EquivalencyOptions"/> used by every
    /// <c>BeEquivalentTo(expected)</c> call that doesn't supply its own configuration. Per-call
    /// <c>BeEquivalentTo(expected, config =&gt; ...)</c> configuration is applied on top of (in
    /// addition to) these defaults, not instead of them.
    /// </summary>
    public static void ConfigureEquivalency(Action<EquivalencyOptions> configure)
    {
        var options = new EquivalencyOptions();
        configure(options);
        _equivalencyDefaults = options;
    }

    /// <summary>Restores the global equivalency defaults to their out-of-the-box (empty) state.</summary>
    public static void ResetEquivalencyDefaults() => _equivalencyDefaults = new EquivalencyOptions();

    /// <summary>Returns a fresh, independent copy of the current global equivalency defaults.</summary>
    internal static EquivalencyOptions CreateDefaultEquivalencyOptions() => _equivalencyDefaults.Clone();
}
