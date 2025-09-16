using Microsoft.EntityFrameworkCore;
using Url.Shortener.Api.Data.Entities;

namespace Url.Shortener.Api.Data;

public class UrlShortenerDbContext : DbContext
{
    public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options)
        : base(options)
    { }

    public DbSet<ShortenedUrl> Urls => Set<ShortenedUrl>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortenedUrl>(builder =>
        {
            builder
                .HasIndex(shortenedUrl => shortenedUrl.GeneratedCode)
                .IsUnique();
        });
    }
}