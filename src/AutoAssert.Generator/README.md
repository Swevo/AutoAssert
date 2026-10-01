# Swevo.AutoAssert.Generator

Compile-time generated comparers for `BeEquivalentTo` to reduce reflection and improve AOT friendliness.

## Install

```bash
dotnet add package Swevo.AutoAssert.Generator
```

## Quickstart

```csharp
[AutoAssert.GenerateEquivalencyComparer]
public sealed class OrderDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = "";
    public decimal Total { get; set; }
}
```

```csharp
actual.Should().BeEquivalentTo(expected); // uses generated comparer when both sides are OrderDto
```

## Notes

- Generated comparer is used when both compared values are the same registered type.
- Non-matching type pairs and unsupported cases safely fall back to core AutoAssert behavior.

## License

MIT © Justin Bannister
