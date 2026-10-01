using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoAssert.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UnawaitedAsyncAssertionCodeFixProvider))]
public sealed class UnawaitedAsyncAssertionCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [UnawaitedAsyncAssertionAnalyzer.DiagnosticId];

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
                title: "Await async assertion (and make containing scope async)",
                createChangedDocument: token => AddAwaitAndAsyncAsync(context.Document, invocation, token),
                equivalenceKey: "AUTOA002_AddAwait"),
            diagnostic);
    }

    private static async Task<Document> AddAwaitAndAsyncAsync(Document document, InvocationExpressionSyntax invocation, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var container = FindContainer(invocation);
        var awaitExpression = SyntaxFactory.AwaitExpression(invocation.WithoutTrivia()).WithTriviaFrom(invocation);

        if (container is null)
        {
            return document.WithSyntaxRoot(root.ReplaceNode(invocation, awaitExpression));
        }

        var rewrittenContainer = container.ReplaceNode(invocation, awaitExpression);
        rewrittenContainer = MakeContainerAsync(rewrittenContainer);
        var newRoot = root.ReplaceNode(container, rewrittenContainer);
        return document.WithSyntaxRoot(newRoot);
    }

    private static SyntaxNode? FindContainer(SyntaxNode invocation)
    {
        for (SyntaxNode? current = invocation; current is not null; current = current.Parent)
        {
            if (current is MethodDeclarationSyntax
                or LocalFunctionStatementSyntax
                or SimpleLambdaExpressionSyntax
                or ParenthesizedLambdaExpressionSyntax
                or AnonymousMethodExpressionSyntax)
            {
                return current;
            }
        }

        return null;
    }

    private static SyntaxNode MakeContainerAsync(SyntaxNode node)
    {
        return node switch
        {
            MethodDeclarationSyntax method => MakeMethodAsync(method),
            LocalFunctionStatementSyntax localFunction => MakeLocalFunctionAsync(localFunction),
            SimpleLambdaExpressionSyntax simpleLambda => simpleLambda.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword)
                ? simpleLambda
                : simpleLambda.WithAsyncKeyword(SyntaxFactory.Token(SyntaxKind.AsyncKeyword).WithTrailingTrivia(SyntaxFactory.Space)),
            ParenthesizedLambdaExpressionSyntax parenthesizedLambda => parenthesizedLambda.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword)
                ? parenthesizedLambda
                : parenthesizedLambda.WithAsyncKeyword(SyntaxFactory.Token(SyntaxKind.AsyncKeyword).WithTrailingTrivia(SyntaxFactory.Space)),
            AnonymousMethodExpressionSyntax anonymousMethod => anonymousMethod.AsyncKeyword.IsKind(SyntaxKind.AsyncKeyword)
                ? anonymousMethod
                : anonymousMethod.WithAsyncKeyword(SyntaxFactory.Token(SyntaxKind.AsyncKeyword).WithTrailingTrivia(SyntaxFactory.Space)),
            _ => node
        };
    }

    private static MethodDeclarationSyntax MakeMethodAsync(MethodDeclarationSyntax method)
    {
        var updated = method;

        if (!updated.Modifiers.Any(SyntaxKind.AsyncKeyword))
        {
            updated = updated.WithModifiers(updated.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword).WithTrailingTrivia(SyntaxFactory.Space)));
        }

        if (updated.ReturnType is PredefinedTypeSyntax predefined && predefined.Keyword.IsKind(SyntaxKind.VoidKeyword))
        {
            updated = updated.WithReturnType(
                SyntaxFactory.ParseTypeName("global::System.Threading.Tasks.Task")
                    .WithTrailingTrivia(SyntaxFactory.Space));
        }

        return updated;
    }

    private static LocalFunctionStatementSyntax MakeLocalFunctionAsync(LocalFunctionStatementSyntax localFunction)
    {
        var updated = localFunction;

        if (!updated.Modifiers.Any(SyntaxKind.AsyncKeyword))
        {
            updated = updated.WithModifiers(updated.Modifiers.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword).WithTrailingTrivia(SyntaxFactory.Space)));
        }

        if (updated.ReturnType is PredefinedTypeSyntax predefined && predefined.Keyword.IsKind(SyntaxKind.VoidKeyword))
        {
            updated = updated.WithReturnType(
                SyntaxFactory.ParseTypeName("global::System.Threading.Tasks.Task")
                    .WithTrailingTrivia(SyntaxFactory.Space));
        }

        return updated;
    }
}
