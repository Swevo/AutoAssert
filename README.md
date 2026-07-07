# AutoAssert

[![NuGet](https://img.shields.io/nuget/v/AutoAssert.svg)](https://www.nuget.org/packages/AutoAssert/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/AutoAssert.svg)](https://www.nuget.org/packages/AutoAssert/)
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
dotnet add package AutoAssert
```

## Supported assertions

| Type | Examples |
|---|---|
| Objects | `Be`, `NotBe`, `BeNull`, `NotBeNull`, `BeSameAs`, `BeOfType<T>`, `BeAssignableTo<T>`, `Match` |
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

## Design goals

- **MIT licensed, forever.** No commercial tier, no per-seat fees.
- **Zero reflection** in the hot assertion path — plain equality/comparison checks.
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
| [**AutoWire**](https://github.com/Swevo/AutoWire) | Compile-time DI auto-registration — `[Scoped]`/`[Singleton]`/`[Transient]` generates `IServiceCollection` registration code. |
| [**AutoMap.Generator**](https://github.com/Swevo/AutoMap.Generator) | Compile-time object mapping — `[Map(typeof(Dto))]` generates `ToDto()` extension methods. |
| [**AutoValidate.Generator**](https://github.com/Swevo/AutoValidate.Generator) | Compile-time FluentValidation wiring. |
| [**AutoResult.Generator**](https://github.com/Swevo/AutoResult.Generator) | Compile-time `Result<T>` monad. |
| [**AutoDispatch.Generator**](https://github.com/Swevo/AutoDispatch.Generator) | Compile-time CQRS dispatcher — free alternative to MediatR's commercial license. |

## License

MIT © Justin Bannister
