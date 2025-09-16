using Microsoft.EntityFrameworkCore;
using Url.Shortener.Api.Data;

namespace Url.Shortener.Api.Features.Url.Resolver;

public record Request(string? GeneratedCode);

public class Endpoint(UrlShortenerDbContext dbContext): Endpoint<Request>
{
    public override void Configure()
    {
        Get("/r/{GeneratedCode}");
        AllowAnonymous();
        Throttle(
            hitLimit: 10,
            durationSeconds: 30
        );
    }

    public override async Task HandleAsync(Request req, CancellationToken ct = default)
    {
        Logger?.LogInformation("Resolving Generated Code: {GeneratedCode}", req.GeneratedCode);

        var record = await dbContext.Urls
            .SingleOrDefaultAsync(u => u.GeneratedCode == req.GeneratedCode, cancellationToken: ct);

        if (record is null)
        {
            await Send.NotFoundAsync(ct);
        }

        await Send.RedirectAsync(record!.OriginalUrl, allowRemoteRedirects: true);
    }
}