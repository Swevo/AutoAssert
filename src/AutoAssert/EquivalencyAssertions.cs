using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AutoAssert;

internal static class EquivalencyAssertions
{
    private static readonly ConcurrentDictionary<Type, MemberDescriptor[]> MembersByType = new();

    public static void AssertEquivalent(object? actual, object? expected, string because, object[] becauseArgs)
    {
        if (!TryCompare(actual, expected, string.Empty, new HashSet<ObjectReferencePair>(), out var failureMessage))
        {
            AssertionHelpers.Fail(failureMessage, because, becauseArgs);
        }
    }

    private static bool TryCompare(object? actual, object? expected, string path, HashSet<ObjectReferencePair> visited, out string failureMessage)
    {
        if (ReferenceEquals(actual, expected))
        {
            failureMessage = string.Empty;
            return true;
        }

        if (actual is null || expected is null)
        {
            failureMessage = BuildValueMismatchMessage(path, expected, actual);
            return false;
        }

        var actualType = actual.GetType();
        var expectedType = expected.GetType();

        if (IsSimple(actualType) && IsSimple(expectedType))
        {
            if (Equals(actual, expected))
            {
                failureMessage = string.Empty;
                return true;
            }

            failureMessage = BuildValueMismatchMessage(path, expected, actual);
            return false;
        }

        if (IsEnumerable(actual) && IsEnumerable(expected))
        {
            return TryCompareEnumerables(ToObjectList((IEnumerable)actual), ToObjectList((IEnumerable)expected), path, visited, out failureMessage);
        }

        if (IsSimple(actualType) || IsSimple(expectedType))
        {
            failureMessage = BuildValueMismatchMessage(path, expected, actual);
            return false;
        }

        if (!actualType.IsValueType && !expectedType.IsValueType)
        {
            var pair = new ObjectReferencePair(actual, expected);
            if (!visited.Add(pair))
            {
                failureMessage = string.Empty;
                return true;
            }
        }

        return TryCompareMembers(actual, expected, path, visited, out failureMessage);
    }

    private static bool TryCompareMembers(object actual, object expected, string path, HashSet<ObjectReferencePair> visited, out string failureMessage)
    {
        var actualMembers = GetMembers(actual.GetType());
        var actualMembersByName = actualMembers.ToDictionary(member => member.Name, StringComparer.Ordinal);

        foreach (var expectedMember in GetMembers(expected.GetType()).OrderBy(member => member.Name, StringComparer.Ordinal))
        {
            if (!actualMembersByName.TryGetValue(expectedMember.Name, out var actualMember))
            {
                failureMessage = BuildMissingMemberMessage(AppendMemberPath(path, expectedMember.Name));
                return false;
            }

            var actualValue = actualMember.GetValue(actual);
            var expectedValue = expectedMember.GetValue(expected);

            if (!TryCompare(actualValue, expectedValue, AppendMemberPath(path, expectedMember.Name), visited, out failureMessage))
            {
                return false;
            }
        }

        failureMessage = string.Empty;
        return true;
    }

    private static bool TryCompareEnumerables(
        IReadOnlyList<object?> actualItems,
        IReadOnlyList<object?> expectedItems,
        string path,
        HashSet<ObjectReferencePair> visited,
        out string failureMessage)
    {
        if (actualItems.Count != expectedItems.Count)
        {
            failureMessage = BuildCountMismatchMessage(path, expectedItems.Count, actualItems.Count);
            return false;
        }

        if (expectedItems.Count == 0)
        {
            failureMessage = string.Empty;
            return true;
        }

        var matchedActual = new bool[actualItems.Count];
        return TryMatchCollectionItem(0, matchedActual, visited, out failureMessage);

        bool TryMatchCollectionItem(int expectedIndex, bool[] usedActual, HashSet<ObjectReferencePair> currentVisited, out string message)
        {
            if (expectedIndex == expectedItems.Count)
            {
                message = string.Empty;
                return true;
            }

            string? bestFailure = null;

            for (var actualIndex = 0; actualIndex < actualItems.Count; actualIndex++)
            {
                if (usedActual[actualIndex])
                {
                    continue;
                }

                var branchVisited = new HashSet<ObjectReferencePair>(currentVisited);
                if (!TryCompare(actualItems[actualIndex], expectedItems[expectedIndex], AppendIndexPath(path, expectedIndex), branchVisited, out var comparisonFailure))
                {
                    bestFailure ??= comparisonFailure;
                    continue;
                }

                usedActual[actualIndex] = true;

                if (TryMatchCollectionItem(expectedIndex + 1, usedActual, branchVisited, out message))
                {
                    return true;
                }

                usedActual[actualIndex] = false;
                bestFailure ??= message;
            }

            message = bestFailure ?? BuildMissingCollectionItemMessage(path, expectedIndex, expectedItems[expectedIndex]);
            return false;
        }
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
