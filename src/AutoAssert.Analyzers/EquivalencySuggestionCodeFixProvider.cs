using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoAssert.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EquivalencySuggestionCodeFixProvider))]
public sealed class EquivalencySuggestionCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [EquivalencySuggestionAnalyzer.DiagnosticId];

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

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Add BeEquivalentTo scaffold (keep existing asserts)",
                createChangedDocument: token => AddScaffoldAsync(context.Document, diagnostic, token),
                equivalenceKey: "AUTOA004_AddBeEquivalentToScaffold"),
            diagnostic);
    }

    private static async Task<Document> AddScaffoldAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var semanticModel = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
        if (semanticModel is null)
        {
            return document;
        }

        var node = root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
        var block = node.FirstAncestorOrSelf<BlockSyntax>();
        if (block is null)
        {
            return document;
        }

        diagnostic.Properties.TryGetValue("ActualRoot", out var actualRoot);
        diagnostic.Properties.TryGetValue("ExpectedRoot", out var expectedRoot);

        if (string.IsNullOrWhiteSpace(actualRoot) || string.IsNullOrWhiteSpace(expectedRoot))
        {
            return document;
        }

        var group = EquivalencySuggestionHelper
            .FindGroups(block, semanticModel, cancellationToken)
            .FirstOrDefault(g =>
                string.Equals(g.ActualRoot, actualRoot, StringComparison.Ordinal)
                && string.Equals(g.ExpectedRoot, expectedRoot, StringComparison.Ordinal)
                && g.Assertions.Any(a => a.Statement.Span.IntersectsWith(diagnostic.Location.SourceSpan)));

        if (group is null)
        {
            return document;
        }

        var uniqueByProperty = group.Assertions
            .GroupBy(a => a.PropertyName, StringComparer.Ordinal)
            .Select(g => g.First())
            .ToList();

        var propertyAssignments = string.Join(", ", uniqueByProperty.Select(a => $"{a.PropertyName} = {a.ExpectedExpression}"));
        var scaffoldText = $"{group.ActualRoot}.Should().BeEquivalentTo(new {{ {propertyAssignments} }});";
        var scaffoldStatement = SyntaxFactory.ParseStatement(scaffoldText)
            .WithLeadingTrivia(group.Assertions.Last().Statement.GetLeadingTrivia())
            .WithTrailingTrivia(group.Assertions.Last().Statement.GetTrailingTrivia());

        var updatedBlock = block.InsertNodesAfter(group.Assertions.Last().Statement, [scaffoldStatement]);
        var newRoot = root.ReplaceNode(block, updatedBlock);
        return document.WithSyntaxRoot(newRoot);
    }
}
