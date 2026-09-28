using Xunit;

namespace AutoAssert.Tests;

public class ColorizedOutputTests : IDisposable
{
    public ColorizedOutputTests() => AssertionConfig.UseColorizedOutput = false;

    public void Dispose() => AssertionConfig.UseColorizedOutput = false;

    [Fact]
    public void Failure_messages_contain_no_ansi_codes_by_default()
    {
        var ex = Assert.Throws<AssertionFailedException>(() => "actual".Should().Be("expected"));
        Assert.DoesNotContain('\u001b', ex.Message);
    }

    [Fact]
    public void Failure_messages_are_colorized_when_enabled()
    {
        AssertionConfig.UseColorizedOutput = true;

        var ex = Assert.Throws<AssertionFailedException>(() => "actual".Should().Be("expected"));
        Assert.Contains('\u001b', ex.Message);
    }

    [Fact]
    public void BeEquivalentTo_diffs_are_colorized_when_enabled()
    {
        AssertionConfig.UseColorizedOutput = true;

        var actual = new { Name = "Alice", Age = 1 };
        var expected = new { Name = "Alice", Age = 2 };

        var ex = Assert.Throws<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
        Assert.Contains('\u001b', ex.Message);
    }
}

public class WildcardExceptionMessageTests
{
    [Fact]
    public void WithMessageMatching_supports_wildcards()
    {
        Action action = () => throw new InvalidOperationException("Order 12345 was not found");

        action.Should().Throw<InvalidOperationException>()
            .WithMessageMatching("Order * was not found");
    }

    [Fact]
    public void WithMessageMatching_fails_when_pattern_does_not_match()
    {
        Action action = () => throw new InvalidOperationException("Order 12345 was not found");

        Assert.Throws<AssertionFailedException>(() =>
            action.Should().Throw<InvalidOperationException>().WithMessageMatching("Customer * was not found"));
    }
}
