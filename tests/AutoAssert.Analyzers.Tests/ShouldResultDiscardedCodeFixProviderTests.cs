using Xunit;

namespace AutoAssert.Analyzers.Tests;

public class ShouldResultDiscardedCodeFixProviderTests
{
    [Fact]
    public async Task Adds_NotBeNull_Assertion()
    {
        const string source = """
            using AutoAssert;

            public class Sample
            {
                public void Test(object subject)
                {
                    subject.Should();
                }
            }
            """;

        var fixedSource = await CodeFixTestHelper.ApplyCodeFixAsync(
            new ShouldResultDiscardedAnalyzer(),
            new ShouldResultDiscardedCodeFixProvider(),
            source,
            ShouldResultDiscardedAnalyzer.DiagnosticId,
            "NotBeNull");

        Assert.Contains("subject.Should().NotBeNull();", fixedSource);
    }

    [Fact]
    public async Task Adds_NotBeNullOrEmpty_For_String_Subjects()
    {
        const string source = """
            using AutoAssert;

            public class Sample
            {
                public void Test(string subject)
                {
                    subject.Should();
                }
            }
            """;

        var fixedSource = await CodeFixTestHelper.ApplyCodeFixAsync(
            new ShouldResultDiscardedAnalyzer(),
            new ShouldResultDiscardedCodeFixProvider(),
            source,
            ShouldResultDiscardedAnalyzer.DiagnosticId,
            "NotBeNullOrEmpty");

        Assert.Contains("subject.Should().NotBeNullOrEmpty();", fixedSource);
    }

    [Fact]
    public async Task Adds_BeExpected_Scaffold()
    {
        const string source = """
            using AutoAssert;

            public class Sample
            {
                public void Test(int subject, int expected)
                {
                    subject.Should();
                }
            }
            """;

        var fixedSource = await CodeFixTestHelper.ApplyCodeFixAsync(
            new ShouldResultDiscardedAnalyzer(),
            new ShouldResultDiscardedCodeFixProvider(),
            source,
            ShouldResultDiscardedAnalyzer.DiagnosticId,
            "Be(expected)");

        Assert.Contains("subject.Should().Be(expected);", fixedSource);
    }
}
