using Xunit;

namespace AutoAssert.Analyzers.Tests;

public class WeakExceptionAssertionAnalyzerTests
{
    [Fact]
    public async Task Reports_When_Throw_Uses_System_Exception()
    {
        const string source = """
            using System;
            using AutoAssert;

            public class Sample
            {
                public void Test(Action action)
                {
                    action.Should().Throw<Exception>();
                }
            }
            """;

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(new WeakExceptionAssertionAnalyzer(), source);

        Assert.Contains(diagnostics, d => d.Id == WeakExceptionAssertionAnalyzer.DiagnosticId);
    }

    [Fact]
    public async Task Reports_When_ThrowAsync_Uses_System_Exception()
    {
        const string source = """
            using System;
            using System.Threading.Tasks;
            using AutoAssert;

            public class Sample
            {
                public async Task Test(Func<Task> action)
                {
                    await action.Should().ThrowAsync<Exception>();
                }
            }
            """;

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(new WeakExceptionAssertionAnalyzer(), source);

        Assert.Contains(diagnostics, d => d.Id == WeakExceptionAssertionAnalyzer.DiagnosticId);
    }

    [Fact]
    public async Task Does_Not_Report_For_Specific_Exception_Type()
    {
        const string source = """
            using System;
            using AutoAssert;

            public class Sample
            {
                public void Test(Action action)
                {
                    action.Should().Throw<InvalidOperationException>();
                }
            }
            """;

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(new WeakExceptionAssertionAnalyzer(), source);

        Assert.DoesNotContain(diagnostics, d => d.Id == WeakExceptionAssertionAnalyzer.DiagnosticId);
    }

    [Fact]
    public async Task Does_Not_Report_For_Non_AutoAssert_Throw()
    {
        const string source = """
            using System;

            public static class CustomAssert
            {
                public static Thrower Should(this Action action) => new();
            }

            public readonly struct Thrower
            {
                public void Throw<TException>() where TException : Exception
                {
                }
            }

            public class Sample
            {
                public void Test(Action action)
                {
                    action.Should().Throw<Exception>();
                }
            }
            """;

        var diagnostics = await AnalyzerTestHelper.GetDiagnosticsAsync(new WeakExceptionAssertionAnalyzer(), source);

        Assert.DoesNotContain(diagnostics, d => d.Id == WeakExceptionAssertionAnalyzer.DiagnosticId);
    }
}
