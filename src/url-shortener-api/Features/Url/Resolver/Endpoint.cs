using Microsoft.EntityFrameworkCore;
using Url.Shortener.Api.Data;

namespace Url.Shortener.Api.Features.Url.Resolver;

public record Request(string? GeneratedCode);

public record Response(string OriginalUrl);

public class Endpoint(UrlShortenerDbContext dbContext) : Endpoint<Request, Response>
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
            .SingleAsync(u => u.GeneratedCode == req.GeneratedCode, cancellationToken: ct);
        
        await Send.OkAsync(new Response(record.OriginalUrl), cancellation: ct);
    }
}