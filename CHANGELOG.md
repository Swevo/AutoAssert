# Changelog

All notable changes to this project are documented here.

## 1.0.3 (Swevo.AutoAssert.Analyzers)

- Added `AUTOA001` code fixes for bare `Should()` calls:
  - chain `.NotBeNull()`
  - chain `.NotBeNullOrEmpty()` for string subjects
  - chain `.Be(expected)` scaffold
- Analyzer package now ships analyzer + code-fix support together.

## 1.0.2 (Swevo.AutoAssert.Analyzers)

- Added analyzer rule `AUTOA003` to warn on weak exception assertions using `Throw<Exception>()` or `ThrowAsync<Exception>()`.
- Updated analyzer package metadata/README to include the new diagnostic.

## 2.4.1 / 1.0.1 companion packages

- Refreshed NuGet conversion assets: rewritten README quickstart flow, recipes, compatibility matrix, and comparison guide.
- Added release/distribution/metrics documentation to support ongoing download growth.
- Improved package discovery metadata (descriptions/tags/icons) across all AutoAssert packages.
- Updated `Microsoft.SourceLink.GitHub` to `10.0.401` to remove `NU1902` vulnerability warning from builds.

## 2.4.0

- Added `ExceptionAssertions.WithMessageMatching` for wildcard exception message assertions.
- Added `AssertionConfig.UseColorizedOutput` for ANSI expected/actual coloring in common failure output paths.

## 2.3.0

- Added `SnapshotScrubbers` (`Guids`, `IsoTimestamps`, `Pattern`, `Combine`).
- Added `MatchSnapshot(scrub:)` to normalize non-deterministic values before comparison.
- Added `StringAssertions.Subject` public accessor.
- Added companion package: `Swevo.AutoAssert.Json`.

## 2.2.0

- Added companion package: `Swevo.AutoAssert.Generator`.
- Added companion package: `Swevo.AutoAssert.AspNetCore`.
- `BeEquivalentTo` now consults generated equivalency registry before reflection fallback.

## 2.1.0

- Added `AssertionConfig.ConfigureEquivalency` for process-wide `BeEquivalentTo` defaults.
- Added `MatchSnapshot()` JSON snapshot testing support.

## 2.0.0 (breaking)

- Assertion methods now return `AndConstraint<T>` for fluent chaining.
- Added `AssertionScope` for multi-failure collection.
- Added `BeEquivalentTo` options (`Excluding(...)`, `WithStrictOrdering()`).
- Added broader assertions for dates/times, GUIDs, nullable values, enums, dictionaries, and execution time.

## 1.1.0

- Added deep structural `BeEquivalentTo` with nested graph support and cycle protection.

## 1.0.1

- Added package icon.

## 1.0.0

- Initial release with object, string, numeric, boolean, collection, and exception assertions.
