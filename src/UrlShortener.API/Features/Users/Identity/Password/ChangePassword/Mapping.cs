namespace UrlShortener.API.Features.Users.Identity.Password.ChangePassword;

public static class Mapping
{
    public static ChangePasswordCommand MapToCommand(
        this Endpoint.ChangePasswordRequest request,
        Guid userId,
        string? refreshToken)
    => new(
        userId,
        request.CurrentPassword,
        request.NewPassword,
        request.ConfirmNewPassword,
        refreshToken,
        request.RevokeAllSessions);
}
