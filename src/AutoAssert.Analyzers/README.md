# Swevo.AutoAssert.Analyzers

Compile-time diagnostics for [Swevo.AutoAssert](https://www.nuget.org/packages/Swevo.AutoAssert/) that prevent common fluent assertion mistakes.

## Install

```bash
dotnet add package Swevo.AutoAssert.Analyzers
```

This package is development-only and does not ship with application output.

## Diagnostics

| ID | What it catches |
|---|---|
| `AUTOA001` | Bare `subject.Should();` statement that performs no assertion. |
| `AUTOA002` | Unawaited async assertion (`ThrowAsync`, `NotThrowAsync`, `CompleteWithinAsync`). |
| `AUTOA003` | Weak exception assertions that use `Throw<Exception>()` or `ThrowAsync<Exception>()` instead of a specific exception type. |

`AUTOA001` includes quick fixes to chain a real assertion (`.NotBeNull()`, `.NotBeNullOrEmpty()` for strings, or `.Be(expected)` scaffold).
`AUTOA002` includes a quick fix that inserts `await` and upgrades the containing method/local-function/lambda to async when needed.

## Example

```csharp
// AUTOA001
value.Should();

// AUTOA002
action.Should().ThrowAsync<InvalidOperationException>();

// AUTOA003
action.Should().Throw<Exception>();
```

## License

MIT © Justin Bannister
