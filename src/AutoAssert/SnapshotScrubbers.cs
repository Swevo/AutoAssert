using System.Text.RegularExpressions;

namespace AutoAssert;

/// <summary>
/// Composable text normalizers ("scrubbers") for use with <see cref="SnapshotAssertionExtensions.MatchSnapshot"/>,
/// so snapshots stay stable across runs even when the serialized subject contains
/// non-deterministic values like generated GUIDs or "now"-based timestamps.
/// </summary>
public static class SnapshotScrubbers
{
    private static readonly Regex GuidPattern = new(
        @"\b[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}\b",
        RegexOptions.Compiled);

    private static readonly Regex IsoTimestampPattern = new(
        @"\b\d{4}-\d{2}-\d{2}[T ]\d{2}:\d{2}:\d{2}(\.\d+)?(Z|[+-]\d{2}:?\d{2})?\b",
        RegexOptions.Compiled);

    /// <summary>Replaces every GUID-looking value with a fixed placeholder.</summary>
    public static Func<string, string> Guids(string placeholder = "<guid>") =>
        text => GuidPattern.Replace(text, placeholder);

    /// <summary>Replaces every ISO-8601-looking date/time value with a fixed placeholder.</summary>
    public static Func<string, string> IsoTimestamps(string placeholder = "<timestamp>") =>
        text => IsoTimestampPattern.Replace(text, placeholder);

    /// <summary>Replaces every match of a custom regular expression with a fixed placeholder.</summary>
    public static Func<string, string> Pattern(string regex, string placeholder) =>
        text => Regex.Replace(text, regex, placeholder);

    /// <summary>Applies several scrubbers in sequence.</summary>
    public static Func<string, string> Combine(params Func<string, string>[] scrubbers) =>
        text => scrubbers.Aggregate(text, (current, scrubber) => scrubber(current));
}
