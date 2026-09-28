# AutoAssert

[![NuGet](https://img.shields.io/nuget/v/Swevo.AutoAssert.svg)](https://www.nuget.org/packages/Swevo.AutoAssert/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Swevo.AutoAssert.svg)](https://www.nuget.org/packages/Swevo.AutoAssert/)
[![CI](https://github.com/Swevo/AutoAssert/actions/workflows/build.yml/badge.svg)](https://github.com/Swevo/AutoAssert/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET 10 Ready](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](#)

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

## Fluent chaining

Every assertion method returns an `AndConstraint<T>`, so you can chain multiple checks on the
same subject with `.And`:

```csharp
"hello world".Should()
    .NotBeNullOrEmpty()
    .And.StartWith("hello")
    .And.EndWith("world")
    .And.Contain("lo wo");

5.Should().BePositive().And.BeLessThan(10);
```

> **Breaking change note (v2.0.0):** assertion methods used to return `void`; they now return
> `AndConstraint<T>`. Existing call sites keep working unchanged (the return value can simply
> be discarded) — only code that explicitly typed a variable/delegate as `void` around a call
> would need updating, which is extremely rare for fluent assertion call sites.

## AssertionScope

Wrap multiple assertions in an `AssertionScope` to collect **all** failures and report them
together in a single exception, instead of stopping at the first one:

```csharp
using (new AssertionScope())
{
    result.Name.Should().Be("Ada");
    result.Age.Should().Be(30);
    result.Email.Should().NotBeNullOrEmpty();
} // throws one AssertionFailedException listing every failure, if any occurred
```

## Install

```bash
dotnet add package Swevo.AutoAssert
```

Optionally add the companion Roslyn analyzers, which catch common fluent-assertion mistakes at
compile time (e.g. a bare `subject.Should();` that asserts nothing, or an unawaited
`ThrowAsync`/`NotThrowAsync`):

```bash
dotnet add package Swevo.AutoAssert.Analyzers
```

See [`src/AutoAssert.Analyzers/README.md`](src/AutoAssert.Analyzers/README.md) for the full diagnostic list.

## Supported assertions

| Type | Examples |
|---|---|
| Objects | `Be`, `NotBe`, `BeNull`, `NotBeNull`, `BeSameAs`, `BeOfType<T>`, `BeAssignableTo<T>`, `Match`, `BeEquivalentTo` |
| Strings | `Be`, `Contain`, `StartWith`, `EndWith`, `NotStartWith`, `NotEndWith`, `BeNullOrEmpty`, `HaveLength`, `MatchRegex`, `NotMatchRegex`, `BeEquivalentTo`, `ContainEquivalentOf`, `Match` (wildcards), `BeUpperCased`, `BeLowerCased` |
| Booleans | `BeTrue`, `BeFalse` |
| Numerics (int/long/short/byte/uint/ulong/ushort/sbyte/double/float/decimal) | `Be`, `BeGreaterThan`, `BeLessThan`, `BeInRange`, `BeApproximately`, `BePositive`, `BeNegative`, `BeOneOf`, `BeNaN`/`NotBeNaN` (double/float) |
| Collections | `HaveCount`, `HaveCountGreaterThan`, `HaveCountLessThan`, `Contain`, `Contain(predicate)`, `ContainInOrder`, `BeEquivalentTo`, `Equal`, `ContainSingle`, `OnlyHaveUniqueItems`, `AllSatisfy`, `SatisfyRespectively`, `BeInAscendingOrder`, `BeInDescendingOrder` |
| Dictionaries | `ContainKey`, `NotContainKey`, `ContainValue`, `NotContainValue`, `ContainKeyAndValue`, `HaveCount`, `BeEmpty`, `NotBeEmpty` |
| Exceptions | `Throw<T>`, `ThrowAsync<T>`, `NotThrow`, `NotThrow<T>`, `NotThrowAsync`, `WithMessage`, `WithInnerException<T>`, `Where(predicate)`, `WithParameterName` |
| Value-returning functions | `Func<T>.Should().Throw<TException>()/.NotThrow()`, `Func<Task<T>>.Should().ThrowAsync<TException>()/.NotThrowAsync()` (both return the resolved value) |
| Dates/times | `DateTime`/`DateTimeOffset`: `Be`, `NotBe`, `BeBefore`, `BeAfter`, `BeOnOrBefore`, `BeOnOrAfter`, `BeCloseTo`, `BeSameDateAs`. `TimeSpan`: `Be`, `BeGreaterThan`, `BeLessThan`, `BeCloseTo` |
| Guid | `Be`, `NotBe`, `BeEmpty`, `NotBeEmpty` |
| Nullable&lt;T&gt; | `HaveValue`, `NotHaveValue`, `Be`, `NotBe` |
| Enums | `Be`, `NotBe`, `HaveFlag`, `NotHaveFlag` |
| Execution time | `action.ExecutionTime().Should().BeLessThan/BeLessOrEqualTo/BeGreaterThan`, `action.Should().CompleteWithin(TimeSpan)`, `func.Should().CompleteWithinAsync(TimeSpan)` |

Every assertion accepts an optional `because` reasoning clause, matching the syntax you're
used to:

```csharp
result.Should().Be(42, "the answer should always be 42");
```

## BeEquivalentTo

`BeEquivalentTo` performs deep structural comparison of public readable properties and public fields.
It works for plain objects, nested object graphs, and collections of objects, and reports a
**full diff** of every mismatched member (not just the first one found).

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

Configure the comparison with an options callback:

```csharp
actual.Should().BeEquivalentTo(expected, options => options
    .Excluding(x => x.Id)
    .Excluding("SomeFieldName")
    .WithStrictOrdering()); // require collections to match in the same order
```

Current scope:

- compares public readable properties and public fields recursively, reporting every mismatched
  member (not only the first) at any depth
- treats collections as **order-independent** by default; `WithStrictOrdering()` switches to
  positional (order-dependent) comparison
- for order-independent collections, when no perfect item-to-item matching exists, every expected
  item's full diff is reported (via a greedy best-fit pairing), not just the first blocking mismatch
- `Excluding(...)` skips named members or members selected via an expression, at the top level
- uses value equality for primitives, strings, enums, dates, GUIDs, and other value types
- ignores extra public members on the actual value when the expected value has fewer members
- protects against infinite recursion on circular object graphs

Remaining limitation versus FluentAssertions: the greedy best-fit pairing used for reporting
order-independent collection diffs isn't guaranteed globally optimal (unlike the exhaustive
backtracking search used to detect whether a perfect match exists at all) — in rare cases with
many near-duplicate items it may not report the *minimal* possible diff, though it always reports
a diff for every expected item.

### Global equivalency defaults

Configure `BeEquivalentTo` options once for the whole test suite (e.g. always excluding an
audit-tracking `Id`/`CreatedAt` member), instead of repeating the same options at every call
site:

```csharp
// e.g. in a test assembly module initializer or a shared fixture's constructor
AssertionConfig.ConfigureEquivalency(options => options
    .Excluding("Id")
    .Excluding("CreatedAt"));
```

Every `BeEquivalentTo(expected)` call that doesn't specify its own options callback picks up
these defaults automatically. A call that *does* provide `options => ...` still starts from the
global defaults and layers its own configuration on top. `AssertionConfig.ResetEquivalencyDefaults()`
restores the out-of-the-box (no exclusions, order-independent) behavior.

> **Note:** this is process-wide mutable state, same trade-off FluentAssertions' own
> `AssertionOptions` has — avoid relying on it in test suites that run test classes in parallel
> with per-class differing configuration, since configuration set by one test can affect another
> running concurrently.

## Snapshot testing

`MatchSnapshot()` serializes the subject to indented JSON and compares it against a baseline file
stored in a `__snapshots__` folder next to the calling test file — no separate snapshot library
needed:

```csharp
var result = BuildOrderSummary();

result.Should().MatchSnapshot();
```

- **First run**: no baseline exists yet, so one is written (`__snapshots__/{TestMethodName}.snapshot.json`)
  and the assertion passes. Commit this file to source control as your golden file.
- **Subsequent runs**: the current value is compared against the committed baseline; a mismatch
  throws an `AssertionFailedException` showing both the expected (baseline) and actual JSON.
- Use `MatchSnapshot("someName")` to take multiple named snapshots within a single test method.
- If a change is intentional, delete the corresponding `__snapshots__/*.snapshot.json` file and
  re-run the test to record a new baseline.
- Pass `scrub:` to normalize non-deterministic values (generated GUIDs, "now"-based timestamps)
  before comparison, so they don't cause spurious snapshot failures:

```csharp
result.Should().MatchSnapshot(
    scrub: SnapshotScrubbers.Combine(SnapshotScrubbers.Guids(), SnapshotScrubbers.IsoTimestamps()));
```

## Custom assertions

Every assertion type is a public struct, so you can extend AutoAssert with your own domain-specific
assertions exactly like the built-in `FloatingPointAssertionExtensions` does for `NumericAssertions<T>`:

```csharp
public static class MyCustomAssertionExtensions
{
    public static AndConstraint<StringAssertions> BeAValidSku(this StringAssertions assertions, string because = "", params object[] becauseArgs)
    {
        // use AssertionHelpers.Fail(...) to report failures consistently (respects AssertionScope)
        return new AndConstraint<StringAssertions>(assertions);
    }
}
```

## Companion packages

| Package | Description |
|---|---|
| [`Swevo.AutoAssert.Analyzers`](src/AutoAssert.Analyzers) | Dev-only Roslyn analyzers that catch discarded `.Should()` calls and unawaited async assertions at compile time. |
| [`Swevo.AutoAssert.Generator`](src/AutoAssert.Generator) | Source generator that emits a reflection-free `BeEquivalentTo` comparer for types marked `[GenerateEquivalencyComparer]` — faster and AOT-friendly. |
| [`Swevo.AutoAssert.AspNetCore`](src/AutoAssert.AspNetCore) | Fluent `HttpResponseMessage` assertions (status code, headers, content type, string/JSON body) for `WebApplicationFactory`/`HttpClient` integration tests. |
| [`Swevo.AutoAssert.Json`](src/AutoAssert.Json) | Fluent assertions on raw JSON strings (`BeValidJson`, `HaveJsonProperty`, `BeEquivalentToJson`) with full structural diffs. |

## Design goals

- **MIT licensed, forever.** No commercial tier, no per-seat fees.
- **Zero reflection where possible** — most assertions are plain equality/comparison checks; `BeEquivalentTo` uses public-member traversal.
- **AOT-safe** — works with Native AOT test hosts.
- **Framework agnostic** — throws a plain `AssertionFailedException`, recognized as a failure
  by xUnit, NUnit, and MSTest alike.
- **Familiar syntax** — migrating from FluentAssertions should mostly be a find-and-replace of
  the `using` statement for the assertion types covered above.

## Related Packages

| Package | Downloads | Description |
|---|---|---|
| [Swevo.AutoBus](https://www.nuget.org/packages/Swevo.AutoBus) | [![Downloads](https://img.shields.io/nuget/dt/Swevo.AutoBus.svg)](https://www.nuget.org/packages/Swevo.AutoBus) | Free, MIT-licensed in-process message bus for  |
| [Swevo.AutoBus.RabbitMQ](https://www.nuget.org/packages/Swevo.AutoBus.RabbitMQ) | [![Downloads](https://img.shields.io/nuget/dt/Swevo.AutoBus.RabbitMQ.svg)](https://www.nuget.org/packages/Swevo.AutoBus.RabbitMQ) | RabbitMQ transport for AutoBus |
| [Swevo.AutoAuth](https://www.nuget.org/packages/Swevo.AutoAuth) | [![Downloads](https://img.shields.io/nuget/dt/Swevo.AutoAuth.svg)](https://www.nuget.org/packages/Swevo.AutoAuth) | A free, MIT-licensed fluent configuration wrapper around OpenIddict for building OAuth2/OIDC token servers in ASP |
| [Swevo.AutoAudit](https://www.nuget.org/packages/Swevo.AutoAudit) | [![Downloads](https://img.shields.io/nuget/dt/Swevo.AutoAudit.svg)](https://www.nuget.org/packages/Swevo.AutoAudit) | Compile-time audit field generation for EF Core entities using Roslyn source generators |
| [Swevo.AutoResult](https://www.nuget.org/packages/Swevo.AutoResult) | [![Downloads](https://img.shields.io/nuget/dt/Swevo.AutoResult.svg)](https://www.nuget.org/packages/Swevo.AutoResult) | Compile-time Result<T> monad for  |
| [Swevo.AutoGuard](https://www.nuget.org/packages/Swevo.AutoGuard) | [![Downloads](https://img.shields.io/nuget/dt/Swevo.AutoGuard.svg)](https://www.nuget.org/packages/Swevo.AutoGuard) | Compile-time guard clauses for  |
| [Swevo.AutoImage](https://www.nuget.org/packages/Swevo.AutoImage) | [![Downloads](https://img.shields.io/nuget/dt/Swevo.AutoImage.svg)](https://www.nuget.org/packages/Swevo.AutoImage) | A free, MIT-licensed fluent image processing wrapper around SkiaSharp for  |
| [Swevo.AutoFeatureFlag](https://www.nuget.org/packages/Swevo.AutoFeatureFlag) | [![Downloads](https://img.shields.io/nuget/dt/Swevo.AutoFeatureFlag.svg)](https://www.nuget.org/packages/Swevo.AutoFeatureFlag) | Compile-time feature flag stubs for  |
| [Swevo.AutoTestData](https://www.nuget.org/packages/Swevo.AutoTestData) | [![Downloads](https://img.shields.io/nuget/dt/Swevo.AutoTestData.svg)](https://www.nuget.org/packages/Swevo.AutoTestData) | Compile-time test data builders for  |

---

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
