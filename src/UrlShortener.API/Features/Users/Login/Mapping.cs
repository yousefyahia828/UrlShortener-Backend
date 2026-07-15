namespace UrlShortener.API.Features.Users.Login;

public static class Mapping
{
    public static LoginUserCommand MapToCommand(
        this Endpoint.LoginUserRequest request)
    => new(request.Email, request.Password);
}
