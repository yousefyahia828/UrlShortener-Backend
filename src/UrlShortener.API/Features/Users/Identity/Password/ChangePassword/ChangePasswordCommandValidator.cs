using FluentValidation;
using UrlShortener.API.Common.Extensions;

namespace UrlShortener.API.Features.Users.Identity.Password.ChangePassword;

internal sealed class ChangePasswordCommandValidator
    : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required")
            .DependentRules(() =>
            {
                RuleFor(x => x.NewPassword)
                    .NotEqual(x => x.CurrentPassword)
                    .WithMessage("The new password must be different from the current password")
                    .Password();

                RuleFor(x => x.ConfirmNewPassword)
                     .NotEmpty().WithMessage("Confirm password is required")
                    .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
            });
    }
}
