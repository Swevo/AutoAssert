using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AutoAssert.Generator.Tests;

/// <summary>
/// Compiles a snippet with a reference to AutoAssert, runs
/// <see cref="EquivalencyComparerGenerator"/>, and emits the resulting assembly to memory so
/// tests can load it and exercise the generated, self-registering comparer at runtime.
/// </summary>
internal static class GeneratorTestHelper
{
    public static (ImmutableArray<Diagnostic> Diagnostics, Assembly? Assembly) CompileAndEmit(string source)
    {
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .Append(MetadataReference.CreateFromFile(typeof(AssertionExtensions).Assembly.Location))
            .ToList();

        var syntaxTree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Latest));

        var compilation = CSharpCompilation.Create(
            "GeneratorTestAssembly_" + Guid.NewGuid().ToString("N"),
            [syntaxTree],
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var driver = CSharpGeneratorDriver.Create(new EquivalencyComparerGenerator());
        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var generatorDiagnostics);

        using var stream = new MemoryStream();
        var emitResult = outputCompilation.Emit(stream);

        var diagnostics = generatorDiagnostics.AddRange(emitResult.Diagnostics);

        if (!emitResult.Success)
        {
            return (diagnostics, null);
        }

        var assembly = System.Reflection.Assembly.Load(stream.ToArray());
        return (diagnostics, assembly);
    }
}
