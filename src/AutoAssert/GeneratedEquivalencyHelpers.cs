namespace AutoAssert;

/// <summary>
/// Public helpers used by <c>AutoAssert.Generator</c>-emitted equivalency comparers to recurse
/// into complex/collection members and to format consistent failure messages, without needing
/// access to AutoAssert's internal reflection-based engine.
/// </summary>
public static class GeneratedEquivalencyHelpers
{
    /// <summary>
    /// Compares a single member value, recursing through the standard equivalency engine
    /// (registry-aware, so nested generated-comparer types are still reflection-free; falls back
    /// to reflection for any other complex type or collection). Appends any failures found.
    /// </summary>
    /// <remarks>
    /// Starts a fresh circular-reference tracking scope for this subtree rather than sharing the
    /// caller's — in the rare case of a circular reference that spans a generated-comparer
    /// boundary, this could recurse further than the pure-reflection engine would before
    /// detecting the cycle. This doesn't affect any acyclic object graph (the overwhelming
    /// majority of real-world DTOs compared via <c>BeEquivalentTo</c>).
    /// </remarks>
    public static bool CompareValue(object? actual, object? expected, string path, EquivalencyOptions options, List<string> failures) =>
        EquivalencyAssertions.TryCompare(actual, expected, path, [], options, failures);

    /// <summary>Formats a value consistently with the rest of AutoAssert's failure messages.</summary>
    public static string Format(object? value) => AssertionHelpers.Format(value);

    /// <summary>Appends a member name to a dotted member path, e.g. <c>AppendPath("Customer", "Name")</c> → <c>"Customer.Name"</c>.</summary>
    public static string AppendPath(string path, string memberName) =>
        string.IsNullOrEmpty(path) ? memberName : $"{path}.{memberName}";

    /// <summary>Builds a standard "expected X, found Y" mismatch message for a given member path.</summary>
    public static string BuildMismatchMessage(string path, object? expected, object? actual) =>
        string.IsNullOrEmpty(path)
            ? $"Expected value to be {Format(expected)}, but found {Format(actual)}."
            : $"Expected member '{path}' to be {Format(expected)}, but found {Format(actual)}.";
}
