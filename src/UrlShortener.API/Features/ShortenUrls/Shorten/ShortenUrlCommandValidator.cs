namespace UrlShortener.API.Features.ShortenUrls.Shorten;

public class ShortenUrlCommandValidator :
    AbstractValidator<ShortenUrlCommand>
{
    public ShortenUrlCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.LongUrl)
            .NotEmpty().WithMessage("URL is required")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Invalid URL")
            .DependentRules(() =>
            {
                RuleFor(x => x.Description)
                    .MaximumLength(100)
                    .WithMessage("Description can not exceed 50 characters long");
            });
    }
}
