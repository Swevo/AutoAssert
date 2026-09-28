using System.Collections.Concurrent;

namespace AutoAssert;

/// <summary>
/// Signature for a compile-time-generated equivalency comparer (see <c>AutoAssert.Generator</c>'s
/// <c>[GenerateEquivalencyComparer]</c>). Compares <paramref name="actual"/> against
/// <paramref name="expected"/> (both guaranteed to be the exact registered type), appending a
/// failure message to <paramref name="failures"/> for every mismatch found, and returns whether
/// they were equivalent.
/// </summary>
public delegate bool GeneratedEquivalencyComparer(object actual, object expected, string path, EquivalencyOptions options, List<string> failures);

/// <summary>
/// Registry that <c>BeEquivalentTo</c> consults before falling back to reflection-based member
/// traversal. Generated comparers (emitted by the <c>AutoAssert.Generator</c> source generator via
/// <c>[GenerateEquivalencyComparer]</c>) register themselves here in a module initializer, so
/// referencing the generator package is enough — no manual wiring required.
/// </summary>
public static class GeneratedEquivalencyRegistry
{
    private static readonly ConcurrentDictionary<Type, GeneratedEquivalencyComparer> Comparers = new();

    /// <summary>Registers (or replaces) the generated comparer used for <paramref name="type"/>.</summary>
    public static void Register(Type type, GeneratedEquivalencyComparer comparer) => Comparers[type] = comparer;

    internal static bool TryGet(Type type, out GeneratedEquivalencyComparer? comparer) => Comparers.TryGetValue(type, out comparer);
}
