# AutoAssert.AspNetCore

Fluent `HttpResponseMessage` assertions for [AutoAssert](https://www.nuget.org/packages/Swevo.AutoAssert/),
ideal for ASP.NET Core `WebApplicationFactory`/`TestServer` integration tests and general
`HttpClient`-based API tests.

## Usage

```bash
dotnet add package Swevo.AutoAssert.AspNetCore
```

```csharp
var response = await client.GetAsync("/api/orders/1");

response.Should()
    .BeSuccessful()
    .And.HaveContentType("application/json")
    .And.HaveHeader("X-Trace-Id");

await response.Should().HaveJsonContentEquivalentTo(new OrderDto { Id = 1, Total = 42.50m });
```

## API

| Method | Description |
|---|---|
| `HaveStatusCode(HttpStatusCode)` | Asserts an exact status code. |
| `BeSuccessful()` | Asserts a 2xx status code. |
| `BeClientError()` | Asserts a 4xx status code. |
| `BeServerError()` | Asserts a 5xx status code. |
| `HaveHeader(name, value?)` | Asserts a response (or content) header is present, optionally with a specific value. |
| `HaveContentType(mediaType)` | Asserts the response's `Content-Type` media type. |
| `HaveContentAsync(expected)` | Asserts the raw response body string. |
| `HaveJsonContentEquivalentTo<T>(expected)` | Deserializes the JSON body as `T` and asserts it via AutoAssert's `BeEquivalentTo`. |

All synchronous methods return `AndConstraint<HttpResponseMessageAssertions>` for fluent chaining
(`.And.Method()...`); the two body-reading methods are `async` since reading content requires I/O.

## Limitations

Failures always throw immediately as an `AssertionFailedException` — these assertions do not
currently participate in an ambient `AssertionScope` (multi-failure collection is an AutoAssert
core-package feature not yet exposed across this boundary).

## License

MIT © Justin Bannister
