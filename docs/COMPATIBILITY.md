# Compatibility

## Package target frameworks

| Package | Target frameworks |
|---|---|
| `Swevo.AutoAssert` | `netstandard2.0`, `net8.0`, `net9.0`, `net10.0` |
| `Swevo.AutoAssert.AspNetCore` | `netstandard2.0`, `net8.0`, `net9.0`, `net10.0` |
| `Swevo.AutoAssert.Json` | `netstandard2.0`, `net8.0`, `net9.0`, `net10.0` |
| `Swevo.AutoAssert.Analyzers` | `netstandard2.0` (Roslyn analyzer package) |
| `Swevo.AutoAssert.Generator` | `netstandard2.0` (Roslyn source generator package) |

## Test framework interoperability

AutoAssert throws plain assertion exceptions that are recognized as failures by:

- xUnit
- NUnit
- MSTest

## Runtime notes

- Native AOT scenarios are supported by core assertions; the generator package can reduce reflection in `BeEquivalentTo` paths for annotated types.
- For best parity across teams, use the same package version across all test projects in a solution.
