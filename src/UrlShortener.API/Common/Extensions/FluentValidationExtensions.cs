using FluentValidation;

namespace UrlShortener.API.Common.Extensions;

internal static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, string> StrictEmailAddress<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
             .NotEmpty().WithMessage("Email is required")
             .Matches("^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,}$")
             .WithMessage("Email format is invalid")
             .MaximumLength(254).WithMessage("Email is too long");
    }

    public static IRuleBuilderOptions<T, string> FirstName<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
           .NotEmpty().WithMessage("First name is required")
           .MinimumLength(2).WithMessage("First name is too short")
           .MaximumLength(20).WithMessage("First name is too long");
    }

    public static IRuleBuilderOptions<T, string> LastName<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Last name is required")
            .MinimumLength(2).WithMessage("Last name is too short")
            .MaximumLength(20).WithMessage("Last name is too long");
    }

    public static IRuleBuilderOptions<T, string> Password<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Password is required")
            .Must(x => x.Any(char.IsLower))
            .WithMessage("Password must contain at least one lowercase letter")
            .Must(x => x.Any(char.IsUpper))
            .WithMessage("Password must contain at least one uppercase letter")
            .Must(x => x.Any(char.IsDigit))
            .WithMessage("Password must contain at least one digit")
            .Must(x => x.Any(IsSpecialCharacter))
            .WithMessage("Password must contain at least one special character")
            .MinimumLength(8)
            .WithMessage("Password must contain at least 8 characters long")
            .MaximumLength(50)
            .WithMessage("Password can not exceed 50 characters");
    }

    private static bool IsSpecialCharacter(char c)
        => !char.IsLower(c) && !char.IsUpper(c) && !char.IsDigit(c);
}
