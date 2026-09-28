using System.Diagnostics;
using Xunit;

namespace AutoAssert.Tests;

public class ChainingTests
{
    [Fact]
    public void And_Allows_Chaining_Multiple_Assertions()
    {
        "hello world".Should()
            .NotBeNullOrEmpty()
            .And.StartWith("hello")
            .And.EndWith("world")
            .And.Contain("lo wo");
    }

    [Fact]
    public void And_Chains_Numeric_Assertions()
    {
        5.Should().BePositive().And.BeLessThan(10).And.BeGreaterThan(1);
    }

    [Fact]
    public void And_Chains_Collection_Assertions()
    {
        new[] { 1, 2, 3 }.Should().HaveCount(3).And.Contain(2).And.BeInAscendingOrder();
    }
}

public class AssertionScopeTests
{
    [Fact]
    public void Scope_Collects_Multiple_Failures_Into_One_Exception()
    {
        var ex = Assert.Throws<AssertionFailedException>(() =>
        {
            using var scope = new AssertionScope();
            1.Should().Be(2);
            "a".Should().Be("b");
        });

        Assert.Contains("1)", ex.Message);
        Assert.Contains("2)", ex.Message);
    }

    [Fact]
    public void Scope_Does_Not_Throw_When_No_Failures()
    {
        using var scope = new AssertionScope();
        1.Should().Be(1);
        "a".Should().Be("a");
    }

    [Fact]
    public void Scope_Does_Not_Throw_Immediately_On_Failure_But_Captures_It()
    {
        Assert.Throws<AssertionFailedException>(() =>
        {
            using var scope = new AssertionScope();
            1.Should().Be(2);
            Assert.Single(scope.Failures);
        });
    }
}

public class EquivalencyOptionsTests
{
    private class Address
    {
        public string City { get; set; } = "";
        public string Zip { get; set; } = "";
    }

    private class Person
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
        public Address? Address { get; set; }
    }

    [Fact]
    public void BeEquivalentTo_Reports_Full_Diff_For_Multiple_Mismatched_Members()
    {
        var actual = new Person { Name = "Alice", Age = 30, Address = new Address { City = "NYC", Zip = "10001" } };
        var expected = new Person { Name = "Bob", Age = 31, Address = new Address { City = "LA", Zip = "10001" } };

        var ex = Assert.Throws<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.Contains("Name", ex.Message);
        Assert.Contains("Age", ex.Message);
        Assert.Contains("City", ex.Message);
    }

    [Fact]
    public void BeEquivalentTo_Excluding_Member_Ignores_It()
    {
        var actual = new Person { Name = "Alice", Age = 30 };
        var expected = new Person { Name = "Alice", Age = 99 };

        actual.Should().BeEquivalentTo(expected, options => options.Excluding<Person, int>(p => p.Age));
    }

    [Fact]
    public void BeEquivalentTo_WithStrictOrdering_Fails_For_Reordered_Collections()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 3, 2, 1 };

        Assert.Throws<AssertionFailedException>(() =>
            actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering()));
    }

    [Fact]
    public void BeEquivalentTo_Without_StrictOrdering_Passes_For_Reordered_Collections()
    {
        var actual = new[] { 1, 2, 3 };
        var expected = new[] { 3, 2, 1 };

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void BeEquivalentTo_Reports_Diff_For_Every_Item_In_Mismatched_Collection()
    {
        var actual = new[]
        {
            new Person { Name = "Alice", Age = 30 },
            new Person { Name = "Bob", Age = 40 },
        };
        var expected = new[]
        {
            new Person { Name = "Alice", Age = 99 },
            new Person { Name = "Bob", Age = 99 },
        };

        var ex = Assert.Throws<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));

        // Both items mismatch on Age (30->99 and 40->99); a full diff should surface both,
        // not just the first blocking pair found during matching.
        Assert.Contains("30", ex.Message);
        Assert.Contains("40", ex.Message);
    }
}

public class NewStringAssertionsTests
{
    [Fact]
    public void BeEquivalentTo_Ignores_Case() => "HELLO".Should().BeEquivalentTo("hello");

    [Fact]
    public void ContainEquivalentOf_Ignores_Case() => "Hello World".Should().ContainEquivalentOf("WORLD");

    [Fact]
    public void Match_Supports_Wildcards() => "hello.txt".Should().Match("*.txt");

    [Fact]
    public void BeUpperCased_Passes_For_Uppercase() => "ABC".Should().BeUpperCased();

    [Fact]
    public void BeLowerCased_Passes_For_Lowercase() => "abc".Should().BeLowerCased();

    [Fact]
    public void NotStartWith_Fails_When_It_Does() =>
        Assert.Throws<AssertionFailedException>(() => "hello".Should().NotStartWith("he"));
}

public class NewNumericAssertionsTests
{
    [Fact]
    public void BePositive_Passes_For_Positive() => 5.Should().BePositive();

    [Fact]
    public void BeNegative_Passes_For_Negative() => (-5).Should().BeNegative();

    [Fact]
    public void BeOneOf_Passes_When_Contained() => 3.Should().BeOneOf(1, 2, 3);

    [Fact]
    public void BeOneOf_Fails_When_Not_Contained() =>
        Assert.Throws<AssertionFailedException>(() => 3.Should().BeOneOf(1, 2));

    [Fact]
    public void BeNaN_Passes_For_NaN() => double.NaN.Should().BeNaN();

    [Fact]
    public void NotBeNaN_Passes_For_Number() => 1.0.Should().NotBeNaN();
}

public class NewCollectionAssertionsTests
{
    [Fact]
    public void HaveCountGreaterThan_Passes() => new[] { 1, 2, 3 }.Should().HaveCountGreaterThan(2);

    [Fact]
    public void HaveCountLessThan_Passes() => new[] { 1, 2 }.Should().HaveCountLessThan(3);

    [Fact]
    public void ContainInOrder_Passes_For_Subsequence() =>
        new[] { 1, 2, 3, 4 }.Should().ContainInOrder(new[] { 1, 3, 4 });

    [Fact]
    public void SatisfyRespectively_Checks_Each_Item() =>
        new[] { 1, 2, 3 }.Should().SatisfyRespectively(
            x => Assert.Equal(1, x),
            x => Assert.Equal(2, x),
            x => Assert.Equal(3, x));

    [Fact]
    public void BeInAscendingOrder_Passes() => new[] { 1, 2, 3 }.Should().BeInAscendingOrder();

    [Fact]
    public void BeInDescendingOrder_Fails_When_Not() =>
        Assert.Throws<AssertionFailedException>(() => new[] { 1, 2, 3 }.Should().BeInDescendingOrder());
}

public class NewExceptionAssertionsTests
{
    [Fact]
    public void ValueFunc_Throw_Returns_Exception_For_Chaining()
    {
        Func<int> func = () => throw new InvalidOperationException("boom");
        func.Should().Throw<InvalidOperationException>().Where(e => e.Message == "boom");
    }

    [Fact]
    public void ValueFunc_NotThrow_Returns_Value()
    {
        Func<int> func = () => 42;
        var result = func.Should().NotThrow();
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task FuncTask_ThrowAsync_Works()
    {
        Func<Task<int>> func = () => throw new InvalidOperationException("async boom");
        await func.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task FuncTask_NotThrowAsync_Returns_Value()
    {
        Func<Task<int>> func = () => Task.FromResult(42);
        var result = await func.Should().NotThrowAsync();
        Assert.Equal(42, result);
    }

    [Fact]
    public void WithParameterName_Passes_For_Matching_Parameter()
    {
        Action act = () => throw new ArgumentNullException("value");
        act.Should().Throw<ArgumentNullException>().WithParameterName("value");
    }

    [Fact]
    public void NotThrow_Generic_Allows_Other_Exceptions()
    {
        Action act = () => throw new InvalidOperationException();
        Assert.Throws<InvalidOperationException>(() => act.Should().NotThrow<ArgumentException>());
    }
}

public class DateTimeAssertionsTests
{
    [Fact]
    public void BeBefore_Passes() => new DateTime(2020, 1, 1).Should().BeBefore(new DateTime(2021, 1, 1));

    [Fact]
    public void BeAfter_Fails_When_Not() =>
        Assert.Throws<AssertionFailedException>(() =>
            new DateTime(2020, 1, 1).Should().BeAfter(new DateTime(2021, 1, 1)));

    [Fact]
    public void BeCloseTo_Passes_Within_Precision() =>
        new DateTime(2020, 1, 1, 12, 0, 0).Should().BeCloseTo(new DateTime(2020, 1, 1, 12, 0, 1), TimeSpan.FromSeconds(2));
}

public class TimeSpanAssertionsTests
{
    [Fact]
    public void BeGreaterThan_Passes() => TimeSpan.FromSeconds(10).Should().BeGreaterThan(TimeSpan.FromSeconds(5));
}

public class GuidAssertionsTests
{
    [Fact]
    public void BeEmpty_Passes_For_Empty_Guid() => Guid.Empty.Should().BeEmpty();

    [Fact]
    public void NotBeEmpty_Passes_For_NonEmpty_Guid() => Guid.NewGuid().Should().NotBeEmpty();
}

public class NullableAssertionsTests
{
    [Fact]
    public void HaveValue_Passes_When_Has_Value()
    {
        int? subject = 5;
        subject.Should().HaveValue();
    }

    [Fact]
    public void NotHaveValue_Passes_When_Null()
    {
        int? subject = null;
        subject.Should().NotHaveValue();
    }
}

public enum SampleFlags
{
    None = 0,
    A = 1,
    B = 2
}

[Flags]
public enum SampleFlagsEnum
{
    None = 0,
    A = 1,
    B = 2
}

public class EnumAssertionsTests
{
    [Fact]
    public void Be_Passes_For_Equal_Enum() => SampleFlags.A.Should().Be(SampleFlags.A);

    [Fact]
    public void HaveFlag_Passes_For_Combined_Flags()
    {
        var value = SampleFlagsEnum.A | SampleFlagsEnum.B;
        value.Should().HaveFlag(SampleFlagsEnum.A);
    }
}

public class DictionaryAssertionsTests
{
    [Fact]
    public void ContainKey_Passes() => new Dictionary<string, int> { ["a"] = 1 }.Should().ContainKey("a");

    [Fact]
    public void ContainKeyAndValue_Passes() =>
        new Dictionary<string, int> { ["a"] = 1 }.Should().ContainKeyAndValue("a", 1);

    [Fact]
    public void ContainKeyAndValue_Fails_For_Wrong_Value() =>
        Assert.Throws<AssertionFailedException>(() =>
            new Dictionary<string, int> { ["a"] = 1 }.Should().ContainKeyAndValue("a", 2));

    [Fact]
    public void NotContainKey_Passes_When_Absent() =>
        new Dictionary<string, int> { ["a"] = 1 }.Should().NotContainKey("b");
}

public class ExecutionTimeAssertionsTests
{
    [Fact]
    public void CompleteWithin_Passes_For_Fast_Action()
    {
        Action act = () => { };
        act.Should().CompleteWithin(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void CompleteWithin_Fails_For_Slow_Action()
    {
        Action act = () => Thread.Sleep(50);
        Assert.Throws<AssertionFailedException>(() => act.Should().CompleteWithin(TimeSpan.FromMilliseconds(1)));
    }

    [Fact]
    public void ExecutionTime_Should_BeLessThan_Passes()
    {
        Action act = () => { };
        act.ExecutionTime().Should().BeLessThan(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task CompleteWithinAsync_Passes_For_Fast_Operation()
    {
        Func<Task> action = () => Task.CompletedTask;
        await action.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5));
    }
}
