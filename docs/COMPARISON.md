# AutoAssert vs alternatives

## FluentAssertions migration fit

| Area | AutoAssert |
|---|---|
| Licensing | MIT, no commercial tier |
| Syntax familiarity | Fluent `Should()` style |
| Test framework support | xUnit, NUnit, MSTest |
| Deep object diffs | `BeEquivalentTo` reports full mismatch sets |
| Snapshot support | Built-in via `MatchSnapshot()` |
| Compile-time safety add-ons | Roslyn analyzers package |
| AOT direction | Optional generated equivalency comparers |

## Before vs after (manual assertions)

### Before (manual)

```csharp
Assert.Equal(42, order.Id);
Assert.Equal("Ada", order.Customer.Name);
Assert.Equal(2, order.Lines.Count);
Assert.Equal("ABC", order.Lines[0].Sku);
Assert.Equal(2, order.Lines[0].Quantity);
```

### After (AutoAssert)

```csharp
order.Should().BeEquivalentTo(new
{
    Id = 42,
    Customer = new { Name = "Ada" },
    Lines = new[] { new { Sku = "ABC", Quantity = 2 } }
});
```

## Why this improves maintenance

- Fewer brittle assertions when DTOs evolve.
- Rich diff output reduces debugging time.
- Shared assertion style across unit and integration tests.
