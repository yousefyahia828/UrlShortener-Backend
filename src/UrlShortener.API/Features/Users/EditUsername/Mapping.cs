
namespace UrlShortener.API.Features.Users.EditUsername;

public static class Mapping
{
    public static EditUsernameCommand MapToCommand(
        this Endpoint.EditUsernameRequest request, Guid userId)
    => new(userId, request.FirstName, request.LastName);
}
