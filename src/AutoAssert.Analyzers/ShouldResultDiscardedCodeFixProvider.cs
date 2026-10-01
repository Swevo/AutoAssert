using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoAssert.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ShouldResultDiscardedCodeFixProvider))]
public sealed class ShouldResultDiscardedCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [ShouldResultDiscardedAnalyzer.DiagnosticId];

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        var diagnostic = context.Diagnostics.FirstOrDefault();
        if (diagnostic is null)
        {
            return;
        }
        var node = root.FindNode(context.Span, getInnermostNodeForTie: true);
        var invocation = node.FirstAncestorOrSelf<InvocationExpressionSyntax>();
        if (invocation is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Chain '.NotBeNull()'",
                createChangedDocument: token => AddAssertionAsync(context.Document, invocation, "NotBeNull", null, token),
                equivalenceKey: "AUTOA001_NotBeNull"),
            diagnostic);

        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);
        var subjectType = GetSubjectType(semanticModel, invocation, context.CancellationToken);

        if (subjectType?.SpecialType == SpecialType.System_String)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: "Chain '.NotBeNullOrEmpty()'",
                    createChangedDocument: token => AddAssertionAsync(context.Document, invocation, "NotBeNullOrEmpty", null, token),
                    equivalenceKey: "AUTOA001_NotBeNullOrEmpty"),
                diagnostic);
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Chain '.Be(expected)' scaffold",
                createChangedDocument: token => AddAssertionAsync(
                    context.Document,
                    invocation,
                    "Be",
                    SyntaxFactory.IdentifierName("expected"),
                    token),
                equivalenceKey: "AUTOA001_BeExpected"),
            diagnostic);
    }

    private static ITypeSymbol? GetSubjectType(SemanticModel? semanticModel, InvocationExpressionSyntax shouldInvocation, CancellationToken cancellationToken)
    {
        if (semanticModel is null)
        {
            return null;
        }

        if (shouldInvocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return null;
        }

        return semanticModel.GetTypeInfo(memberAccess.Expression, cancellationToken).Type;
    }

    private static async Task<Document> AddAssertionAsync(
        Document document,
        InvocationExpressionSyntax shouldInvocation,
        string assertionMethodName,
        ExpressionSyntax? argumentExpression,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var chainedMemberAccess = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            shouldInvocation,
            SyntaxFactory.IdentifierName(assertionMethodName));

        var newInvocation = SyntaxFactory.InvocationExpression(
            chainedMemberAccess,
            argumentExpression is null
                ? SyntaxFactory.ArgumentList()
                : SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(argumentExpression))))
            .WithTriviaFrom(shouldInvocation);

        var newRoot = root.ReplaceNode(shouldInvocation, newInvocation);
        return document.WithSyntaxRoot(newRoot);
    }
}
