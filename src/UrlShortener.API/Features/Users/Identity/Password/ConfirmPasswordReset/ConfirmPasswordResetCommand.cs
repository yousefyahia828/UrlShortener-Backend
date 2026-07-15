using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.Identity.Password.ConfirmPasswordReset;

public sealed record ConfirmPasswordResetCommand(
    string Token,
    string NewPassword,
    string ConfirmNewPassword) : ICommand;