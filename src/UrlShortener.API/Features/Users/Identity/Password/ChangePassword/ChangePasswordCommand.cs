using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.Identity.Password.ChangePassword;

public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword,
    string? RefreshToken = null,
    bool RevokeAllSessions = true) : ICommand;
