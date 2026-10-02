using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoAssert.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EquivalencySuggestionAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AUTOA004";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "Multiple property asserts can be replaced with BeEquivalentTo",
        messageFormat: "Consider replacing repeated '{0}.*.Should().Be({1}.*)' assertions with a single BeEquivalentTo assertion",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "When multiple property-level equality assertions compare one object to another object in the same scope, a single BeEquivalentTo assertion is usually clearer and less brittle.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeBlock, SyntaxKind.Block);
    }

    private static void AnalyzeBlock(SyntaxNodeAnalysisContext context)
    {
        var block = (BlockSyntax)context.Node;
        var groups = EquivalencySuggestionHelper.FindGroups(block, context.SemanticModel, context.CancellationToken);
        foreach (var group in groups)
        {
            var properties = ImmutableDictionary<string, string?>.Empty
                .Add("ActualRoot", group.ActualRoot)
                .Add("ExpectedRoot", group.ExpectedRoot);

            var location = group.Assertions[2].Statement.GetLocation();
            context.ReportDiagnostic(Diagnostic.Create(Rule, location, properties, group.ActualRoot, group.ExpectedRoot));
        }
    }
}
