# AutoAssert.Generator

A Roslyn incremental source generator for [AutoAssert](https://www.nuget.org/packages/Swevo.AutoAssert/)
that generates a **reflection-free** `BeEquivalentTo` comparer for your types at compile time.

## Why

AutoAssert's `BeEquivalentTo` normally walks an object graph with reflection (cached
`PropertyInfo`/`FieldInfo` lookups). That's fast enough for almost everyone, but reflection:

- Adds a small but nonzero per-property cost on the hot path of large test suites.
- Doesn't work at all in fully trimmed/Native AOT test scenarios that disable reflection metadata.

`AutoAssert.Generator` removes both problems for the types you opt in: it emits plain C# that
reads properties/fields directly and compares them with `EqualityComparer<T>.Default`, then
registers itself automatically — no manual wiring, no attributes on `BeEquivalentTo` call sites.

## Usage

```bash
dotnet add package Swevo.AutoAssert.Generator
```

```csharp
[AutoAssert.GenerateEquivalencyComparer]
public class OrderDto
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = "";
    public decimal Total { get; set; }
}
```

That's it — the next time both sides of a `BeEquivalentTo` call are exactly `OrderDto`, AutoAssert
uses the generated comparer instead of reflection:

```csharp
actual.Should().BeEquivalentTo(expected); // reflection-free for OrderDto
```

## Scope and limitations

- Only applies when **both sides of the comparison are the exact same registered type** —
  duck-typed/anonymous-object comparisons and comparisons involving a base/derived type mismatch
  always fall back to (still fully-supported) reflection.
- Nested types and open generic types are not supported by this generator; annotate the top-level,
  non-generic type instead — the reflection engine handles the rest automatically.
- Members are compared with `EqualityComparer<T>.Default`, matching AutoAssert's reflection engine
  for "simple" (value-typed) members; complex/reference-typed and collection members recurse
  through the normal engine (which itself prefers a generated comparer if that member's type is
  also annotated).
- Circular-reference protection does not extend across the generated/reflection boundary — this
  only matters for object graphs with actual cycles, which real-world DTOs rarely have.
- Registration happens via a `[ModuleInitializer]`-attributed method, which runs automatically on
  .NET 5+ runtimes when the assembly is loaded. If you target an older runtime (e.g. .NET
  Framework via netstandard2.0) where module initializers aren't executed, call the generated
  `<YourType>_AutoAssertEquivalencyComparer.Register()` method once yourself (e.g. from a test
  assembly's module setup).

## Install

This is a development-only dependency (`PrivateAssets="all"` in the generated reference) — it
never ships in your build output; only the generated source becomes part of your compilation.

## License

MIT © Justin Bannister
