# Distribution checklist run — 2026-10-01 (`Swevo.AutoAssert.Analyzers` `1.0.3`)

Based on `docs/DISTRIBUTION.md`.

## Release assets

- [x] Published `Swevo.AutoAssert.Analyzers` `1.0.3` to NuGet.
- [x] Added `AUTOA001` code fixes:
  - `.NotBeNull()`
  - `.NotBeNullOrEmpty()` for strings
  - `.Be(expected)` scaffold
- [x] Updated `CHANGELOG.md` and analyzer README.

## Promotion copy

> `Swevo.AutoAssert.Analyzers` 1.0.3 is live on NuGet.  
> `AUTOA001` now includes one-click code fixes for bare `Should()` calls: `.NotBeNull()`, `.NotBeNullOrEmpty()` for strings, and `.Be(expected)` scaffolding.  
> Package: https://www.nuget.org/packages/Swevo.AutoAssert.Analyzers

## Follow-up

- [ ] Record day-7/day-30 download deltas in `docs/METRICS.md`.
