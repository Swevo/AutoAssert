# AutoAssert recipes

## 1) Assert a collection shape and ordering

```csharp
orders.Should().HaveCount(3);
orders.Select(x => x.Id).Should().ContainInOrder(101, 102, 103);
orders.Should().AllSatisfy(x => x.Total.Should().BePositive());
```

## 2) Assert complex API DTOs without brittle per-field checks

```csharp
actual.Should().BeEquivalentTo(expected, options => options
    .Excluding(x => x.Timestamp)
    .WithStrictOrdering());
```

## 3) Assert async exception behavior safely

```csharp
await (() => service.RunAsync(input))
    .Should().ThrowAsync<ValidationException>()
    .WithMessageContaining("customerId");
```

## 4) Assert multiple failures at once

```csharp
using (new AssertionScope())
{
    result.Id.Should().NotBeEmpty();
    result.Email.Should().Contain("@");
    result.Items.Should().HaveCountGreaterThan(0);
}
```

## 5) Snapshot testing for large payloads

```csharp
result.Should().MatchSnapshot(
    "order-summary",
    scrub: SnapshotScrubbers.Combine(
        SnapshotScrubbers.Guids(),
        SnapshotScrubbers.IsoTimestamps()));
```

## 6) Assert JSON contracts directly

```csharp
payload.Should().BeValidJson();
payload.Should().HaveJsonProperty("data.customer.id");
payload.Should().HaveJsonPropertyEqualTo("data.status", "active");
payload.Should().BeEquivalentToJson(expectedJson);
```

## 7) Assert HTTP integration responses

```csharp
var response = await client.GetAsync("/api/orders/1");

response.Should()
    .BeSuccessful()
    .And.HaveContentType("application/json")
    .And.HaveHeader("X-Trace-Id");
```
