namespace UrlShortener.API.Features.Users.Login;

internal sealed class LoginUserCommandValidator
    : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is requied");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required");
    }
}
