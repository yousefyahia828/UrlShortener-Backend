using UrlShortener.API.Common.Extensions;

namespace UrlShortener.API.Features.Users.Register;

internal sealed class RegisterUserCommandValidator
    : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.FirstName).FirstName();

        RuleFor(x => x.LastName).LastName();

        RuleFor(x => x.Email).StrictEmailAddress();

        RuleFor(x => x.Password).Password();
    }
}
