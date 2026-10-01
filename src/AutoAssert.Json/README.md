# Swevo.AutoAssert.Json

Fluent assertions for raw JSON strings built on top of [Swevo.AutoAssert](https://www.nuget.org/packages/Swevo.AutoAssert/).

## Install

```bash
dotnet add package Swevo.AutoAssert.Json
```

## Quickstart

```csharp
json.Should().BeValidJson();
json.Should().HaveJsonProperty("customer.name");
json.Should().HaveJsonPropertyEqualTo("customer.status", "active");
json.Should().BeEquivalentToJson("""{ "a": 1, "b": 2 }""");
```

## API

| Method | Description |
|---|---|
| `BeValidJson()` | Valid JSON parsing assertion |
| `HaveJsonProperty(path)` | Dotted/indexed path presence assertion |
| `HaveJsonPropertyEqualTo(path, value)` | Path value assertion |
| `BeEquivalentToJson(expectedJson)` | Structural JSON comparison with clear mismatches |

All failures flow through AutoAssert's core failure pipeline, so these assertions participate in `AssertionScope`.

## License

MIT © Justin Bannister
