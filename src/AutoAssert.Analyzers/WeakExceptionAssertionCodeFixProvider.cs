using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoAssert.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(WeakExceptionAssertionCodeFixProvider))]
public sealed class WeakExceptionAssertionCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [WeakExceptionAssertionAnalyzer.DiagnosticId];

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
        var typeArgument = node.FirstAncestorOrSelf<TypeSyntax>();
        if (typeArgument is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Replace with InvalidOperationException",
                createChangedDocument: token => ReplaceTypeAsync(context.Document, typeArgument, "global::System.InvalidOperationException", token),
                equivalenceKey: "AUTOA003_InvalidOperationException"),
            diagnostic);

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Replace with ArgumentException",
                createChangedDocument: token => ReplaceTypeAsync(context.Document, typeArgument, "global::System.ArgumentException", token),
                equivalenceKey: "AUTOA003_ArgumentException"),
            diagnostic);
    }

    private static async Task<Document> ReplaceTypeAsync(
        Document document,
        TypeSyntax existingType,
        string replacementTypeName,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var replacement = SyntaxFactory.ParseTypeName(replacementTypeName)
            .WithTriviaFrom(existingType);

        var newRoot = root.ReplaceNode(existingType, replacement);
        return document.WithSyntaxRoot(newRoot);
    }
}
