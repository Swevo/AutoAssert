using Xunit;

namespace AutoAssert.Analyzers.Tests;

public class EquivalencySuggestionAnalyzerTests
{
    [Fact]
    public async Task Reports_When_Three_Or_More_SameRoot_Be_Assertions_Exist()
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

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(new EquivalencySuggestionAnalyzer(), source);

        Assert.Contains(diagnostics, d => d.Id == EquivalencySuggestionAnalyzer.DiagnosticId);
    }

    [Fact]
    public async Task Does_Not_Report_For_Two_Assertions()
    {
        const string source = """
            using AutoAssert;

            public sealed class Person
            {
                public string Name { get; set; } = "";
                public int Age { get; set; }
            }

            public class Sample
            {
                public void Test(Person actual, Person expected)
                {
                    actual.Name.Should().Be(expected.Name);
                    actual.Age.Should().Be(expected.Age);
                }
            }
            """;

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(new EquivalencySuggestionAnalyzer(), source);

        Assert.DoesNotContain(diagnostics, d => d.Id == EquivalencySuggestionAnalyzer.DiagnosticId);
    }

    [Fact]
    public async Task Does_Not_Report_When_BeEquivalentTo_Already_Exists()
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
                    actual.Should().BeEquivalentTo(expected);
                }
            }
            """;

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(new EquivalencySuggestionAnalyzer(), source);

        Assert.DoesNotContain(diagnostics, d => d.Id == EquivalencySuggestionAnalyzer.DiagnosticId);
    }
}
