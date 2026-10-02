# Distribution checklist run — 2026-10-02 (`Swevo.AutoAssert.Analyzers` `1.0.6`)

Based on `docs/DISTRIBUTION.md`.

## Release assets

- [x] Published `Swevo.AutoAssert.Analyzers` `1.0.6` to NuGet.
- [x] Added `AUTOA004` analyzer for `BeEquivalentTo` migration suggestions.
- [x] Added non-destructive scaffold code fix for `AUTOA004`.
- [x] Added analyzer/code-fix tests and updated README/changelog.

## Promotion copy

> `Swevo.AutoAssert.Analyzers` 1.0.6 is live on NuGet.  
> New `AUTOA004` detects brittle repeated property asserts and adds a safe scaffold fix for `actual.Should().BeEquivalentTo(new { ... })` while keeping existing assertions.  
> Package: https://www.nuget.org/packages/Swevo.AutoAssert.Analyzers

## Follow-up

- [ ] Record day-7/day-30 download deltas in `docs/METRICS.md`.
