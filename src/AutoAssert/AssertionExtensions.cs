namespace AutoAssert;

/// <summary>
/// Entry points for AutoAssert's fluent assertion syntax. Mirrors the common
/// FluentAssertions <c>Should()</c> surface so existing test suites can migrate
/// with minimal changes.
/// </summary>
public static class AssertionExtensions
{
    public static BooleanAssertions Should(this bool subject) => new(subject);

    public static StringAssertions Should(this string? subject) => new(subject);

    public static NumericAssertions<int> Should(this int subject) => new(subject);
    public static NumericAssertions<long> Should(this long subject) => new(subject);
    public static NumericAssertions<short> Should(this short subject) => new(subject);
    public static NumericAssertions<byte> Should(this byte subject) => new(subject);
    public static NumericAssertions<double> Should(this double subject) => new(subject);
    public static NumericAssertions<float> Should(this float subject) => new(subject);
    public static NumericAssertions<decimal> Should(this decimal subject) => new(subject);

    public static CollectionAssertions<TItem> Should<TItem>(this IEnumerable<TItem>? subject) => new(subject);

    public static ActionAssertions Should(this Action action) => new(action);

    public static FuncAssertions Should(this Func<Task> action) => new(action);

    public static ObjectAssertions Should(this object? subject) => new(subject);
}
