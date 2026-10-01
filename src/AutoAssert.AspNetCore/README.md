# Swevo.AutoAssert.AspNetCore

Fluent `HttpResponseMessage` assertions for ASP.NET Core integration tests.

## Install

```bash
dotnet add package Swevo.AutoAssert.AspNetCore
```

## Quickstart

```csharp
var response = await client.GetAsync("/api/orders/1");

response.Should()
    .BeSuccessful()
    .And.HaveContentType("application/json")
    .And.HaveHeader("X-Trace-Id");

await response.Should().HaveJsonContentEquivalentTo(new OrderDto { Id = 1, Total = 42.5m });
```

## API overview

| Method | Purpose |
|---|---|
| `HaveStatusCode(...)` | Exact status assertion |
| `BeSuccessful()` / `BeClientError()` / `BeServerError()` | Status class assertions |
| `HaveHeader(name, value?)` | Header presence/value assertion |
| `HaveContentType(mediaType)` | Content type assertion |
| `HaveContentAsync(expected)` | Exact body string assertion |
| `HaveJsonContentEquivalentTo<T>(expected)` | JSON body deserialization + AutoAssert equivalency |

## License

MIT © Justin Bannister
