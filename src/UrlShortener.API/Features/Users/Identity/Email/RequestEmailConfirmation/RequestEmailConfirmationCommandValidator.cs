using FluentValidation;
using UrlShortener.API.Common.Extensions;

namespace UrlShortener.API.Features.Users.Identity.Email.RequestEmailConfirmation;

internal sealed class RequestEmailConfirmationCommandValidator
    : AbstractValidator<RequestEmailConfirmationCommand>
{
    public RequestEmailConfirmationCommandValidator()
    {
        RuleFor(x => x.Email).StrictEmailAddress();
    }
}
