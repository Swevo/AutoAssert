using System.Text.RegularExpressions;

namespace AutoAssert;

/// <summary>
/// Shared wildcard-pattern matching (<c>*</c> = any run of characters, <c>?</c> = any single
/// character) used by <see cref="StringAssertions.Match"/> and
/// <see cref="ExceptionAssertions{TException}.WithMessageMatching"/>.
/// </summary>
internal static class WildcardMatcher
{
    public static bool IsMatch(string value, string wildcardPattern)
    {
        var regexPattern = "^" + Regex.Escape(wildcardPattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        return Regex.IsMatch(value, regexPattern, RegexOptions.Singleline);
    }
}
