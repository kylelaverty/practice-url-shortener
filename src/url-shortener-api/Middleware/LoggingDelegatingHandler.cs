namespace Url.Shortener.Api.Middleware;

/// <summary>
/// A delegating handler for logging HTTP requests and responses.
/// </summary>
/// <param name="logger">The logger to use for logging.</param>
public class LoggingDelegatingHandler(ILogger<LoggingDelegatingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            logger?.LogInformation("Before HTTP request");
            var result = await base.SendAsync(request, cancellationToken);
            _ = result.EnsureSuccessStatusCode();
            logger?.LogInformation("After HTTP request");
            return result;
        }
        catch (Exception e)
        {
            logger?.LogError(e, "HTTP request failed");
            throw;
        }
    }
}