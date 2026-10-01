# Metrics model for download growth

## Primary KPIs

| KPI | Why it matters | Suggested cadence |
|---|---|---|
| NuGet total downloads | Overall adoption trend | Weekly |
| NuGet version downloads | Release-level performance | Weekly |
| Dependent repositories/projects | Ecosystem penetration | Monthly |
| GitHub stars/watchers/issues | Trust and interest | Monthly |

## Leading indicators

| Indicator | Signal |
|---|---|
| README views / clone count | Discovery quality |
| Time-to-first-success feedback | Onboarding friction |
| Analyzer + companion package installs | Depth of adoption |

## Simple release scorecard

### 2026-10-01 baseline (release: `v2.4.1`)

| Package | Released version | Baseline capture date | Total downloads (all versions) | Version downloads | Day 7 target date | Day 30 target date |
|---|---:|---|---:|---:|---|---|
| Swevo.AutoAssert | 2.4.1 | 2026-10-01 | 508 | 0 | 2026-10-08 | 2026-10-31 |
| Swevo.AutoAssert.Analyzers | 1.0.1 | 2026-10-01 | 0 | 0 | 2026-10-08 | 2026-10-31 |
| Swevo.AutoAssert.Generator | 1.0.1 | 2026-10-01 | 0 | 0 | 2026-10-08 | 2026-10-31 |
| Swevo.AutoAssert.AspNetCore | 1.0.1 | 2026-10-01 | 0 | 0 | 2026-10-08 | 2026-10-31 |
| Swevo.AutoAssert.Json | 1.0.1 | 2026-10-01 | 0 | 0 | 2026-10-08 | 2026-10-31 |

### Day 7 / Day 30 update template

| Package | Day 7 total | Day 7 version | Day 30 total | Day 30 version | Best channel | Message angle |
|---|---:|---:|---:|---:|---|---|
| Swevo.AutoAssert |  |  |  |  |  |  |
| Swevo.AutoAssert.Analyzers |  |  |  |  |  |  |
| Swevo.AutoAssert.Generator |  |  |  |  |  |  |
| Swevo.AutoAssert.AspNetCore |  |  |  |  |  |  |
| Swevo.AutoAssert.Json |  |  |  |  |  |  |

Data source used for baseline: NuGet Search API (`https://azuresearch-usnc.nuget.org/query`).

## Target-setting example

- +15% day-30 downloads over prior comparable release
- +10% companion package attach rate (`Analyzers`, `Generator`, `Json`, `AspNetCore`)
