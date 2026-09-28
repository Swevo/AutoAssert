# AutoAssert.Json

Fluent JSON assertions for [AutoAssert](https://www.nuget.org/packages/Swevo.AutoAssert/) — assert
directly on raw JSON strings, with full structural diffs.

## Usage

```bash
dotnet add package Swevo.AutoAssert.Json
```

```csharp
json.Should().BeValidJson();

json.Should()
    .HaveJsonProperty("customer.name")
    .And.HaveJsonProperty("items[0].id");

json.Should().HaveJsonPropertyEqualTo("customer.name", "Ada");

json.Should().BeEquivalentToJson("""{ "a": 1, "b": 2 }""");
```

## API

| Method | Description |
|---|---|
| `BeValidJson()` | Asserts the subject parses as JSON. |
| `HaveJsonProperty(path)` | Asserts a property exists at a dotted/indexed path, e.g. `"customer.address.city"` or `"items[0].id"`. |
| `HaveJsonPropertyEqualTo(path, value)` | Asserts the property at `path` equals `value`. |
| `BeEquivalentToJson(expectedJson)` | Structural comparison: objects compared property-by-property regardless of key order, arrays compared element-by-element in order, primitives by value. Reports every mismatch found (missing/extra properties, differing values) rather than stopping at the first. |

## Why not just `HaveJsonProperty(path, value)`?

`Should().HaveJsonProperty("customer.name", "Ada")` would be ambiguous with the presence-only
overload's optional `because` parameter (`HaveJsonProperty(path, because: "...")`) — both accept a
plain `string` in the second position, and C# would silently prefer the exact-`string` overload,
turning your expected value into a reason clause instead of an assertion. `HaveJsonPropertyEqualTo`
avoids that footgun entirely.

## Limitations

Failures always throw immediately as an `AssertionFailedException` — these assertions do not
currently participate in an ambient `AssertionScope`.

## License

MIT © Justin Bannister
