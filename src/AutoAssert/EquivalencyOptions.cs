using System.Linq.Expressions;

namespace AutoAssert;

/// <summary>
/// Configures a single <c>BeEquivalentTo(expected, config =&gt; ...)</c> call: exclude specific
/// members from comparison, or require collections to match in exact order.
/// </summary>
public sealed class EquivalencyOptions
{
    internal static readonly EquivalencyOptions Default = new();

    private readonly HashSet<string> _excludedMemberNames = new(StringComparer.Ordinal);

    /// <summary>When set, collection members must match item-for-item in the given order (default: order-independent).</summary>
    public bool StrictOrdering { get; private set; }

    /// <summary>Excludes a member by name (matched at any depth in the object graph) from comparison.</summary>
    public EquivalencyOptions Excluding(string memberName)
    {
        _excludedMemberNames.Add(memberName);
        return this;
    }

    /// <summary>Excludes a member, identified via a simple member-access expression (e.g. <c>x =&gt; x.Id</c>), from comparison.</summary>
    public EquivalencyOptions Excluding<T, TMember>(Expression<Func<T, TMember>> memberExpression)
    {
        _excludedMemberNames.Add(GetMemberName(memberExpression));
        return this;
    }

    /// <summary>Requires collection members to appear in the same order instead of the default order-independent matching.</summary>
    public EquivalencyOptions WithStrictOrdering()
    {
        StrictOrdering = true;
        return this;
    }

    internal bool IsExcluded(string memberName) => _excludedMemberNames.Contains(memberName);

    private static string GetMemberName<T, TMember>(Expression<Func<T, TMember>> expression)
    {
        var body = expression.Body;
        if (body is UnaryExpression { Operand: MemberExpression unaryMember })
        {
            return unaryMember.Member.Name;
        }

        if (body is MemberExpression member)
        {
            return member.Member.Name;
        }

        throw new ArgumentException("Expression must be a simple member access, e.g. x => x.PropertyName.", nameof(expression));
    }
}
