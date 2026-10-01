# AutoAssert

[![NuGet](https://img.shields.io/nuget/v/Swevo.AutoAssert.svg)](https://www.nuget.org/packages/Swevo.AutoAssert/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Swevo.AutoAssert.svg)](https://www.nuget.org/packages/Swevo.AutoAssert/)
[![CI](https://github.com/Swevo/AutoAssert/actions/workflows/build.yml/badge.svg)](https://github.com/Swevo/AutoAssert/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-netstandard2.0%20%7C%20net8.0%20%7C%20net9.0%20%7C%20net10.0-512BD4?logo=dotnet)](docs/COMPATIBILITY.md)

**Free, MIT-licensed fluent assertions for .NET** with familiar `Should()` syntax, deep object diffs, snapshot testing, and zero commercial license requirements.

## Why AutoAssert instead of FluentAssertions?

- **No commercial license requirement** for commercial use.
- **Familiar migration path**: fluent `Should()` API for xUnit, NUnit, and MSTest.
- **Deep diff output** for `BeEquivalentTo` mismatches.
- **AOT-safe direction** with optional generated reflection-free equivalency comparers.

## Install

```bash
dotnet add package Swevo.AutoAssert
```

Optional companion packages:

```bash
dotnet add package Swevo.AutoAssert.Analyzers
dotnet add package Swevo.AutoAssert.Generator
dotnet add package Swevo.AutoAssert.AspNetCore
dotnet add package Swevo.AutoAssert.Json
```

## 5-minute quickstart

```csharp
using AutoAssert;

[Fact]
public void Order_total_is_calculated()
{
    var order = new { Subtotal = 100m, Tax = 20m, Total = 120m };

    order.Total.Should().Be(120m);
    order.Should().BeEquivalentTo(new { Subtotal = 100m, Tax = 20m, Total = 120m });
}
```

## High-value examples

### 1) Basic assertions and fluent chaining

```csharp
using AutoAssert;

"hello world".Should()
    .NotBeNullOrEmpty()
    .And.StartWith("hello")
    .And.EndWith("world");

5.Should().BePositive().And.BeLessThan(10);
```

### 2) Compare complex object graphs with full diffs

```csharp
using AutoAssert;

var actual = new OrderDto
{
    Id = 42,
    Customer = new CustomerDto { Name = "Ada" },
    Lines = [new OrderLineDto { Sku = "ABC", Quantity = 2 }]
};

var expected = new
{
    Id = 42,
    Customer = new { Name = "Ada" },
    Lines = new[] { new { Sku = "ABC", Quantity = 2 } }
};

actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
```

### 3) Aggregate failures with `AssertionScope`

```csharp
using AutoAssert;

using (new AssertionScope())
{
    result.Name.Should().Be("Ada");
    result.Age.Should().Be(30);
    result.Email.Should().NotBeNullOrEmpty();
}
```

### 4) Snapshot tests with scrubbers

```csharp
using AutoAssert;

result.Should().MatchSnapshot(
    scrub: SnapshotScrubbers.Combine(
        SnapshotScrubbers.Guids(),
        SnapshotScrubbers.IsoTimestamps()));
```

## Migration comparison

AutoAssert is designed to keep common usage nearly identical:

| Scenario | FluentAssertions style | AutoAssert style |
|---|---|---|
| Basic value | `value.Should().Be(42);` | `value.Should().Be(42);` |
| Null/empty | `name.Should().NotBeNullOrEmpty();` | `name.Should().NotBeNullOrEmpty();` |
| Exception | `act.Should().Throw<InvalidOperationException>();` | `act.Should().Throw<InvalidOperationException>();` |
| Equivalency | `actual.Should().BeEquivalentTo(expected);` | `actual.Should().BeEquivalentTo(expected);` |

See deeper comparisons in [`docs/COMPARISON.md`](docs/COMPARISON.md).

## Recipes

Copy/paste recipes for common test scenarios:

- [`docs/RECIPES.md`](docs/RECIPES.md) (collections, API payloads, async exceptions, snapshots)
- [`src/AutoAssert.AspNetCore/README.md`](src/AutoAssert.AspNetCore/README.md) (HTTP integration tests)
- [`src/AutoAssert.Json/README.md`](src/AutoAssert.Json/README.md) (raw JSON assertions)

## Companion packages

| Package | Purpose |
|---|---|
| [`Swevo.AutoAssert.Analyzers`](https://www.nuget.org/packages/Swevo.AutoAssert.Analyzers) | Compile-time diagnostics for assertion footguns. |
| [`Swevo.AutoAssert.Generator`](https://www.nuget.org/packages/Swevo.AutoAssert.Generator) | Reflection-free generated equivalency comparers. |
| [`Swevo.AutoAssert.AspNetCore`](https://www.nuget.org/packages/Swevo.AutoAssert.AspNetCore) | Fluent `HttpResponseMessage` assertions for integration tests. |
| [`Swevo.AutoAssert.Json`](https://www.nuget.org/packages/Swevo.AutoAssert.Json) | Fluent assertions over raw JSON strings. |

## Compatibility matrix

See [`docs/COMPATIBILITY.md`](docs/COMPATIBILITY.md) for package-by-package target frameworks and test framework support.

## Release notes

See [`CHANGELOG.md`](CHANGELOG.md) for version history and feature changes.

## Growth and distribution assets

The repository now includes reusable go-to-market assets:

- [`docs/NUGET-LISTING.md`](docs/NUGET-LISTING.md) for listing copy and metadata guidance.
- [`docs/DISTRIBUTION.md`](docs/DISTRIBUTION.md) for release promotion checklist.
- [`docs/METRICS.md`](docs/METRICS.md) for a simple download and retention measurement model.

## License

MIT © Justin Bannister
