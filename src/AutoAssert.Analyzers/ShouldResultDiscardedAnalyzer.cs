using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoAssert.Analyzers;

/// <summary>
/// Flags <c>subject.Should();</c> used as a bare statement. Calling <c>Should()</c> alone
/// produces an assertion struct (e.g. <c>ObjectAssertions</c>) but asserts nothing on its
/// own — the developer almost certainly meant to chain an assertion method after it.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ShouldResultDiscardedAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AUTOA001";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "Should() result is discarded without an assertion",
        messageFormat: "'Should()' was called but no assertion method follows it, so nothing is actually asserted",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Calling Should() alone (e.g. 'subject.Should();') produces an assertion struct but doesn't verify anything. Chain an assertion method, e.g. 'subject.Should().NotBeNull();'.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeExpressionStatement, SyntaxKind.ExpressionStatement);
    }

    private static void AnalyzeExpressionStatement(SyntaxNodeAnalysisContext context)
    {
        var statement = (ExpressionStatementSyntax)context.Node;

        if (statement.Expression is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol methodSymbol)
        {
            return;
        }

        if (methodSymbol.Name != "Should")
        {
            return;
        }

        var containingType = methodSymbol.ContainingType;
        if (containingType is null ||
            containingType.Name != "AssertionExtensions" ||
            containingType.ContainingNamespace?.ToDisplayString() != "AutoAssert")
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.GetLocation()));
    }
}
