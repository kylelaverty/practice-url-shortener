using System.Net;
using Url.Shortener.Api.Features.Url.Shorten;
using Url.Shortener.Api.Tests.Util;

namespace Url.Shortener.Api.Tests.Features.Url.Shorten;

public class Tests(App app) : TestBase<App>
{
    [Fact]
    public async Task SendUrl_Should_Return_Url()
    {
        // Arrange
        var testUrl = "https://test.com";
        var request = new Request(testUrl);

        // Act
        var (responseMessage, result) = await app.Client.POSTAsync<Endpoint, Request, Response>(request);

        // Assert
        responseMessage.StatusCode.ShouldBe(HttpStatusCode.OK);
        _ = result.ShouldNotBeNull();
        result.ShortenedUrl.ShouldNotBeNullOrWhiteSpace();
        result.ShortenedUrl.ShouldBeAValidUrl();
    }
}