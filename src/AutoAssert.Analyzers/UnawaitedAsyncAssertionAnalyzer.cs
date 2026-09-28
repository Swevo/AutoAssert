using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoAssert.Analyzers;

/// <summary>
/// Flags AutoAssert async assertion calls (e.g. <c>ThrowAsync</c>, <c>NotThrowAsync</c>,
/// <c>CompleteWithinAsync</c>) that are used as a bare statement without <c>await</c>.
/// Without awaiting, the assertion may not have run (or thrown) before the test
/// method returns, producing silent false-positive passes.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnawaitedAsyncAssertionAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AUTOA002";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "Async assertion is not awaited",
        messageFormat: "'{0}' returns a Task but is not awaited, so the assertion may not run before the test completes",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "AutoAssert async assertion methods (e.g. ThrowAsync, NotThrowAsync, CompleteWithinAsync) must be awaited so the assertion executes, and so any AssertionFailedException it throws is observed by the test runner.");

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

        // An `await x;` statement's Expression is an AwaitExpressionSyntax, not an
        // InvocationExpressionSyntax directly, so already-awaited calls never reach here.
        if (statement.Expression is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol methodSymbol)
        {
            return;
        }

        var returnType = methodSymbol.ReturnType;
        if (!IsTaskLike(returnType))
        {
            return;
        }

        var containingNamespace = methodSymbol.ContainingType?.ContainingNamespace?.ToDisplayString();
        if (containingNamespace != "AutoAssert")
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(Rule, invocation.GetLocation(), methodSymbol.Name));
    }

    private static bool IsTaskLike(ITypeSymbol type)
    {
        if (type is not INamedTypeSymbol named)
        {
            return false;
        }

        return named.Name is "Task" or "ValueTask" && named.ContainingNamespace?.ToDisplayString() == "System.Threading.Tasks";
    }
}
