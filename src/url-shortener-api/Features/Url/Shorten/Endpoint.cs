using Microsoft.Extensions.Options;
using Url.Shortener.Api.Data;
using Url.Shortener.Api.Data.Entities;
using Url.Shortener.Api.Util;

namespace Url.Shortener.Api.Features.Url.Shorten;

public record Request(string? OriginalUrl);

public class Endpoint(UrlShortenerDbContext dbContext, IOptionsSnapshot<ShortenerSettings> options) : Endpoint<Request>
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

    public override async Task HandleAsync(Request req, CancellationToken ct = default)
    {
        Logger?.LogInformation("Shortening URL: {Url}", req.OriginalUrl);

        var newUrl = new ShortenedUrl()
        {
            OriginalUrl = req.OriginalUrl!,
            GeneratedCode = SecureAlphanumericGenerator.GenerateSecureRandomAlphanumeric(options.Value.CodeGenLength),
            CreatedDate = DateTime.UtcNow,
        };
        
        // Update the DB.
        _ = dbContext.Urls.Add(newUrl);
        _ = await dbContext.SaveChangesAsync(ct);

        await Send.CreatedAtAsync<Resolver.Endpoint>(
            new { GeneratedCode = newUrl.GeneratedCode }
            , cancellation: ct);
    }
}