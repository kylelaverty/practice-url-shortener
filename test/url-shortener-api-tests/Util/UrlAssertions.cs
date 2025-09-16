namespace Url.Shortener.Api.Tests.Util;

public static class UrlAssertions
{
    public static void ShouldBeAValidUrl(this string actual, params string[] allowedSchemes)
    {
        if (!Uri.TryCreate(actual, UriKind.Absolute, out var uri))
        {
            throw new ShouldAssertException($"Expected '{actual}' to be a valid URL.");
        }

        if (allowedSchemes?.Length > 0 && !Array.Exists(allowedSchemes, s => s.Equals(uri.Scheme, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ShouldAssertException($"Expected URL scheme to be one of [{string.Join(", ", allowedSchemes)}], but was '{uri.Scheme}'.");
        }
    }
}