# AutoAssert.Analyzers

Roslyn analyzers for [AutoAssert](https://www.nuget.org/packages/Swevo.AutoAssert/) that catch
common fluent-assertion footguns at compile time.

## Diagnostics

| ID | Description |
|---|---|
| `AUTOA001` | `subject.Should();` used as a bare statement — asserts nothing since no assertion method follows `Should()`. |
| `AUTOA002` | An AutoAssert async assertion (`ThrowAsync`, `NotThrowAsync`, `CompleteWithinAsync`) is called without `await`, so the assertion may not run before the test completes. |

## Install

```bash
dotnet add package Swevo.AutoAssert.Analyzers
```

This is a development-only dependency (`PrivateAssets="all"` in the generated reference) — it
never ships in your build output.

## License

MIT © Justin Bannister
