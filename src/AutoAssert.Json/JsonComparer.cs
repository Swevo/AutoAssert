using System.Text.Json;

namespace AutoAssert;

/// <summary>
/// Structural JSON comparison used by <see cref="JsonAssertionExtensions.BeEquivalentToJson"/>.
/// Objects are compared property-by-property regardless of order; arrays are compared
/// element-by-element in order; primitives (numbers, strings, booleans, null) are compared by
/// value. Produces a full diff (every mismatch, not just the first) similar in spirit to
/// AutoAssert's core <c>BeEquivalentTo</c>.
/// </summary>
internal static class JsonComparer
{
    public static List<string> Diff(JsonElement expected, JsonElement actual, string path = "$")
    {
        var failures = new List<string>();
        Compare(expected, actual, path, failures);
        return failures;
    }

    private static void Compare(JsonElement expected, JsonElement actual, string path, List<string> failures)
    {
        if (expected.ValueKind != actual.ValueKind)
        {
            failures.Add($"At '{path}': expected {Describe(expected)}, but found {Describe(actual)}.");
            return;
        }

        switch (expected.ValueKind)
        {
            case JsonValueKind.Object:
                CompareObjects(expected, actual, path, failures);
                break;
            case JsonValueKind.Array:
                CompareArrays(expected, actual, path, failures);
                break;
            case JsonValueKind.String:
                if (expected.GetString() != actual.GetString())
                {
                    failures.Add($"At '{path}': expected {Describe(expected)}, but found {Describe(actual)}.");
                }

                break;
            case JsonValueKind.Number:
                if (expected.GetRawText() != actual.GetRawText() && expected.GetDouble() != actual.GetDouble())
                {
                    failures.Add($"At '{path}': expected {Describe(expected)}, but found {Describe(actual)}.");
                }

                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                // Value fully captured by ValueKind for these cases; nothing further to compare.
                break;
        }
    }

    private static void CompareObjects(JsonElement expected, JsonElement actual, string path, List<string> failures)
    {
        var actualProperties = actual.EnumerateObject().ToDictionary(p => p.Name, p => p.Value);
        var seen = new HashSet<string>();

        foreach (var expectedProperty in expected.EnumerateObject())
        {
            seen.Add(expectedProperty.Name);
            var memberPath = $"{path}.{expectedProperty.Name}";

            if (!actualProperties.TryGetValue(expectedProperty.Name, out var actualValue))
            {
                failures.Add($"At '{memberPath}': expected property to be present, but it was missing.");
                continue;
            }

            Compare(expectedProperty.Value, actualValue, memberPath, failures);
        }

        foreach (var extra in actualProperties.Keys.Except(seen))
        {
            failures.Add($"At '{path}.{extra}': unexpected property found (not present in expected JSON).");
        }
    }

    private static void CompareArrays(JsonElement expected, JsonElement actual, string path, List<string> failures)
    {
        var expectedItems = expected.EnumerateArray().ToList();
        var actualItems = actual.EnumerateArray().ToList();

        if (expectedItems.Count != actualItems.Count)
        {
            failures.Add($"At '{path}': expected an array with {expectedItems.Count} item(s), but found {actualItems.Count}.");
        }

        for (var i = 0; i < Math.Min(expectedItems.Count, actualItems.Count); i++)
        {
            Compare(expectedItems[i], actualItems[i], $"{path}[{i}]", failures);
        }
    }

    private static string Describe(JsonElement element) => element.ValueKind switch
    {
        JsonValueKind.String => $"\"{element.GetString()}\"",
        JsonValueKind.Null => "null",
        JsonValueKind.Undefined => "<undefined>",
        _ => element.GetRawText()
    };
}
