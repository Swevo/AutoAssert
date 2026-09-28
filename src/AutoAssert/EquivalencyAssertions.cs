using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AutoAssert;

internal static class EquivalencyAssertions
{
    private static readonly ConcurrentDictionary<Type, MemberDescriptor[]> MembersByType = new();

    public static void AssertEquivalent(object? actual, object? expected, EquivalencyOptions options, string because, object[] becauseArgs)
    {
        var failures = new List<string>();
        TryCompare(actual, expected, string.Empty, new HashSet<ObjectReferencePair>(), options, failures);

        if (failures.Count == 0)
        {
            return;
        }

        var message = failures.Count == 1
            ? failures[0]
            : $"Found {failures.Count} difference(s):" + string.Concat(failures.Select((failure, index) => Environment.NewLine + $"  {index + 1}) {failure}"));

        AssertionHelpers.Fail(message, because, becauseArgs);
    }

    private static bool TryCompare(object? actual, object? expected, string path, HashSet<ObjectReferencePair> visited, EquivalencyOptions options, List<string> failures)
    {
        if (ReferenceEquals(actual, expected))
        {
            return true;
        }

        if (actual is null || expected is null)
        {
            failures.Add(BuildValueMismatchMessage(path, expected, actual));
            return false;
        }

        var actualType = actual.GetType();
        var expectedType = expected.GetType();

        if (IsSimple(actualType) && IsSimple(expectedType))
        {
            if (Equals(actual, expected))
            {
                return true;
            }

            failures.Add(BuildValueMismatchMessage(path, expected, actual));
            return false;
        }

        if (IsEnumerable(actual) && IsEnumerable(expected))
        {
            return TryCompareEnumerables(ToObjectList((IEnumerable)actual), ToObjectList((IEnumerable)expected), path, visited, options, failures);
        }

        if (IsSimple(actualType) || IsSimple(expectedType))
        {
            failures.Add(BuildValueMismatchMessage(path, expected, actual));
            return false;
        }

        if (!actualType.IsValueType && !expectedType.IsValueType)
        {
            var pair = new ObjectReferencePair(actual, expected);
            if (!visited.Add(pair))
            {
                return true;
            }
        }

        return TryCompareMembers(actual, expected, path, visited, options, failures);
    }

    /// <summary>
    /// Compares every (non-excluded) member of <paramref name="expected"/>'s type, collecting a
    /// failure for *every* mismatching member instead of stopping at the first one — so a single
    /// <c>BeEquivalentTo</c> failure reports the full diff, not just the first difference found.
    /// </summary>
    private static bool TryCompareMembers(object actual, object expected, string path, HashSet<ObjectReferencePair> visited, EquivalencyOptions options, List<string> failures)
    {
        var actualMembersByName = GetMembers(actual.GetType()).ToDictionary(member => member.Name, StringComparer.Ordinal);
        var ok = true;

        foreach (var expectedMember in GetMembers(expected.GetType()).OrderBy(member => member.Name, StringComparer.Ordinal))
        {
            if (options.IsExcluded(expectedMember.Name))
            {
                continue;
            }

            if (!actualMembersByName.TryGetValue(expectedMember.Name, out var actualMember))
            {
                failures.Add(BuildMissingMemberMessage(AppendMemberPath(path, expectedMember.Name)));
                ok = false;
                continue;
            }

            var actualValue = actualMember.GetValue(actual);
            var expectedValue = expectedMember.GetValue(expected);

            if (!TryCompare(actualValue, expectedValue, AppendMemberPath(path, expectedMember.Name), visited, options, failures))
            {
                ok = false;
            }
        }

        return ok;
    }

    private static bool TryCompareEnumerables(
        IReadOnlyList<object?> actualItems,
        IReadOnlyList<object?> expectedItems,
        string path,
        HashSet<ObjectReferencePair> visited,
        EquivalencyOptions options,
        List<string> failures)
    {
        if (actualItems.Count != expectedItems.Count)
        {
            failures.Add(BuildCountMismatchMessage(path, expectedItems.Count, actualItems.Count));
            return false;
        }

        if (expectedItems.Count == 0)
        {
            return true;
        }

        if (options.StrictOrdering)
        {
            var ok = true;
            for (var i = 0; i < expectedItems.Count; i++)
            {
                if (!TryCompare(actualItems[i], expectedItems[i], AppendIndexPath(path, i), visited, options, failures))
                {
                    ok = false;
                }
            }

            return ok;
        }

        var matchedActual = new bool[actualItems.Count];
        if (TryMatchCollectionItem(0, matchedActual, visited, out _))
        {
            return true;
        }

        // No perfect one-to-one matching exists. Rather than reporting only the first blocking
        // mismatch (as a strict backtracking-failure trace would), greedily pair each expected
        // item with whichever remaining actual item has the fewest member-level differences, and
        // report the full diff for every expected item. This isn't guaranteed globally optimal
        // (unlike the success-path backtracking search), but it surfaces every mismatch instead
        // of stopping at the first, which matters far more for diagnosing a failing assertion.
        failures.AddRange(BuildGreedyFullDiff(actualItems, expectedItems, path, visited, options));
        return false;

        bool TryMatchCollectionItem(int expectedIndex, bool[] usedActual, HashSet<ObjectReferencePair> currentVisited, out List<string> resultFailures)
        {
            if (expectedIndex == expectedItems.Count)
            {
                resultFailures = [];
                return true;
            }

            for (var actualIndex = 0; actualIndex < actualItems.Count; actualIndex++)
            {
                if (usedActual[actualIndex])
                {
                    continue;
                }

                var branchVisited = new HashSet<ObjectReferencePair>(currentVisited);
                var attemptFailures = new List<string>();
                if (!TryCompare(actualItems[actualIndex], expectedItems[expectedIndex], AppendIndexPath(path, expectedIndex), branchVisited, options, attemptFailures))
                {
                    continue;
                }

                usedActual[actualIndex] = true;

                if (TryMatchCollectionItem(expectedIndex + 1, usedActual, branchVisited, out resultFailures))
                {
                    return true;
                }

                usedActual[actualIndex] = false;
            }

            resultFailures = [];
            return false;
        }
    }

    /// <summary>
    /// Greedily pairs each expected item with the remaining actual item that best matches it
    /// (fewest member-level differences), and returns the full diff for every expected item —
    /// used only for reporting once we already know no perfect matching exists.
    /// </summary>
    private static List<string> BuildGreedyFullDiff(
        IReadOnlyList<object?> actualItems,
        IReadOnlyList<object?> expectedItems,
        string path,
        HashSet<ObjectReferencePair> visited,
        EquivalencyOptions options)
    {
        var results = new List<string>();
        var usedActual = new bool[actualItems.Count];

        for (var expectedIndex = 0; expectedIndex < expectedItems.Count; expectedIndex++)
        {
            var itemPath = AppendIndexPath(path, expectedIndex);
            var bestActualIndex = -1;
            List<string>? bestFailures = null;

            for (var actualIndex = 0; actualIndex < actualItems.Count; actualIndex++)
            {
                if (usedActual[actualIndex])
                {
                    continue;
                }

                var attemptFailures = new List<string>();
                TryCompare(actualItems[actualIndex], expectedItems[expectedIndex], itemPath, new HashSet<ObjectReferencePair>(visited), options, attemptFailures);

                if (bestFailures is null || attemptFailures.Count < bestFailures.Count)
                {
                    bestActualIndex = actualIndex;
                    bestFailures = attemptFailures;

                    if (attemptFailures.Count == 0)
                    {
                        break;
                    }
                }
            }

            if (bestActualIndex < 0)
            {
                results.Add(BuildMissingCollectionItemMessage(path, expectedIndex, expectedItems[expectedIndex]));
                continue;
            }

            usedActual[bestActualIndex] = true;
            results.AddRange(bestFailures!);
        }

        return results;
    }

    private static MemberDescriptor[] GetMembers(Type type)
    {
        return MembersByType.GetOrAdd(type, static t =>
        {
            var properties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(property => property.CanRead && property.GetIndexParameters().Length == 0)
                .Select(property => new MemberDescriptor(property.Name, property))
                .ToArray();

            var fields = t.GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Select(field => new MemberDescriptor(field.Name, field))
                .ToArray();

            return properties.Concat(fields)
                .GroupBy(member => member.Name, StringComparer.Ordinal)
                .Select(group => group.First())
                .ToArray();
        });
    }

    private static List<object?> ToObjectList(IEnumerable values)
    {
        var items = new List<object?>();
        foreach (var value in values)
        {
            items.Add(value);
        }

        return items;
    }

    private static bool IsEnumerable(object value) => value is IEnumerable && value is not string;

    private static bool IsSimple(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

        if (underlyingType.IsPrimitive || underlyingType.IsEnum)
        {
            return true;
        }

        return underlyingType == typeof(string)
            || underlyingType == typeof(decimal)
            || underlyingType == typeof(DateTime)
            || underlyingType == typeof(DateTimeOffset)
            || underlyingType == typeof(TimeSpan)
            || underlyingType == typeof(Guid)
            || underlyingType.IsValueType;
    }

    private static string AppendMemberPath(string path, string memberName) =>
        string.IsNullOrEmpty(path) ? memberName : $"{path}.{memberName}";

    private static string AppendIndexPath(string path, int index) =>
        string.IsNullOrEmpty(path) ? $"[{index}]" : $"{path}[{index}]";

    private static string BuildValueMismatchMessage(string path, object? expected, object? actual)
    {
        return string.IsNullOrEmpty(path)
            ? $"Expected value to be {AssertionHelpers.Format(expected)}, but found {AssertionHelpers.Format(actual)}."
            : $"Expected member '{path}' to be {AssertionHelpers.Format(expected)}, but found {AssertionHelpers.Format(actual)}.";
    }

    private static string BuildMissingMemberMessage(string path) =>
        $"Expected member '{path}' to exist on the actual value, but it was missing.";

    private static string BuildCountMismatchMessage(string path, int expectedCount, int actualCount)
    {
        return string.IsNullOrEmpty(path)
            ? $"Expected collection to contain {expectedCount} item(s), but found {actualCount}."
            : $"Expected member '{path}' to contain {expectedCount} item(s), but found {actualCount}.";
    }

    private static string BuildMissingCollectionItemMessage(string path, int index, object? expectedItem)
    {
        var itemPath = AppendIndexPath(path, index);
        return $"Expected member '{itemPath}' to match {AssertionHelpers.Format(expectedItem)}, but no equivalent item was found.";
    }

    private sealed class MemberDescriptor
    {
        private readonly PropertyInfo? _property;
        private readonly FieldInfo? _field;

        public MemberDescriptor(string name, PropertyInfo property)
        {
            Name = name;
            _property = property;
        }

        public MemberDescriptor(string name, FieldInfo field)
        {
            Name = name;
            _field = field;
        }

        public string Name { get; }

        public object? GetValue(object instance)
        {
            if (_property is not null)
            {
                return _property.GetValue(instance);
            }

            return _field!.GetValue(instance);
        }
    }

    private readonly struct ObjectReferencePair : IEquatable<ObjectReferencePair>
    {
        private readonly object _actual;
        private readonly object _expected;

        public ObjectReferencePair(object actual, object expected)
        {
            _actual = actual;
            _expected = expected;
        }

        public bool Equals(ObjectReferencePair other) =>
            ReferenceEquals(_actual, other._actual) && ReferenceEquals(_expected, other._expected);

        public override bool Equals(object? obj) => obj is ObjectReferencePair other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (RuntimeHelpers.GetHashCode(_actual) * 397) ^ RuntimeHelpers.GetHashCode(_expected);
            }
        }
    }
}
