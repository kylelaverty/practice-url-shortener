namespace Url.Shortener.Api.Features.Url.Shorten;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.OriginalUrl)
            .NotEmpty()
            .WithMessage("Original URL is required.");
        
        RuleFor(x => x.OriginalUrl)
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x.OriginalUrl))
            .WithMessage("Original URL must be a valid absolute URL.");
    }
}