namespace UrlShortener.API.Features.Users.Identity.Password.ConfirmPasswordReset;

public static class Mapping
{
    public static ConfirmPasswordResetCommand MapToCommand(
        this Endpoint.ResetPasswordRequest request)
    => new(request.Token, request.NewPassword, request.ConfirmNewPassword);
}
