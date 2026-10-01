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

## Example

```csharp
// AUTOA001
value.Should();

// AUTOA002
action.Should().ThrowAsync<InvalidOperationException>();
```

## License

MIT © Justin Bannister
