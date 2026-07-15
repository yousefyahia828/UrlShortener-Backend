namespace UrlShortener.API.Features.ShortenUrls.EditDescription;

internal sealed class EditUrlDescriptionCommandValidator
    : AbstractValidator<EditUrlDescriptionCommand>
{
    public EditUrlDescriptionCommandValidator()
    {
        RuleFor(x => x.Description)
            .MaximumLength(100)
            .WithMessage("Description can not exceed 50 characters long");
    }
}
