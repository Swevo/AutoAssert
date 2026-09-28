using System.Runtime.CompilerServices;
using System.Text.Json;

namespace AutoAssert;

/// <summary>
/// Snapshot ("golden file") assertions: serializes the subject to JSON and compares it against a
/// baseline file stored next to the calling test file. The baseline is created automatically on
/// first run; subsequent runs fail if the serialized subject no longer matches it.
/// </summary>
public static class SnapshotAssertionExtensions
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    /// <summary>
    /// Compares <paramref name="assertions"/>'s subject (serialized as indented JSON) against a
    /// baseline snapshot file stored in a <c>__snapshots__</c> folder next to the calling test
    /// file. If no baseline exists yet, one is written and the assertion passes (first-run
    /// recording); otherwise the current value must match the stored baseline exactly.
    /// </summary>
    /// <param name="snapshotName">
    /// Optional discriminator when a single test method takes more than one snapshot
    /// (e.g. <c>MatchSnapshot("before")</c> / <c>MatchSnapshot("after")</c>).
    /// </param>
    public static AndConstraint<ObjectAssertions> MatchSnapshot(
        this ObjectAssertions assertions,
        string? snapshotName = null,
        string because = "",
        [CallerFilePath] string sourceFilePath = "",
        [CallerMemberName] string memberName = "",
        params object[] becauseArgs)
    {
        var actualJson = JsonSerializer.Serialize(assertions.Subject, SerializerOptions);
        var filePath = ResolveSnapshotFilePath(sourceFilePath, memberName, snapshotName);

        if (!File.Exists(filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            File.WriteAllText(filePath, actualJson);
            return new AndConstraint<ObjectAssertions>(assertions);
        }

        var expectedJson = File.ReadAllText(filePath);
        if (!string.Equals(Normalize(expectedJson), Normalize(actualJson), StringComparison.Ordinal))
        {
            AssertionHelpers.Fail(
                $"Snapshot '{Path.GetFileName(filePath)}' does not match.{Environment.NewLine}" +
                $"Expected (baseline):{Environment.NewLine}{expectedJson}{Environment.NewLine}" +
                $"Actual:{Environment.NewLine}{actualJson}{Environment.NewLine}" +
                $"If this change is intentional, delete '{filePath}' and re-run to record a new baseline.",
                because, becauseArgs);
        }

        return new AndConstraint<ObjectAssertions>(assertions);
    }

    private static string ResolveSnapshotFilePath(string sourceFilePath, string memberName, string? snapshotName)
    {
        var directory = Path.Combine(
            string.IsNullOrEmpty(sourceFilePath) ? "." : Path.GetDirectoryName(sourceFilePath) ?? ".",
            "__snapshots__");

        var fileName = string.IsNullOrEmpty(snapshotName)
            ? $"{memberName}.snapshot.json"
            : $"{memberName}.{snapshotName}.snapshot.json";

        return Path.Combine(directory, fileName);
    }

    private static string Normalize(string text) => text.Replace("\r\n", "\n").Trim();
}
