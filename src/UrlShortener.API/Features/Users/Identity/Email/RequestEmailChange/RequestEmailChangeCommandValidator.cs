using FluentValidation;
using UrlShortener.API.Common.Extensions;

namespace UrlShortener.API.Features.Users.Identity.Email.RequestEmailChange;

internal sealed class RequestEmailChangeCommandValidator :
    AbstractValidator<RequestEmailChangeCommand>
{
    public RequestEmailChangeCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        RuleFor(x => x.NewEmail).StrictEmailAddress();
    }
}
