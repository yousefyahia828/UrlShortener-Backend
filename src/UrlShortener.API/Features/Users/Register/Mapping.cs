namespace UrlShortener.API.Features.Users.Register;

public static class Mapping
{
    public static RegisterUserCommand MapToCommand(
        this Endpoint.RegisterUserRequest request, Guid idempotencyKey)
    => new(idempotencyKey,
           request.FirstName,
           request.LastName,
           request.Email,
           request.Password);
}
