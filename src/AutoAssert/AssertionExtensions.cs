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
    public static NumericAssertions<uint> Should(this uint subject) => new(subject);
    public static NumericAssertions<ulong> Should(this ulong subject) => new(subject);
    public static NumericAssertions<ushort> Should(this ushort subject) => new(subject);
    public static NumericAssertions<sbyte> Should(this sbyte subject) => new(subject);

    public static CollectionAssertions<TItem> Should<TItem>(this IEnumerable<TItem>? subject) => new(subject);

    /// <summary>
    /// Note: this overload only applies to <see cref="IDictionary{TKey, TValue}"/> subjects specifically
    /// (it wins over the generic <see cref="IEnumerable{T}"/> overload because it is a more specific match).
    /// </summary>
    public static DictionaryAssertions<TKey, TValue> Should<TKey, TValue>(this IDictionary<TKey, TValue>? subject) where TKey : notnull => new(subject);

    public static ActionAssertions Should(this Action action) => new(action);

    public static FuncAssertions Should(this Func<Task> action) => new(action);

    /// <summary>Assertions for asynchronous, value-returning operations (e.g. <c>Func&lt;Task&lt;int&gt;&gt;</c>).</summary>
    public static FuncAssertions<TResult> Should<TResult>(this Func<Task<TResult>> action) => new(action);

    /// <summary>
    /// Assertions for synchronous, value-returning functions. Constrained to <c>struct</c> so it can never
    /// be ambiguous with <see cref="Should(Func{Task})"/> or <see cref="Should{TResult}(Func{Task{TResult}})"/>
    /// (both of which return reference types). Functions returning reference types should be wrapped in an
    /// <see cref="Action"/> instead, e.g. <c>(() => { Method(); }).Should()...</c>.
    /// </summary>
    public static ValueFuncAssertions<TResult> Should<TResult>(this Func<TResult> function) where TResult : struct => new(function);

    public static ObjectAssertions Should(this object? subject) => new(subject);

    public static DateTimeAssertions Should(this DateTime subject) => new(subject);

    public static DateTimeOffsetAssertions Should(this DateTimeOffset subject) => new(subject);

    public static TimeSpanAssertions Should(this TimeSpan subject) => new(subject);

    public static GuidAssertions Should(this Guid subject) => new(subject);

    public static EnumAssertions<TEnum> Should<TEnum>(this TEnum subject) where TEnum : struct, Enum => new(subject);

    // Nullable<T> overloads are intentionally limited to the concrete value types AutoAssert already knows
    // about (rather than a generic `Should<T>(this T? subject) where T : struct` catch-all), because such a
    // catch-all would win overload resolution over Should(this object? subject) for *any* other struct type
    // (Nullable<T> is a "better conversion target" than object), silently hijacking ObjectAssertions for
    // arbitrary custom structs that don't otherwise have a dedicated overload.
    public static NullableAssertions<bool> Should(this bool? subject) => new(subject);
    public static NullableAssertions<int> Should(this int? subject) => new(subject);
    public static NullableAssertions<long> Should(this long? subject) => new(subject);
    public static NullableAssertions<short> Should(this short? subject) => new(subject);
    public static NullableAssertions<byte> Should(this byte? subject) => new(subject);
    public static NullableAssertions<double> Should(this double? subject) => new(subject);
    public static NullableAssertions<float> Should(this float? subject) => new(subject);
    public static NullableAssertions<decimal> Should(this decimal? subject) => new(subject);
    public static NullableAssertions<uint> Should(this uint? subject) => new(subject);
    public static NullableAssertions<ulong> Should(this ulong? subject) => new(subject);
    public static NullableAssertions<ushort> Should(this ushort? subject) => new(subject);
    public static NullableAssertions<sbyte> Should(this sbyte? subject) => new(subject);
    public static NullableAssertions<DateTime> Should(this DateTime? subject) => new(subject);
    public static NullableAssertions<DateTimeOffset> Should(this DateTimeOffset? subject) => new(subject);
    public static NullableAssertions<TimeSpan> Should(this TimeSpan? subject) => new(subject);
    public static NullableAssertions<Guid> Should(this Guid? subject) => new(subject);
}
