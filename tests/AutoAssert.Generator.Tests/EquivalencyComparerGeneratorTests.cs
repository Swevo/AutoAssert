using System.Linq;
using Xunit;

namespace AutoAssert.Generator.Tests;

public class EquivalencyComparerGeneratorTests
{
    private const string SamplePersonSource = """
        namespace SampleApp
        {
            [AutoAssert.GenerateEquivalencyComparer]
            public class Person
            {
                public string Name { get; set; } = "";
                public int Age { get; set; }
            }
        }
        """;

    [Fact]
    public void Generator_produces_no_diagnostics_for_a_simple_public_class()
    {
        var (diagnostics, assembly) = GeneratorTestHelper.CompileAndEmit(SamplePersonSource);

        Assert.Empty(diagnostics);
        Assert.NotNull(assembly);
    }

    [Fact]
    public void Generator_emits_a_comparer_that_self_registers_and_is_used_by_BeEquivalentTo()
    {
        var (_, assembly) = GeneratorTestHelper.CompileAndEmit(SamplePersonSource);
        Assert.NotNull(assembly);

        var personType = assembly!.GetType("SampleApp.Person")!;

        // Accessing a type from the module triggers its module initializer (net5.0+ runtime
        // behavior), which is what registers the generated comparer with AutoAssert.
        var actual = Activator.CreateInstance(personType)!;
        var expected = Activator.CreateInstance(personType)!;
        personType.GetProperty("Name")!.SetValue(actual, "Ada");
        personType.GetProperty("Name")!.SetValue(expected, "Ada");
        personType.GetProperty("Age")!.SetValue(actual, 30);
        personType.GetProperty("Age")!.SetValue(expected, 30);

        // Equivalent values pass.
        InvokeBeEquivalentTo(actual, expected);

        // A mismatch should still be reported (proving the generated comparer performs real
        // member comparisons, not just a stub that always passes).
        personType.GetProperty("Age")!.SetValue(expected, 31);
        var ex = Assert.ThrowsAny<Exception>(() => InvokeBeEquivalentTo(actual, expected));
        Assert.Contains("Age", ex.Message);
    }

    private static void InvokeBeEquivalentTo(object actual, object expected)
    {
        var should = typeof(AssertionExtensions)
            .GetMethods()
            .First(m => m.Name == "Should" && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(object));

        var assertions = should.Invoke(null, [actual]);
        var beEquivalentTo = assertions!.GetType()
            .GetMethods()
            .First(m => m.Name == "BeEquivalentTo" && m.GetParameters().Length == 3
                        && m.GetParameters()[0].ParameterType == typeof(object)
                        && m.GetParameters()[1].ParameterType == typeof(string));

        var parameters = beEquivalentTo.GetParameters();
        var args = new object?[parameters.Length];
        args[0] = expected;
        for (var i = 1; i < parameters.Length; i++)
        {
            args[i] = parameters[i].HasDefaultValue ? parameters[i].DefaultValue : null;
        }

        try
        {
            beEquivalentTo.Invoke(assertions, args);
        }
        catch (System.Reflection.TargetInvocationException tie) when (tie.InnerException is not null)
        {
            throw tie.InnerException;
        }
    }
}
