using Xunit;

namespace AutoAssert.Tests;

public class BooleanAssertionsTests
{
    [Fact]
    public void BeTrue_Passes_When_True() => true.Should().BeTrue();

    [Fact]
    public void BeTrue_Fails_When_False() =>
        Assert.Throws<AssertionFailedException>(() => false.Should().BeTrue());

    [Fact]
    public void BeFalse_Passes_When_False() => false.Should().BeFalse();

    [Fact]
    public void BeFalse_Fails_When_True() =>
        Assert.Throws<AssertionFailedException>(() => true.Should().BeFalse());
}

public class ObjectAssertionsTests
{
    [Fact]
    public void Be_Passes_When_Equal() => 5.Should().Be(5);

    [Fact]
    public void Be_Fails_When_NotEqual() =>
        Assert.Throws<AssertionFailedException>(() => "a".Should().Be("b"));

    [Fact]
    public void NotBeNull_Passes_When_NotNull() => new object().Should().NotBeNull();

    [Fact]
    public void NotBeNull_Fails_When_Null()
    {
        object? subject = null;
        Assert.Throws<AssertionFailedException>(() => subject.Should().NotBeNull());
    }

    [Fact]
    public void BeNull_Passes_When_Null()
    {
        object? subject = null;
        subject.Should().BeNull();
    }

    [Fact]
    public void BeOfType_Passes_For_Matching_Type() => ((object)"hello").Should().BeOfType<string>();

    [Fact]
    public void BeOfType_Fails_For_Mismatched_Type() =>
        Assert.Throws<AssertionFailedException>(() => ((object)"hello").Should().BeOfType<int>());

    [Fact]
    public void Match_Passes_When_Predicate_True() => ((object)10).Should().Match(x => (int)x! > 5);

    [Fact]
    public void Match_Fails_When_Predicate_False() =>
        Assert.Throws<AssertionFailedException>(() => ((object)10).Should().Match(x => (int)x! > 50));

    [Fact]
    public void Because_Reason_Is_Appended_To_Failure_Message()
    {
        var ex = Assert.Throws<AssertionFailedException>(() => 1.Should().Be(2, "the answer must match"));
        Assert.Contains("because the answer must match", ex.Message);
    }
}

public class StringAssertionsTests
{
    [Fact]
    public void Be_Passes_When_Equal() => "abc".Should().Be("abc");

    [Fact]
    public void Contain_Passes_When_Substring_Present() => "hello world".Should().Contain("world");

    [Fact]
    public void Contain_Fails_When_Substring_Absent() =>
        Assert.Throws<AssertionFailedException>(() => "hello world".Should().Contain("xyz"));

    [Fact]
    public void StartWith_Passes() => "hello world".Should().StartWith("hello");

    [Fact]
    public void EndWith_Passes() => "hello world".Should().EndWith("world");

    [Fact]
    public void BeNullOrEmpty_Passes_For_Empty() => "".Should().BeNullOrEmpty();

    [Fact]
    public void BeNullOrEmpty_Passes_For_Null()
    {
        string? subject = null;
        subject.Should().BeNullOrEmpty();
    }

    [Fact]
    public void HaveLength_Passes() => "abc".Should().HaveLength(3);

    [Fact]
    public void HaveLength_Fails() =>
        Assert.Throws<AssertionFailedException>(() => "abc".Should().HaveLength(4));

    [Fact]
    public void MatchRegex_Passes() => "abc123".Should().MatchRegex(@"^[a-z]+\d+$");
}

public class NumericAssertionsTests
{
    [Fact]
    public void BeGreaterThan_Passes() => 10.Should().BeGreaterThan(5);

    [Fact]
    public void BeGreaterThan_Fails() =>
        Assert.Throws<AssertionFailedException>(() => 5.Should().BeGreaterThan(10));

    [Fact]
    public void BeInRange_Passes() => 5.Should().BeInRange(1, 10);

    [Fact]
    public void BeInRange_Fails() =>
        Assert.Throws<AssertionFailedException>(() => 15.Should().BeInRange(1, 10));

    [Fact]
    public void BeApproximately_Passes_For_Double() => 3.14159.Should().BeApproximately(3.14, 0.01);

    [Fact]
    public void BeApproximately_Fails_For_Double() =>
        Assert.Throws<AssertionFailedException>(() => 3.14159.Should().BeApproximately(3.0, 0.01));

    [Fact]
    public void Decimal_Be_Passes() => 5.5m.Should().Be(5.5m);
}

public class CollectionAssertionsTests
{
    [Fact]
    public void HaveCount_Passes() => new[] { 1, 2, 3 }.Should().HaveCount(3);

    [Fact]
    public void HaveCount_Fails() =>
        Assert.Throws<AssertionFailedException>(() => new[] { 1, 2, 3 }.Should().HaveCount(5));

    [Fact]
    public void Contain_Passes() => new[] { 1, 2, 3 }.Should().Contain(2);

    [Fact]
    public void BeEquivalentTo_Passes_Regardless_Of_Order() =>
        new[] { 1, 2, 3 }.Should().BeEquivalentTo(new[] { 3, 1, 2 });

    [Fact]
    public void BeEquivalentTo_Fails_When_Different() =>
        Assert.Throws<AssertionFailedException>(() => new[] { 1, 2, 3 }.Should().BeEquivalentTo(new[] { 1, 2 }));

    [Fact]
    public void Equal_Passes_When_Same_Order() =>
        new[] { 1, 2, 3 }.Should().Equal(new[] { 1, 2, 3 });

    [Fact]
    public void Equal_Fails_When_Different_Order() =>
        Assert.Throws<AssertionFailedException>(() => new[] { 1, 2, 3 }.Should().Equal(new[] { 3, 2, 1 }));

    [Fact]
    public void BeEmpty_Passes_For_Empty_Collection() => Array.Empty<int>().Should().BeEmpty();

    [Fact]
    public void OnlyHaveUniqueItems_Fails_For_Duplicates() =>
        Assert.Throws<AssertionFailedException>(() => new[] { 1, 1, 2 }.Should().OnlyHaveUniqueItems());
}

public class ExceptionAssertionsTests
{
    [Fact]
    public void Throw_Passes_When_Expected_Exception_Thrown()
    {
        Action act = () => throw new InvalidOperationException("boom");
        act.Should().Throw<InvalidOperationException>().WithMessage("boom");
    }

    [Fact]
    public void Throw_Fails_When_No_Exception_Thrown()
    {
        Action act = () => { };
        Assert.Throws<AssertionFailedException>(() => act.Should().Throw<InvalidOperationException>());
    }

    [Fact]
    public void Throw_Fails_When_Wrong_Exception_Type_Thrown()
    {
        Action act = () => throw new ArgumentException("wrong type");
        Assert.Throws<AssertionFailedException>(() => act.Should().Throw<InvalidOperationException>());
    }

    [Fact]
    public void NotThrow_Passes_When_No_Exception() =>
        ((Action)(() => { })).Should().NotThrow();

    [Fact]
    public void NotThrow_Fails_When_Exception_Thrown()
    {
        Action act = () => throw new Exception("oops");
        Assert.Throws<AssertionFailedException>(() => act.Should().NotThrow());
    }

    [Fact]
    public async Task ThrowAsync_Passes_When_Expected_Exception_Thrown()
    {
        Func<Task> act = () => throw new InvalidOperationException("async boom");
        var ex = await act.Should().ThrowAsync<InvalidOperationException>();
        ex.WithMessage("async boom");
    }

    [Fact]
    public async Task NotThrowAsync_Passes_When_No_Exception() =>
        await (((Func<Task>)(() => Task.CompletedTask)).Should().NotThrowAsync());
}
