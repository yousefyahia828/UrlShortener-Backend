using FluentValidation;
using UrlShortener.API.Common.Extensions;

namespace UrlShortener.API.Features.Users.Identity.Password.ConfirmPasswordReset;

internal sealed class ConfirmPasswordResetCommandValidator
    : AbstractValidator<ConfirmPasswordResetCommand>
{
    public ConfirmPasswordResetCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.NewPassword).Password();

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Confirm password is required")
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
    }
}
