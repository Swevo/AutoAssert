using Xunit;

namespace AutoAssert.Analyzers.Tests;

public class WeakExceptionAssertionCodeFixProviderTests
{
    [Fact]
    public async Task Replaces_Throw_Exception_With_InvalidOperationException()
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

        var fixedSource = await CodeFixTestHelper.ApplyCodeFixAsync(
            new WeakExceptionAssertionAnalyzer(),
            new WeakExceptionAssertionCodeFixProvider(),
            source,
            WeakExceptionAssertionAnalyzer.DiagnosticId,
            "InvalidOperationException");

        Assert.Contains("Throw<global::System.InvalidOperationException>()", fixedSource);
    }

    [Fact]
    public async Task Replaces_ThrowAsync_Exception_With_ArgumentException()
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

        var fixedSource = await CodeFixTestHelper.ApplyCodeFixAsync(
            new WeakExceptionAssertionAnalyzer(),
            new WeakExceptionAssertionCodeFixProvider(),
            source,
            WeakExceptionAssertionAnalyzer.DiagnosticId,
            "ArgumentException");

        Assert.Contains("ThrowAsync<global::System.ArgumentException>()", fixedSource);
    }
}
