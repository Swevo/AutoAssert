using System.Net;
using System.Net.Http;

namespace AutoAssert;

/// <summary>Entry point for fluent assertions on <see cref="HttpResponseMessage"/>.</summary>
public static class HttpResponseMessageAssertionExtensions
{
    public static HttpResponseMessageAssertions Should(this HttpResponseMessage? response) => new(response);
}
