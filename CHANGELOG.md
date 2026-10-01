# Changelog

All notable changes to this project are documented here.

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
