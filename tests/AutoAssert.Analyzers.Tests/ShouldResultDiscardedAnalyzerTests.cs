using Xunit;

namespace AutoAssert.Analyzers.Tests;

public class ShouldResultDiscardedAnalyzerTests
{
    [Fact]
    public async Task Reports_When_Should_Is_Called_Bare()
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

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(new ShouldResultDiscardedAnalyzer(), source);

        Assert.Contains(diagnostics, d => d.Id == ShouldResultDiscardedAnalyzer.DiagnosticId);
    }

    [Fact]
    public async Task Does_Not_Report_When_Assertion_Method_Follows()
    {
        const string source = """
            using AutoAssert;

            public class Sample
            {
                public void Test(object subject)
                {
                    subject.Should().NotBeNull();
                }
            }
            """;

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(new ShouldResultDiscardedAnalyzer(), source);

        Assert.DoesNotContain(diagnostics, d => d.Id == ShouldResultDiscardedAnalyzer.DiagnosticId);
    }

    [Fact]
    public async Task Does_Not_Report_When_Should_Result_Is_Assigned()
    {
        const string source = """
            using AutoAssert;

            public class Sample
            {
                public void Test(object subject)
                {
                    var assertions = subject.Should();
                }
            }
            """;

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(new ShouldResultDiscardedAnalyzer(), source);

        Assert.DoesNotContain(diagnostics, d => d.Id == ShouldResultDiscardedAnalyzer.DiagnosticId);
    }
}
