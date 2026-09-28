using Xunit;

namespace AutoAssert.Json.Tests;

public class JsonAssertionExtensionsTests
{
    [Fact]
    public void BeValidJson_passes_for_valid_json()
    {
        """{"a":1}""".Should().BeValidJson();
    }

    [Fact]
    public void BeValidJson_fails_for_invalid_json()
    {
        Assert.Throws<AssertionFailedException>(() => "not json".Should().BeValidJson());
    }

    [Fact]
    public void HaveJsonProperty_finds_nested_and_indexed_paths()
    {
        var json = """{"customer":{"name":"Ada"},"items":[{"id":1},{"id":2}]}""";

        json.Should()
            .HaveJsonProperty("customer.name")
            .And.HaveJsonProperty("items[0].id")
            .And.HaveJsonProperty("items[1].id");
    }

    [Fact]
    public void HaveJsonProperty_fails_when_path_missing()
    {
        var json = """{"customer":{"name":"Ada"}}""";
        Assert.Throws<AssertionFailedException>(() => json.Should().HaveJsonProperty("customer.address"));
    }

    [Fact]
    public void HaveJsonProperty_with_expected_value_compares_by_value()
    {
        var json = """{"customer":{"name":"Ada","age":30}}""";

        json.Should().HaveJsonPropertyEqualTo("customer.name", "Ada");
        json.Should().HaveJsonPropertyEqualTo("customer.age", 30);

        Assert.Throws<AssertionFailedException>(() => json.Should().HaveJsonPropertyEqualTo("customer.name", "Bob"));
    }

    [Fact]
    public void BeEquivalentToJson_ignores_object_property_order()
    {
        var actual = """{"a":1,"b":2}""";
        var expected = """{"b":2,"a":1}""";

        actual.Should().BeEquivalentToJson(expected);
    }

    [Fact]
    public void BeEquivalentToJson_respects_array_order()
    {
        var actual = """[1,2,3]""";
        var reordered = """[3,2,1]""";

        Assert.Throws<AssertionFailedException>(() => actual.Should().BeEquivalentToJson(reordered));
    }

    [Fact]
    public void BeEquivalentToJson_reports_every_mismatch()
    {
        var actual = """{"a":1,"b":2,"c":3}""";
        var expected = """{"a":99,"b":2,"c":100}""";

        var ex = Assert.Throws<AssertionFailedException>(() => actual.Should().BeEquivalentToJson(expected));
        Assert.Contains("$.a", ex.Message);
        Assert.Contains("$.c", ex.Message);
        Assert.DoesNotContain("$.b", ex.Message);
    }

    [Fact]
    public void BeEquivalentToJson_detects_missing_and_extra_properties()
    {
        var actual = """{"a":1,"extra":true}""";
        var expected = """{"a":1,"missing":true}""";

        var ex = Assert.Throws<AssertionFailedException>(() => actual.Should().BeEquivalentToJson(expected));
        Assert.Contains("missing", ex.Message);
        Assert.Contains("extra", ex.Message);
    }
}
