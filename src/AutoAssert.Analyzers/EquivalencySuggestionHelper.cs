using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoAssert.Analyzers;

internal static class EquivalencySuggestionHelper
{
    internal sealed class CandidateAssertion
    {
        public CandidateAssertion(
            ExpressionStatementSyntax statement,
            string actualRoot,
            string propertyName,
            string expectedRoot,
            ExpressionSyntax expectedExpression)
        {
            Statement = statement;
            ActualRoot = actualRoot;
            PropertyName = propertyName;
            ExpectedRoot = expectedRoot;
            ExpectedExpression = expectedExpression;
        }

        public ExpressionStatementSyntax Statement { get; }
        public string ActualRoot { get; }
        public string PropertyName { get; }
        public string ExpectedRoot { get; }
        public ExpressionSyntax ExpectedExpression { get; }
    }

    internal sealed class CandidateGroup
    {
        public CandidateGroup(string actualRoot, string expectedRoot, IReadOnlyList<CandidateAssertion> assertions)
        {
            ActualRoot = actualRoot;
            ExpectedRoot = expectedRoot;
            Assertions = assertions;
        }

        public string ActualRoot { get; }
        public string ExpectedRoot { get; }
        public IReadOnlyList<CandidateAssertion> Assertions { get; }
    }

    public static IReadOnlyList<CandidateGroup> FindGroups(BlockSyntax block, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        var candidates = new List<CandidateAssertion>();

        foreach (var statement in block.Statements.OfType<ExpressionStatementSyntax>())
        {
            if (TryGetCandidate(statement, semanticModel, cancellationToken, out var candidate))
            {
                candidates.Add(candidate);
            }
        }

        var groups = candidates
            .GroupBy(c => (c.ActualRoot, c.ExpectedRoot))
            .Select(group => new CandidateGroup(
                group.Key.ActualRoot,
                group.Key.ExpectedRoot,
                group.OrderBy(c => c.Statement.SpanStart).ToList()))
            .Where(group => group.Assertions.Select(a => a.PropertyName).Distinct(StringComparer.Ordinal).Count() >= 3)
            .Where(group => !HasExistingBeEquivalentTo(block, semanticModel, cancellationToken, group.ActualRoot))
            .ToList();

        return groups;
    }

    private static bool TryGetCandidate(
        ExpressionStatementSyntax statement,
        SemanticModel semanticModel,
        CancellationToken cancellationToken,
        out CandidateAssertion candidate)
    {
        candidate = null!;

        if (statement.Expression is not InvocationExpressionSyntax beInvocation)
        {
            return false;
        }

        if (semanticModel.GetSymbolInfo(beInvocation, cancellationToken).Symbol is not IMethodSymbol beSymbol
            || beSymbol.Name != "Be"
            || beSymbol.ContainingType?.ContainingNamespace?.ToDisplayString() != "AutoAssert")
        {
            return false;
        }

        if (beInvocation.ArgumentList.Arguments.Count != 1)
        {
            return false;
        }

        if (beInvocation.Expression is not MemberAccessExpressionSyntax
            {
                Expression: InvocationExpressionSyntax shouldInvocation
            })
        {
            return false;
        }

        if (shouldInvocation.Expression is not MemberAccessExpressionSyntax
            {
                Name.Identifier.Text: "Should",
                Expression: MemberAccessExpressionSyntax
                {
                    Expression: IdentifierNameSyntax actualRootIdentifier,
                    Name: IdentifierNameSyntax propertyIdentifier
                }
            })
        {
            return false;
        }

        var expectedExpression = beInvocation.ArgumentList.Arguments[0].Expression;
        if (expectedExpression is not MemberAccessExpressionSyntax
            {
                Expression: IdentifierNameSyntax expectedRootIdentifier,
                Name: IdentifierNameSyntax expectedPropertyIdentifier
            })
        {
            return false;
        }

        if (!string.Equals(propertyIdentifier.Identifier.ValueText, expectedPropertyIdentifier.Identifier.ValueText, StringComparison.Ordinal))
        {
            return false;
        }

        candidate = new CandidateAssertion(
            statement,
            actualRootIdentifier.Identifier.ValueText,
            propertyIdentifier.Identifier.ValueText,
            expectedRootIdentifier.Identifier.ValueText,
            expectedExpression);
        return true;
    }

    private static bool HasExistingBeEquivalentTo(
        BlockSyntax block,
        SemanticModel semanticModel,
        CancellationToken cancellationToken,
        string actualRoot)
    {
        foreach (var statement in block.Statements.OfType<ExpressionStatementSyntax>())
        {
            if (statement.Expression is not InvocationExpressionSyntax invocation)
            {
                continue;
            }

            if (semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is not IMethodSymbol symbol
                || symbol.Name != "BeEquivalentTo"
                || symbol.ContainingType?.ContainingNamespace?.ToDisplayString() != "AutoAssert")
            {
                continue;
            }

            if (invocation.Expression is not MemberAccessExpressionSyntax
                {
                    Expression: InvocationExpressionSyntax shouldInvocation
                })
            {
                continue;
            }

            if (shouldInvocation.Expression is not MemberAccessExpressionSyntax
                {
                    Name.Identifier.Text: "Should",
                    Expression: IdentifierNameSyntax actualRootIdentifier
                })
            {
                continue;
            }

            if (string.Equals(actualRootIdentifier.Identifier.ValueText, actualRoot, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }
}
