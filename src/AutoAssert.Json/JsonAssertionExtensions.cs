using System.Text.Json;

namespace AutoAssert;

/// <summary>
/// JSON assertions on <see cref="StringAssertions"/> (i.e. <c>jsonString.Should()...</c>):
/// validity, presence/value of a property by path, and full structural equivalence.
/// </summary>
public static class JsonAssertionExtensions
{
    /// <summary>Asserts that the subject parses as valid JSON.</summary>
    public static AndConstraint<StringAssertions> BeValidJson(this StringAssertions assertions, string because = "", params object[] becauseArgs)
    {
        if (!TryParse(assertions.Subject, out _, out var parseError))
        {
            Fail($"Expected value to be valid JSON, but it was not: {parseError}", because, becauseArgs);
        }

        return new AndConstraint<StringAssertions>(assertions);
    }

    /// <summary>Asserts that a JSON property exists at <paramref name="path"/> (e.g. <c>"customer.address.city"</c> or <c>"items[0].id"</c>).</summary>
    public static AndConstraint<StringAssertions> HaveJsonProperty(this StringAssertions assertions, string path, string because = "", params object[] becauseArgs)
    {
        if (!TryParse(assertions.Subject, out var document, out var parseError))
        {
            Fail($"Expected value to be valid JSON, but it was not: {parseError}", because, becauseArgs);
            return new AndConstraint<StringAssertions>(assertions);
        }

        using (document)
        {
            if (!JsonPathNavigator.TryGetValue(document.RootElement, path, out _))
            {
                Fail($"Expected JSON to have a property at path '{path}', but it did not.", because, becauseArgs);
            }
        }

        return new AndConstraint<StringAssertions>(assertions);
    }

    /// <summary>Asserts that the JSON property at <paramref name="path"/> equals <paramref name="expectedValue"/>.</summary>
    public static AndConstraint<StringAssertions> HaveJsonPropertyEqualTo(this StringAssertions assertions, string path, object? expectedValue, string because = "", params object[] becauseArgs)
    {
        if (!TryParse(assertions.Subject, out var document, out var parseError))
        {
            Fail($"Expected value to be valid JSON, but it was not: {parseError}", because, becauseArgs);
            return new AndConstraint<StringAssertions>(assertions);
        }

        using (document)
        {
            if (!JsonPathNavigator.TryGetValue(document.RootElement, path, out var actualValue))
            {
                Fail($"Expected JSON to have a property at path '{path}', but it did not.", because, becauseArgs);
                return new AndConstraint<StringAssertions>(assertions);
            }

            var expectedElement = JsonSerializer.SerializeToElement(expectedValue);
            var diff = JsonComparer.Diff(expectedElement, actualValue, path);
            if (diff.Count > 0)
            {
                Fail($"Expected JSON property at '{path}' to be {JsonSerializer.Serialize(expectedValue)}, but found {actualValue.GetRawText()}.", because, becauseArgs);
            }
        }

        return new AndConstraint<StringAssertions>(assertions);
    }

    /// <summary>
    /// Asserts that the subject is structurally equivalent to <paramref name="expectedJson"/>:
    /// objects are compared property-by-property regardless of order, arrays element-by-element
    /// in order, and primitives by value. Reports every mismatch found, not just the first.
    /// </summary>
    public static AndConstraint<StringAssertions> BeEquivalentToJson(this StringAssertions assertions, string expectedJson, string because = "", params object[] becauseArgs)
    {
        if (!TryParse(assertions.Subject, out var actualDocument, out var actualParseError))
        {
            Fail($"Expected value to be valid JSON, but it was not: {actualParseError}", because, becauseArgs);
            return new AndConstraint<StringAssertions>(assertions);
        }

        using (actualDocument)
        {
            if (!TryParse(expectedJson, out var expectedDocument, out var expectedParseError))
            {
                Fail($"Expected JSON was not valid JSON: {expectedParseError}", because, becauseArgs);
                return new AndConstraint<StringAssertions>(assertions);
            }

            using (expectedDocument)
            {
                var diff = JsonComparer.Diff(expectedDocument.RootElement, actualDocument.RootElement);
                if (diff.Count > 0)
                {
                    Fail(
                        $"Expected JSON to be equivalent to the given value, but found {diff.Count} difference(s):{Environment.NewLine}" +
                        string.Join(Environment.NewLine, diff.Select(d => "- " + d)),
                        because, becauseArgs);
                }
            }
        }

        return new AndConstraint<StringAssertions>(assertions);
    }

    private static bool TryParse(string? json, out JsonDocument document, out string error)
    {
        try
        {
            document = JsonDocument.Parse(json ?? "null");
            error = "";
            return true;
        }
        catch (JsonException ex)
        {
            document = null!;
            error = ex.Message;
            return false;
        }
    }

    private static void Fail(string message, string because, object[] becauseArgs)
    {
        var reason = BuildReason(because, becauseArgs);
        throw new AssertionFailedException(message + reason);
    }

    private static string BuildReason(string because, object[] becauseArgs)
    {
        if (string.IsNullOrWhiteSpace(because))
        {
            return string.Empty;
        }

        var reason = becauseArgs is { Length: > 0 } ? string.Format(because, becauseArgs) : because;
        if (!reason.StartsWith("because", StringComparison.OrdinalIgnoreCase))
        {
            reason = "because " + reason;
        }

        return " " + reason;
    }
}
