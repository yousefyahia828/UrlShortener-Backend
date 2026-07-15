namespace UrlShortener.API.Features.Users.Identity.Email.RequestEmailChange;

public static class Mapping
{
    public static RequestEmailChangeCommand MapToCommand(
        this Endpoint.ChangeEmailRequest request, Guid userId)
    => new(userId, request.NewEmail);
}
