# Distribution checklist run — 2026-10-01 (`Swevo.AutoAssert.Analyzers` `1.0.4`)

Based on `docs/DISTRIBUTION.md`.

## Release assets

- [x] Published `Swevo.AutoAssert.Analyzers` `1.0.4` to NuGet.
- [x] Added `AUTOA002` code fix to insert `await` and make containing scope async when needed.
- [x] Added code-fix tests (method, local function, lambda edge case).
- [x] Updated `CHANGELOG.md` and analyzer README guidance.

## Promotion copy

> `Swevo.AutoAssert.Analyzers` 1.0.4 is live on NuGet.  
> `AUTOA002` now has a one-click fix: adds missing `await` for async assertions and upgrades enclosing method/local-function/lambda to async when required.  
> Package: https://www.nuget.org/packages/Swevo.AutoAssert.Analyzers

## Follow-up

- [ ] Record day-7/day-30 download deltas in `docs/METRICS.md`.
