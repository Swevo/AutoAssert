# Distribution checklist run — 2026-10-01

Based on `docs/DISTRIBUTION.md`.

## 1) Release assets

- [x] Published package updates to NuGet:
  - `Swevo.AutoAssert` `2.4.1`
  - `Swevo.AutoAssert.Analyzers` `1.0.1`
  - `Swevo.AutoAssert.Generator` `1.0.1`
  - `Swevo.AutoAssert.AspNetCore` `1.0.1`
  - `Swevo.AutoAssert.Json` `1.0.1`
- [x] Updated `CHANGELOG.md` with release entry.
- [x] Updated package release notes (`Swevo.AutoAssert`).
- [x] README links to quickstart and recipes.

## 2) Community distribution (prepared copy)

- [x] Drafted short release blurb for Reddit/Discord/LinkedIn/X:

> AutoAssert 2.4.1 and companion 1.0.1 packages are live on NuGet.  
> This release improves onboarding and discoverability: rewritten quickstart README, new recipes/comparison/compatibility docs, and cleaner package metadata.  
> Also includes SourceLink dependency update to remove NU1902 build warnings.  
> Start here: https://www.nuget.org/packages/Swevo.AutoAssert

## 3) Developer trust

- [x] Build is green locally (`dotnet build AutoAssert.slnx -c Release`).
- [x] Changelog is current.
- [x] Compatibility matrix is present in `docs/COMPATIBILITY.md`.

## 4) Conversion support

- [x] 5-minute quickstart in root `README.md`.
- [x] Recipe library in `docs/RECIPES.md`.
- [x] Migration comparison in `docs/COMPARISON.md`.

## 5) Follow-up loop

- [x] Tag pushed: `v2.4.1`.
- [ ] Record day-1/day-7/day-30 download deltas in `docs/METRICS.md` scorecard.
