using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace AutoAssert;

/// <summary>
/// Fluent assertions for <see cref="HttpResponseMessage"/> — status code, headers, content type,
/// and string/JSON body. Designed for ASP.NET Core <c>WebApplicationFactory</c>/<c>TestServer</c>
/// integration tests and general <see cref="HttpClient"/>-based API tests.
/// </summary>
/// <remarks>
/// Failures always throw immediately as an <see cref="AssertionFailedException"/>; unlike the
/// core AutoAssert assertion types, these do not currently participate in an ambient
/// <see cref="AssertionScope"/> (that integration point is internal to the core package).
/// </remarks>
public readonly struct HttpResponseMessageAssertions
{
    private readonly HttpResponseMessage? _subject;

    internal HttpResponseMessageAssertions(HttpResponseMessage? subject) => _subject = subject;

    /// <summary>The underlying response being asserted against.</summary>
    public HttpResponseMessage? Subject => _subject;

    public AndConstraint<HttpResponseMessageAssertions> HaveStatusCode(HttpStatusCode expected, string because = "", params object[] becauseArgs)
    {
        EnsureSubject();

        if (_subject!.StatusCode != expected)
        {
            Fail($"Expected response to have status code {(int)expected} ({expected}), but found {(int)_subject.StatusCode} ({_subject.StatusCode}).", because, becauseArgs);
        }

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    public AndConstraint<HttpResponseMessageAssertions> BeSuccessful(string because = "", params object[] becauseArgs)
    {
        EnsureSubject();

        if (!_subject!.IsSuccessStatusCode)
        {
            Fail($"Expected response to have a successful (2xx) status code, but found {(int)_subject.StatusCode} ({_subject.StatusCode}).", because, becauseArgs);
        }

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    public AndConstraint<HttpResponseMessageAssertions> BeClientError(string because = "", params object[] becauseArgs)
    {
        EnsureSubject();

        var code = (int)_subject!.StatusCode;
        if (code is < 400 or >= 500)
        {
            Fail($"Expected response to have a client error (4xx) status code, but found {code} ({_subject.StatusCode}).", because, becauseArgs);
        }

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    public AndConstraint<HttpResponseMessageAssertions> BeServerError(string because = "", params object[] becauseArgs)
    {
        EnsureSubject();

        var code = (int)_subject!.StatusCode;
        if (code < 500)
        {
            Fail($"Expected response to have a server error (5xx) status code, but found {code} ({_subject.StatusCode}).", because, becauseArgs);
        }

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    public AndConstraint<HttpResponseMessageAssertions> HaveHeader(string name, string? expectedValue = null, string because = "", params object[] becauseArgs)
    {
        EnsureSubject();

        var hasHeader = _subject!.Headers.TryGetValues(name, out var values)
            || _subject.Content.Headers.TryGetValues(name, out values);

        if (!hasHeader)
        {
            Fail($"Expected response to have header '{name}', but it was not present.", because, becauseArgs);
            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }

        if (expectedValue is not null && !values!.Contains(expectedValue))
        {
            Fail($"Expected header '{name}' to have value \"{expectedValue}\", but found \"{string.Join(", ", values!)}\".", because, becauseArgs);
        }

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    public AndConstraint<HttpResponseMessageAssertions> HaveContentType(string expectedMediaType, string because = "", params object[] becauseArgs)
    {
        EnsureSubject();

        var actualMediaType = _subject!.Content.Headers.ContentType?.MediaType;
        if (!string.Equals(actualMediaType, expectedMediaType, StringComparison.OrdinalIgnoreCase))
        {
            Fail($"Expected response content type to be \"{expectedMediaType}\", but found \"{actualMediaType ?? "<none>"}\".", because, becauseArgs);
        }

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    public async Task<AndConstraint<HttpResponseMessageAssertions>> HaveContentAsync(string expected, string because = "", params object[] becauseArgs)
    {
        EnsureSubject();

        var actual = await _subject!.Content.ReadAsStringAsync();
        if (actual != expected)
        {
            Fail($"Expected response content to be \"{expected}\", but found \"{actual}\".", because, becauseArgs);
        }

        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    /// <summary>
    /// Deserializes the response body as JSON into <typeparamref name="T"/> and asserts it is
    /// equivalent to <paramref name="expected"/> via AutoAssert's <c>BeEquivalentTo</c>.
    /// </summary>
    public async Task<AndConstraint<HttpResponseMessageAssertions>> HaveJsonContentEquivalentTo<T>(T expected, string because = "", params object[] becauseArgs)
    {
        EnsureSubject();

        var stream = await _subject!.Content.ReadAsStreamAsync();
        T? actual;
        try
        {
            actual = await JsonSerializer.DeserializeAsync<T>(stream);
        }
        catch (JsonException ex)
        {
            Fail($"Expected response content to deserialize as {typeof(T).Name}, but it did not: {ex.Message}", because, becauseArgs);
            return new AndConstraint<HttpResponseMessageAssertions>(this);
        }

        actual.Should().BeEquivalentTo(expected, because, becauseArgs);
        return new AndConstraint<HttpResponseMessageAssertions>(this);
    }

    private void EnsureSubject()
    {
        if (_subject is null)
        {
            Fail("Expected an HttpResponseMessage, but found <null>.", string.Empty, []);
        }
    }

    private static void Fail(string message, string because, object[] becauseArgs)
    {
        var reason = BuildReason(because, becauseArgs);
        throw new AssertionFailedException(message + reason);
    }

    private static string BuildReason(string because, object[] becauseArgs)
    {
        if (string.IsNullOrWhiteSpace(because))
        {
            return string.Empty;
        }

        var reason = becauseArgs is { Length: > 0 } ? string.Format(because, becauseArgs) : because;
        if (!reason.StartsWith("because", StringComparison.OrdinalIgnoreCase))
        {
            reason = "because " + reason;
        }

        return " " + reason;
    }
}
