namespace Url.Shortener.Api.Features.Url.Resolver;

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.GeneratedCode)
            .NotEmpty()
            .WithMessage("Shortened URL is required.");
    }
}