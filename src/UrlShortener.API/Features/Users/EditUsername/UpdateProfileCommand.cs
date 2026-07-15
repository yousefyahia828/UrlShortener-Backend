using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.EditUsername;

public sealed record EditUsernameCommand(
    Guid UserId,
    string FirstName,
    string LastName) : ICommand;