namespace AutoAssert;

/// <summary>
/// Returned by every assertion method so multiple assertions can be chained fluently via
/// <see cref="And"/>, e.g. <c>value.Should().NotBeNull().And.BeOfType&lt;Foo&gt;()</c>.
/// </summary>
public readonly struct AndConstraint<TAssertions>
{
    /// <summary>The assertions object to continue chaining from.</summary>
    public TAssertions And { get; }

    public AndConstraint(TAssertions assertions) => And = assertions;
}
