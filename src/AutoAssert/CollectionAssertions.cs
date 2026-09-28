namespace AutoAssert;

/// <summary>
/// Assertions for <see cref="IEnumerable{T}"/> collections.
/// </summary>
public readonly struct CollectionAssertions<TItem>
{
    private readonly IEnumerable<TItem>? _subject;

    internal CollectionAssertions(IEnumerable<TItem>? subject) => _subject = subject;

    public AndConstraint<CollectionAssertions<TItem>> BeEmpty(string because = "", params object[] becauseArgs)
    {
        if (_subject is null || _subject.Any())
        {
            AssertionHelpers.Fail("Expected collection to be empty, but it was not.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> NotBeEmpty(string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !_subject.Any())
        {
            AssertionHelpers.Fail("Expected collection not to be empty, but it was.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> BeNull(string because = "", params object[] becauseArgs)
    {
        if (_subject is not null)
        {
            AssertionHelpers.Fail("Expected collection to be null, but it was not.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> NotBeNull(string because = "", params object[] becauseArgs)
    {
        if (_subject is null)
        {
            AssertionHelpers.Fail("Expected collection not to be null, but it was.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> HaveCount(int expected, string because = "", params object[] becauseArgs)
    {
        var actual = _subject?.Count() ?? 0;
        if (actual != expected)
        {
            AssertionHelpers.Fail($"Expected collection to have {expected} item(s), but found {actual}.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> HaveCountGreaterThan(int expected, string because = "", params object[] becauseArgs)
    {
        var actual = _subject?.Count() ?? 0;
        if (actual <= expected)
        {
            AssertionHelpers.Fail($"Expected collection to have more than {expected} item(s), but found {actual}.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> HaveCountLessThan(int expected, string because = "", params object[] becauseArgs)
    {
        var actual = _subject?.Count() ?? 0;
        if (actual >= expected)
        {
            AssertionHelpers.Fail($"Expected collection to have fewer than {expected} item(s), but found {actual}.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> ContainSingle(string because = "", params object[] becauseArgs)
    {
        var count = _subject?.Count() ?? 0;
        if (count != 1)
        {
            AssertionHelpers.Fail($"Expected collection to contain a single item, but found {count}.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> Contain(TItem expected, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !_subject.Contains(expected))
        {
            AssertionHelpers.Fail($"Expected collection to contain {AssertionHelpers.Format(expected)}, but it did not.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> Contain(Func<TItem, bool> predicate, string because = "", params object[] becauseArgs)
    {
        if (_subject is null || !_subject.Any(predicate))
        {
            AssertionHelpers.Fail("Expected collection to contain an item matching the given predicate, but it did not.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> NotContain(TItem unexpected, string because = "", params object[] becauseArgs)
    {
        if (_subject is not null && _subject.Contains(unexpected))
        {
            AssertionHelpers.Fail($"Expected collection not to contain {AssertionHelpers.Format(unexpected)}, but it did.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> NotContain(Func<TItem, bool> predicate, string because = "", params object[] becauseArgs)
    {
        if (_subject is not null && _subject.Any(predicate))
        {
            AssertionHelpers.Fail("Expected collection not to contain an item matching the given predicate, but it did.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    /// <summary>Asserts that <paramref name="expectedSubsequence"/> appears in the collection, in that relative order (other items may be interspersed).</summary>
    public AndConstraint<CollectionAssertions<TItem>> ContainInOrder(IEnumerable<TItem> expectedSubsequence, string because = "", params object[] becauseArgs)
    {
        var actualList = (_subject ?? []).ToList();
        var expectedList = expectedSubsequence.ToList();

        var searchFrom = 0;
        foreach (var expectedItem in expectedList)
        {
            var foundIndex = actualList.FindIndex(searchFrom, item => Equals(item, expectedItem));
            if (foundIndex < 0)
            {
                AssertionHelpers.Fail(
                    $"Expected collection to contain [{string.Join(", ", expectedList.Select(x => AssertionHelpers.Format(x)))}] in order, " +
                    $"but {AssertionHelpers.Format(expectedItem)} was not found in the expected relative order.",
                    because, becauseArgs);
                return new AndConstraint<CollectionAssertions<TItem>>(this);
            }

            searchFrom = foundIndex + 1;
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    /// <summary>Order-independent equivalence: same items, any order, duplicates counted.</summary>
    public AndConstraint<CollectionAssertions<TItem>> BeEquivalentTo(IEnumerable<TItem>? expected, string because = "", params object[] becauseArgs)
    {
        EquivalencyAssertions.AssertEquivalent(_subject, expected, EquivalencyOptions.Default, because, becauseArgs);
        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    /// <summary>Equivalence with configurable options, e.g. <c>.WithStrictOrdering()</c> or <c>.Excluding(...)</c>.</summary>
    public AndConstraint<CollectionAssertions<TItem>> BeEquivalentTo(IEnumerable<TItem>? expected, Action<EquivalencyOptions> config, string because = "", params object[] becauseArgs)
    {
        var options = new EquivalencyOptions();
        config(options);
        EquivalencyAssertions.AssertEquivalent(_subject, expected, options, because, becauseArgs);
        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    /// <summary>Order-dependent sequence equality.</summary>
    public AndConstraint<CollectionAssertions<TItem>> Equal(IEnumerable<TItem> expected, string because = "", params object[] becauseArgs)
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

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> OnlyHaveUniqueItems(string because = "", params object[] becauseArgs)
    {
        var list = (_subject ?? Enumerable.Empty<TItem>()).ToList();
        if (list.Distinct().Count() != list.Count)
        {
            AssertionHelpers.Fail("Expected collection to only have unique items, but duplicates were found.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> AllSatisfy(Func<TItem, bool> predicate, string because = "", params object[] becauseArgs)
    {
        var list = (_subject ?? Enumerable.Empty<TItem>()).ToList();
        if (!list.All(predicate))
        {
            AssertionHelpers.Fail("Expected all items in the collection to satisfy the given predicate, but at least one did not.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    /// <summary>Asserts each item, at its corresponding index, satisfies its own inspector — item count must match inspector count.</summary>
    public AndConstraint<CollectionAssertions<TItem>> SatisfyRespectively(params Action<TItem>[] inspectors)
    {
        var list = (_subject ?? Enumerable.Empty<TItem>()).ToList();
        if (list.Count != inspectors.Length)
        {
            AssertionHelpers.Fail($"Expected collection to contain exactly {inspectors.Length} item(s) to satisfy each inspector, but found {list.Count}.", "", []);
            return new AndConstraint<CollectionAssertions<TItem>>(this);
        }

        for (var i = 0; i < list.Count; i++)
        {
            try
            {
                inspectors[i](list[i]);
            }
            catch (Exception ex) when (ex is not AssertionFailedException)
            {
                AssertionHelpers.Fail($"Expected item at index {i} to satisfy its inspector, but it threw: {ex.Message}", "", []);
            }
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> BeInAscendingOrder(string because = "", params object[] becauseArgs)
    {
        var list = (_subject ?? Enumerable.Empty<TItem>()).ToList();
        var sorted = list.OrderBy(x => x, Comparer<TItem>.Default).ToList();
        if (!list.SequenceEqual(sorted))
        {
            AssertionHelpers.Fail("Expected collection to be in ascending order, but it was not.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }

    public AndConstraint<CollectionAssertions<TItem>> BeInDescendingOrder(string because = "", params object[] becauseArgs)
    {
        var list = (_subject ?? Enumerable.Empty<TItem>()).ToList();
        var sorted = list.OrderByDescending(x => x, Comparer<TItem>.Default).ToList();
        if (!list.SequenceEqual(sorted))
        {
            AssertionHelpers.Fail("Expected collection to be in descending order, but it was not.", because, becauseArgs);
        }

        return new AndConstraint<CollectionAssertions<TItem>>(this);
    }
}
