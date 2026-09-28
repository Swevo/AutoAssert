using Xunit;

namespace AutoAssert.Analyzers.Tests;

public class UnawaitedAsyncAssertionAnalyzerTests
{
    [Fact]
    public async Task Reports_When_ThrowAsync_Is_Not_Awaited()
    {
        const string source = """
            using System;
            using System.Threading.Tasks;
            using AutoAssert;

            public class Sample
            {
                public void Test(Func<Task> action)
                {
                    action.Should().ThrowAsync<InvalidOperationException>();
                }
            }
            """;

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(new UnawaitedAsyncAssertionAnalyzer(), source);

        Assert.Contains(diagnostics, d => d.Id == UnawaitedAsyncAssertionAnalyzer.DiagnosticId);
    }

    [Fact]
    public async Task Reports_When_NotThrowAsync_Is_Not_Awaited()
    {
        const string source = """
            using System;
            using System.Threading.Tasks;
            using AutoAssert;

            public class Sample
            {
                public void Test(Func<Task> action)
                {
                    action.Should().NotThrowAsync();
                }
            }
            """;

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(new UnawaitedAsyncAssertionAnalyzer(), source);

        Assert.Contains(diagnostics, d => d.Id == UnawaitedAsyncAssertionAnalyzer.DiagnosticId);
    }

    [Fact]
    public async Task Does_Not_Report_When_Awaited()
    {
        const string source = """
            using System;
            using System.Threading.Tasks;
            using AutoAssert;

            public class Sample
            {
                public async Task Test(Func<Task> action)
                {
                    await action.Should().NotThrowAsync();
                }
            }
            """;

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(new UnawaitedAsyncAssertionAnalyzer(), source);

        Assert.DoesNotContain(diagnostics, d => d.Id == UnawaitedAsyncAssertionAnalyzer.DiagnosticId);
    }
}
