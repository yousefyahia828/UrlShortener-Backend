using FluentValidation;
using UrlShortener.API.Common.Extensions;

namespace UrlShortener.API.Features.Users.EditUsername;

internal sealed class UpdateProfileCommandValidator
    : AbstractValidator<EditUsernameCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.FirstName).FirstName();
        RuleFor(x => x.LastName).LastName();
    }
}
