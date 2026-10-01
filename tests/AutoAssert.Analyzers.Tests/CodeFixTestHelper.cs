using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace AutoAssert.Analyzers.Tests;

internal static class CodeFixTestHelper
{
    public static async Task<string> ApplyCodeFixAsync(
        DiagnosticAnalyzer analyzer,
        CodeFixProvider codeFixProvider,
        string source,
        string diagnosticId,
        string actionTitleContains)
    {
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .Append(MetadataReference.CreateFromFile(typeof(AssertionExtensions).Assembly.Location))
            .ToList();

        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId);

        using var workspace = new AdhocWorkspace();
        var solution = workspace.CurrentSolution
            .AddProject(projectId, "TestProject", "TestProject", LanguageNames.CSharp)
            .WithProjectCompilationOptions(projectId, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .WithProjectParseOptions(projectId, new CSharpParseOptions(LanguageVersion.Latest))
            .AddMetadataReferences(projectId, references)
            .AddDocument(documentId, "Test.cs", SourceText.From(source));

        var document = solution.GetDocument(documentId)!;
        var compilation = await document.Project.GetCompilationAsync().ConfigureAwait(false);
        var diagnostics = await compilation!.WithAnalyzers(ImmutableArray.Create(analyzer)).GetAnalyzerDiagnosticsAsync().ConfigureAwait(false);
        var diagnostic = diagnostics.First(d => d.Id == diagnosticId);

        var actions = new List<CodeAction>();
        var context = new CodeFixContext(document, diagnostic, (action, _) => actions.Add(action), CancellationToken.None);
        await codeFixProvider.RegisterCodeFixesAsync(context).ConfigureAwait(false);

        var chosenAction = actions.First(a => a.Title.Contains(actionTitleContains, StringComparison.Ordinal));
        var operations = await chosenAction.GetOperationsAsync(CancellationToken.None).ConfigureAwait(false);
        var applyChanges = operations.OfType<ApplyChangesOperation>().Single();
        var updatedDoc = applyChanges.ChangedSolution.GetDocument(documentId)!;
        var updatedText = await updatedDoc.GetTextAsync().ConfigureAwait(false);
        return updatedText.ToString();
    }
}
