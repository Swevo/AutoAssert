using AutoAssert;

namespace AutoAssert.Quickstart.Tests;

public sealed class OrderTests
{
    [Fact]
    public void Can_assert_values_and_object_equivalency()
    {
        var order = new Order(42, "Ada", 120m);

        order.Total.Should().Be(120m);
        order.Should().BeEquivalentTo(new { Id = 42, CustomerName = "Ada", Total = 120m });
    }

    [Fact]
    public void Can_collect_multiple_failures_with_assertion_scope()
    {
        var order = new Order(42, "Ada", 120m);

        using var scope = new AssertionScope();
        order.Id.Should().Be(42);
        order.CustomerName.Should().NotBeNullOrEmpty();
        order.Total.Should().BePositive();
    }

    private sealed record Order(int Id, string CustomerName, decimal Total);
}
