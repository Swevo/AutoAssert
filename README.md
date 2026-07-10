# AutoAssert

[![NuGet](https://img.shields.io/nuget/v/Swevo.AutoAssert.svg)](https://www.nuget.org/packages/Swevo.AutoAssert/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Swevo.AutoAssert.svg)](https://www.nuget.org/packages/Swevo.AutoAssert/)
[![CI](https://github.com/Swevo/AutoAssert/actions/workflows/build.yml/badge.svg)](https://github.com/Swevo/AutoAssert/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**Free, MIT-licensed fluent assertions for .NET.** No commercial license required — ever.

## Why AutoAssert?

Starting with v8, **FluentAssertions requires a paid commercial license** for use in commercial
projects (via its Xceed partnership). AutoAssert provides the same fluent `Should()` syntax
you already know, fully free and open source under MIT, for teams who don't want licensing
fees attached to their test suite.

```csharp
using AutoAssert;

result.Should().Be(42);
name.Should().NotBeNullOrEmpty();
items.Should().HaveCount(3);
action.Should().Throw<InvalidOperationException>().WithMessage("boom");
```

## Install

```bash
dotnet add package Swevo.AutoAssert
```

## Supported assertions

| Type | Examples |
|---|---|
| Objects | `Be`, `NotBe`, `BeNull`, `NotBeNull`, `BeSameAs`, `BeOfType<T>`, `BeAssignableTo<T>`, `Match`, `BeEquivalentTo` |
| Strings | `Be`, `Contain`, `StartWith`, `EndWith`, `BeNullOrEmpty`, `HaveLength`, `MatchRegex` |
| Booleans | `BeTrue`, `BeFalse` |
| Numerics (int/long/short/byte/double/float/decimal) | `Be`, `BeGreaterThan`, `BeLessThan`, `BeInRange`, `BeApproximately` |
| Collections | `HaveCount`, `Contain`, `BeEquivalentTo`, `Equal`, `ContainSingle`, `OnlyHaveUniqueItems`, `AllSatisfy` |
| Exceptions | `Throw<T>`, `ThrowAsync<T>`, `NotThrow`, `NotThrowAsync`, `WithMessage`, `WithInnerException<T>` |

Every assertion accepts an optional `because` reasoning clause, matching the syntax you're
used to:

```csharp
result.Should().Be(42, "the answer should always be 42");
```

## BeEquivalentTo

`BeEquivalentTo` performs deep structural comparison of public readable properties and public fields.
It works for plain objects, nested object graphs, and collections of objects.

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

actual.Should().BeEquivalentTo(expected);
```

Current v1 scope:

- compares public readable properties and public fields recursively
- treats collections as **order-independent** by default
- uses value equality for primitives, strings, enums, dates, GUIDs, and other value types
- ignores extra public members on the actual value when the expected value has fewer members
- protects against infinite recursion on circular object graphs

Limitations versus FluentAssertions:

- no options API yet (`Excluding`, `WithStrictOrdering`, custom comparers, etc.)
- failure output stops at the first difference instead of producing a full diff
- this assertion uses reflection for member traversal, while most other assertions remain direct non-reflective checks

## Design goals

- **MIT licensed, forever.** No commercial tier, no per-seat fees.
- **Zero reflection where possible** — most assertions are plain equality/comparison checks; `BeEquivalentTo` uses public-member traversal.
- **AOT-safe** — works with Native AOT test hosts.
- **Framework agnostic** — throws a plain `AssertionFailedException`, recognized as a failure
  by xUnit, NUnit, and MSTest alike.
- **Familiar syntax** — migrating from FluentAssertions should mostly be a find-and-replace of
  the `using` statement for the assertion types covered above.

## 💼 Need .NET consulting?

I'm the author of AutoAssert and a suite of compile-time source generators
([AutoWire](https://github.com/Swevo/AutoWire), [AutoMap.Generator](https://github.com/Swevo/AutoMap.Generator))
and 28+ Polly v8 resilience packages. I'm available for consulting on **Polly v8 resilience**,
**Azure cloud architecture**, and **clean .NET design**.

**[→ solidqualitysolutions.com](https://www.solidqualitysolutions.com/)** · **[LinkedIn](https://www.linkedin.com/in/justbannister/)**

## Also by the same author

> 🌐 Full suite overview: **[swevo.github.io](https://swevo.github.io/)**

| Package | Description |
|---|---|
| [**FluentPdf**](https://github.com/Swevo/FluentPdf) | Free, MIT-licensed fluent PDF generation — alternative to QuestPDF's commercial license. |
| [**AutoBus**](https://github.com/Swevo/AutoBus) | Free, MIT-licensed message bus — alternative to MassTransit's commercial license. |
| [**AutoArchitecture**](https://github.com/Swevo/AutoArchitecture) | Free, MIT-licensed compile-time architecture rule enforcement — alternative to NDepend. |
| [**EFCore.BulkOperations**](https://github.com/Swevo/EFCore.BulkOperations) | Free, MIT-licensed bulk insert/update/delete for EF Core. |
| [**AutoWire**](https://github.com/Swevo/AutoWire) | Compile-time DI auto-registration — `[Scoped]`/`[Singleton]`/`[Transient]` generates `IServiceCollection` registration code. |
| [**AutoMap.Generator**](https://github.com/Swevo/AutoMap.Generator) | Compile-time object mapping — `[Map(typeof(Dto))]` generates `ToDto()` extension methods. |
| [**AutoValidate.Generator**](https://github.com/Swevo/AutoValidate.Generator) | Compile-time FluentValidation wiring. |
| [**AutoResult.Generator**](https://github.com/Swevo/AutoResult.Generator) | Compile-time `Result<T>` monad. |
| [**AutoDispatch.Generator**](https://github.com/Swevo/AutoDispatch.Generator) | Compile-time CQRS dispatcher — free alternative to MediatR's commercial license. |
| [**PollyAnalyzers**](https://github.com/Swevo/PollyAnalyzers) | Free Roslyn analyzers for async/resilience anti-patterns — blocking calls, async void, fire-and-forget tasks, swallowed exceptions. |
| [**PollyAction**](https://github.com/Swevo/PollyAction) | Free retry/backoff GitHub Action — wrap any CI step with exponential-backoff retries. |

## License

MIT © Justin Bannister
