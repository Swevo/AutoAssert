using Xunit;

namespace AutoAssert.Analyzers.Tests;

public class EquivalencySuggestionCodeFixProviderTests
{
    [Fact]
    public async Task Adds_BeEquivalentTo_Scaffold_Without_Removing_Existing_Asserts()
    {
        const string source = """
            using AutoAssert;

            public sealed class Person
            {
                public string Name { get; set; } = "";
                public int Age { get; set; }
                public string Email { get; set; } = "";
            }

            public class Sample
            {
                public void Test(Person actual, Person expected)
                {
                    actual.Name.Should().Be(expected.Name);
                    actual.Age.Should().Be(expected.Age);
                    actual.Email.Should().Be(expected.Email);
                }
            }
            """;

        var fixedSource = await CodeFixTestHelper.ApplyCodeFixAsync(
            new EquivalencySuggestionAnalyzer(),
            new EquivalencySuggestionCodeFixProvider(),
            source,
            EquivalencySuggestionAnalyzer.DiagnosticId,
            "BeEquivalentTo scaffold");

        Assert.Contains("actual.Name.Should().Be(expected.Name);", fixedSource);
        Assert.Contains("actual.Age.Should().Be(expected.Age);", fixedSource);
        Assert.Contains("actual.Email.Should().Be(expected.Email);", fixedSource);
        Assert.Contains(
            "actual.Should().BeEquivalentTo(new { Name = expected.Name, Age = expected.Age, Email = expected.Email });",
            fixedSource);
    }
}
