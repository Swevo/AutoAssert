namespace AutoAssert;

/// <summary>
/// Assertions for <see cref="IEnumerable{T}"/> collections.
/// </summary>
public readonly struct CollectionAssertions<TItem>
{
    private readonly IEnumerable<TItem>? _subject;

    internal CollectionAssertions(IEnumerable<TItem>? subject) => _subject = subject;

    public void BeEmpty(string because = "", params object[] becauseArgs)
    {
        if (_subject is null || _subject.Any())
        {
            AssertionHelpers.Fail("Expected collection to be empty, but it was not.", because, becauseArgs);
        }
    }

    public void NotBeEmpty(string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !_subject.Any())
        {
            AssertionHelpers.Fail("Expected collection not to be empty, but it was.", because, becauseArgs);
        }
    }

    public void BeNull(string because = "", params object[] becauseArgs)
    {
        if (_subject is not null)
        {
            AssertionHelpers.Fail("Expected collection to be null, but it was not.", because, becauseArgs);
        }
    }

    public void NotBeNull(string because = "", params object[] becauseArgs)
    {
        if (_subject is null)
        {
            AssertionHelpers.Fail("Expected collection not to be null, but it was.", because, becauseArgs);
        }
    }

    public void HaveCount(int expected, string because = "", params object[] becauseArgs)
    {
        var actual = _subject?.Count() ?? 0;
        if (actual != expected)
        {
            AssertionHelpers.Fail($"Expected collection to have {expected} item(s), but found {actual}.", because, becauseArgs);
        }
    }

    public void ContainSingle(string because = "", params object[] becauseArgs)
    {
        var count = _subject?.Count() ?? 0;
        if (count != 1)
        {
            AssertionHelpers.Fail($"Expected collection to contain a single item, but found {count}.", because, becauseArgs);
        }
    }

    public void Contain(TItem expected, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !_subject.Contains(expected))
        {
            AssertionHelpers.Fail($"Expected collection to contain {AssertionHelpers.Format(expected)}, but it did not.", because, becauseArgs);
        }
    }

    public void NotContain(TItem unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject is not null && _subject.Contains(unexpected))
        {
            AssertionHelpers.Fail($"Expected collection not to contain {AssertionHelpers.Format(unexpected)}, but it did.", because, becauseArgs);
        }
    }

    /// <summary>Order-independent equivalence: same items, any order, duplicates counted.</summary>
    public void BeEquivalentTo(IEnumerable<TItem> expected, string because = "", params object[] becauseArgs)
    {
        var actualList = (_subject ?? Enumerable.Empty<TItem>()).ToList();
        var expectedList = expected.ToList();

        var actualCounts = actualList.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());
        var expectedCounts = expectedList.GroupBy(x => x).ToDictionary(g => g.Key, g => g.Count());

        var equivalent = actualCounts.Count == expectedCounts.Count &&
                         actualCounts.All(kv => expectedCounts.TryGetValue(kv.Key!, out var c) && c == kv.Value);

        if (!equivalent)
        {
            AssertionHelpers.Fail(
                $"Expected collection to be equivalent to [{string.Join(", ", expectedList.Select(x => AssertionHelpers.Format(x)))}], " +
                $"but found [{string.Join(", ", actualList.Select(x => AssertionHelpers.Format(x)))}].",
                because, becauseArgs);
        }
    }

    /// <summary>Order-dependent sequence equality.</summary>
    public void Equal(IEnumerable<TItem> expected, string because = "", params object[] becauseArgs)
    {
        var actualList = (_subject ?? Enumerable.Empty<TItem>()).ToList();
        var expectedList = expected.ToList();

        if (!actualList.SequenceEqual(expectedList))
        {
            AssertionHelpers.Fail(
                $"Expected collection to equal [{string.Join(", ", expectedList.Select(x => AssertionHelpers.Format(x)))}] in order, " +
                $"but found [{string.Join(", ", actualList.Select(x => AssertionHelpers.Format(x)))}].",
                because, becauseArgs);
        }
    }

    public void OnlyHaveUniqueItems(string because = "", params object[] becauseArgs)
    {
        var list = (_subject ?? Enumerable.Empty<TItem>()).ToList();
        if (list.Distinct().Count() != list.Count)
        {
            AssertionHelpers.Fail("Expected collection to only have unique items, but duplicates were found.", because, becauseArgs);
        }
    }

    public void AllSatisfy(Func<TItem, bool> predicate, string because = "", params object[] becauseArgs)
    {
        var list = (_subject ?? Enumerable.Empty<TItem>()).ToList();
        if (!list.All(predicate))
        {
            AssertionHelpers.Fail("Expected all items in the collection to satisfy the given predicate, but at least one did not.", because, becauseArgs);
        }
    }
}
