using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace AutoAssert.AspNetCore.Tests;

public class HttpResponseMessageAssertionsTests
{
    private static HttpResponseMessage CreateResponse(HttpStatusCode status, string? content = null, string? contentType = "application/json")
    {
        var response = new HttpResponseMessage(status);
        if (content is not null)
        {
            response.Content = new StringContent(content, Encoding.UTF8);
            if (contentType is not null)
            {
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            }
        }

        response.Headers.Add("X-Trace-Id", "abc123");
        return response;
    }

    [Fact]
    public void HaveStatusCode_passes_for_matching_status()
    {
        using var response = CreateResponse(HttpStatusCode.OK);
        response.Should().HaveStatusCode(HttpStatusCode.OK);
    }

    [Fact]
    public void HaveStatusCode_fails_for_mismatched_status()
    {
        using var response = CreateResponse(HttpStatusCode.BadRequest);
        var ex = Assert.Throws<AssertionFailedException>(() => response.Should().HaveStatusCode(HttpStatusCode.OK));
        Assert.Contains("200", ex.Message);
        Assert.Contains("400", ex.Message);
    }

    [Theory]
    [InlineData(HttpStatusCode.OK, true)]
    [InlineData(HttpStatusCode.Created, true)]
    [InlineData(HttpStatusCode.BadRequest, false)]
    public void BeSuccessful_reflects_2xx_status(HttpStatusCode status, bool expectSuccess)
    {
        using var response = CreateResponse(status);
        if (expectSuccess)
        {
            response.Should().BeSuccessful();
        }
        else
        {
            Assert.Throws<AssertionFailedException>(() => response.Should().BeSuccessful());
        }
    }

    [Fact]
    public void BeClientError_and_BeServerError_distinguish_4xx_from_5xx()
    {
        using var clientError = CreateResponse(HttpStatusCode.NotFound);
        clientError.Should().BeClientError();
        Assert.Throws<AssertionFailedException>(() => clientError.Should().BeServerError());

        using var serverError = CreateResponse(HttpStatusCode.InternalServerError);
        serverError.Should().BeServerError();
        Assert.Throws<AssertionFailedException>(() => serverError.Should().BeClientError());
    }

    [Fact]
    public void HaveHeader_checks_presence_and_optionally_value()
    {
        using var response = CreateResponse(HttpStatusCode.OK);
        response.Should().HaveHeader("X-Trace-Id").And.HaveHeader("X-Trace-Id", "abc123");

        Assert.Throws<AssertionFailedException>(() => response.Should().HaveHeader("X-Trace-Id", "wrong"));
        Assert.Throws<AssertionFailedException>(() => response.Should().HaveHeader("Missing-Header"));
    }

    [Fact]
    public void HaveContentType_checks_media_type()
    {
        using var response = CreateResponse(HttpStatusCode.OK, "{}", "application/json");
        response.Should().HaveContentType("application/json");
        Assert.Throws<AssertionFailedException>(() => response.Should().HaveContentType("text/plain"));
    }

    [Fact]
    public async Task HaveContentAsync_compares_raw_body()
    {
        using var response = CreateResponse(HttpStatusCode.OK, "hello world", "text/plain");
        await response.Should().HaveContentAsync("hello world");
        await Assert.ThrowsAsync<AssertionFailedException>(() => response.Should().HaveContentAsync("goodbye"));
    }

    private record Person(string Name, int Age);

    [Fact]
    public async Task HaveJsonContentEquivalentTo_deserializes_and_compares_via_BeEquivalentTo()
    {
        using var response = CreateResponse(HttpStatusCode.OK, """{"Name":"Ada","Age":30}""");
        await response.Should().HaveJsonContentEquivalentTo(new Person("Ada", 30));

        await Assert.ThrowsAsync<AssertionFailedException>(
            () => response.Should().HaveJsonContentEquivalentTo(new Person("Ada", 31)));
    }

    [Fact]
    public void Null_subject_fails_with_a_clear_message()
    {
        HttpResponseMessage? response = null;
        var ex = Assert.Throws<AssertionFailedException>(() => response.Should().HaveStatusCode(HttpStatusCode.OK));
        Assert.Contains("null", ex.Message);
    }
}
