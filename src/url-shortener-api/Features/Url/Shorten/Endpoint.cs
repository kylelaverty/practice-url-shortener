using Url.Shortener.Api.Data;
using Url.Shortener.Api.Data.Entities;
using Url.Shortener.Api.Util;

namespace Url.Shortener.Api.Features.Url.Shorten;

public record Request(string? OriginalUrl);

public record Response(string ShortenedUrl);

public class Endpoint(UrlShortenerDbContext dbContext) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/url/shorten");
        AllowAnonymous();
        Throttle(
            hitLimit: 10,
            durationSeconds: 30
        );
    }

    /// <inheritdoc/>
    public override async Task HandleAsync(Request req, CancellationToken ct = default)
    {
        Logger?.LogInformation("Shortening URL: {Url}", req.OriginalUrl);

        var newUrl = new ShortenedUrl()
        {
            OriginalUrl = req.OriginalUrl!,
            GeneratedCode = SecureAlphanumericGenerator.GenerateSecureRandomAlphanumeric(8),
            CreatedDate = DateTime.UtcNow,
        };
        
        // Update the DB.
        _ = dbContext.Urls.Add(newUrl);
        _ = await dbContext.SaveChangesAsync(ct);
        
        var shortenedUrl = $"https://smu.com/r/{newUrl.GeneratedCode}";
        await Send.OkAsync(new Response(shortenedUrl), cancellation: ct);
    }
}