using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AutoAssert.Analyzers;

/// <summary>
/// Flags AutoAssert throw assertions that use <see cref="Exception"/> as the asserted type.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class WeakExceptionAssertionAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "AUTOA003";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        title: "Exception assertion is too broad",
        messageFormat: "'{0}<Exception>' is too broad; assert a specific exception type",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Using Throw<Exception>()/ThrowAsync<Exception>() can hide behavioral regressions by allowing any exception type to pass. Assert the most specific expected exception type instead.");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol methodSymbol)
        {
            return;
        }

        if (methodSymbol.Name is not ("Throw" or "ThrowAsync") || methodSymbol.TypeArguments.Length != 1)
        {
            return;
        }

        if (methodSymbol.ContainingType?.ContainingNamespace?.ToDisplayString() != "AutoAssert")
        {
            return;
        }

        if (methodSymbol.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) != "global::System.Exception")
        {
            return;
        }

        var location = methodSymbol.Name switch
        {
            _ when invocation.Expression is MemberAccessExpressionSyntax
            {
                Name: GenericNameSyntax { TypeArgumentList.Arguments.Count: > 0 } genericName
            } => genericName.TypeArgumentList.Arguments[0].GetLocation(),
            _ => invocation.GetLocation()
        };

        context.ReportDiagnostic(Diagnostic.Create(Rule, location, methodSymbol.Name));
    }
}
