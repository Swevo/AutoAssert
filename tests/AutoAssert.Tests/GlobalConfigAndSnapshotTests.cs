using System.IO;
using Xunit;

namespace AutoAssert.Tests;

public class AssertionConfigTests
{
    private class Person
    {
        public string Name { get; set; } = "";
        public int Id { get; set; }
    }

    public AssertionConfigTests() => AssertionConfig.ResetEquivalencyDefaults();

    [Fact]
    public void Global_Defaults_Apply_To_BeEquivalentTo_Without_Explicit_Options()
    {
        try
        {
            AssertionConfig.ConfigureEquivalency(options => options.Excluding<Person, int>(p => p.Id));

            var actual = new Person { Name = "Alice", Id = 1 };
            var expected = new Person { Name = "Alice", Id = 2 };

            actual.Should().BeEquivalentTo(expected);
        }
        finally
        {
            AssertionConfig.ResetEquivalencyDefaults();
        }
    }

    [Fact]
    public void Global_Defaults_Compose_With_Per_Call_Configuration()
    {
        try
        {
            AssertionConfig.ConfigureEquivalency(options => options.Excluding<Person, int>(p => p.Id));

            var actual = new Person { Name = "Alice", Id = 1 };
            var expected = new Person { Name = "Bob", Id = 2 };

            // Id is excluded globally; Name is additionally excluded per-call.
            actual.Should().BeEquivalentTo(expected, options => options.Excluding<Person, string>(p => p.Name));
        }
        finally
        {
            AssertionConfig.ResetEquivalencyDefaults();
        }
    }

    [Fact]
    public void Without_Global_Defaults_Mismatch_Still_Fails()
    {
        var actual = new Person { Name = "Alice", Id = 1 };
        var expected = new Person { Name = "Alice", Id = 2 };

        Assert.Throws<AssertionFailedException>(() => actual.Should().BeEquivalentTo(expected));
    }
}

public class SnapshotAssertionsTests : IDisposable
{
    private readonly string _snapshotDirectory =
        Path.Combine(Path.GetDirectoryName(GetThisFilePath())!, "__snapshots__");

    private static string GetThisFilePath([System.Runtime.CompilerServices.CallerFilePath] string path = "") => path;

    public void Dispose()
    {
        var path = Path.Combine(_snapshotDirectory, $"{nameof(MatchSnapshot_Creates_Baseline_On_First_Run)}.snapshot.json");
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private class Widget
    {
        public string Name { get; set; } = "";
        public int Count { get; set; }
    }

    [Fact]
    public void MatchSnapshot_Creates_Baseline_On_First_Run()
    {
        var path = Path.Combine(_snapshotDirectory, $"{nameof(MatchSnapshot_Creates_Baseline_On_First_Run)}.snapshot.json");
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        var subject = new Widget { Name = "Widget", Count = 1 };

        // First call records the baseline and passes.
        subject.Should().MatchSnapshot();
        Assert.True(File.Exists(path));

        // Second call with the same value against the now-existing baseline still passes.
        subject.Should().MatchSnapshot();
    }

    [Fact]
    public void MatchSnapshot_Fails_When_Value_No_Longer_Matches_Baseline()
    {
        var path = Path.Combine(_snapshotDirectory, "MatchSnapshot_Fails_When_Value_No_Longer_Matches_Baseline.snapshot.json");
        Directory.CreateDirectory(_snapshotDirectory);
        File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(new Widget { Name = "Widget", Count = 1 }, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));

        try
        {
            var subject = new Widget { Name = "Widget", Count = 2 };
            Assert.Throws<AssertionFailedException>(() => subject.Should().MatchSnapshot());
        }
        finally
        {
            File.Delete(path);
        }
    }
}
