using Xunit;

namespace AutoAssert.Analyzers.Tests;

public class UnawaitedAsyncAssertionCodeFixProviderTests
{
    [Fact]
    public async Task Adds_Await_And_Upgrades_Void_Method_To_Async_Task()
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

        var fixedSource = await CodeFixTestHelper.ApplyCodeFixAsync(
            new UnawaitedAsyncAssertionAnalyzer(),
            new UnawaitedAsyncAssertionCodeFixProvider(),
            source,
            UnawaitedAsyncAssertionAnalyzer.DiagnosticId,
            "Await async assertion");

        Assert.Contains("public async", fixedSource);
        Assert.Contains("Task Test(", fixedSource);
        Assert.Contains("await action.Should().NotThrowAsync();", fixedSource);
    }

    [Fact]
    public async Task Adds_Await_In_Already_Async_Method()
    {
        const string source = """
            using System;
            using System.Threading.Tasks;
            using AutoAssert;

            public class Sample
            {
                public async Task Test(Func<Task> action)
                {
                    action.Should().ThrowAsync<InvalidOperationException>();
                }
            }
            """;

        var fixedSource = await CodeFixTestHelper.ApplyCodeFixAsync(
            new UnawaitedAsyncAssertionAnalyzer(),
            new UnawaitedAsyncAssertionCodeFixProvider(),
            source,
            UnawaitedAsyncAssertionAnalyzer.DiagnosticId,
            "Await async assertion");

        Assert.DoesNotContain("global::System.Threading.Tasks.Task Test", fixedSource);
        Assert.Contains("public async Task Test", fixedSource);
        Assert.Contains("await action.Should().ThrowAsync<InvalidOperationException>();", fixedSource);
    }

    [Fact]
    public async Task Adds_Await_And_Upgrades_Local_Function()
    {
        const string source = """
            using System;
            using System.Threading.Tasks;
            using AutoAssert;

            public class Sample
            {
                public void Test(Func<Task> action)
                {
                    void Local()
                    {
                        action.Should().NotThrowAsync();
                    }

                    Local();
                }
            }
            """;

        var fixedSource = await CodeFixTestHelper.ApplyCodeFixAsync(
            new UnawaitedAsyncAssertionAnalyzer(),
            new UnawaitedAsyncAssertionCodeFixProvider(),
            source,
            UnawaitedAsyncAssertionAnalyzer.DiagnosticId,
            "Await async assertion");

        Assert.Contains("async", fixedSource);
        Assert.Contains("Task Local(", fixedSource);
        Assert.Contains("await action.Should().NotThrowAsync();", fixedSource);
    }

    [Fact]
    public async Task Adds_Await_And_Makes_Lambda_Async()
    {
        const string source = """
            using System;
            using System.Threading.Tasks;
            using AutoAssert;

            public class Sample
            {
                public void Test(Func<Task> action)
                {
                    Action run = () =>
                    {
                        action.Should().NotThrowAsync();
                    };
                }
            }
            """;

        var fixedSource = await CodeFixTestHelper.ApplyCodeFixAsync(
            new UnawaitedAsyncAssertionAnalyzer(),
            new UnawaitedAsyncAssertionCodeFixProvider(),
            source,
            UnawaitedAsyncAssertionAnalyzer.DiagnosticId,
            "Await async assertion");

        Assert.Contains("Action run = async () =>", fixedSource);
        Assert.Contains("await action.Should().NotThrowAsync();", fixedSource);
    }
}
