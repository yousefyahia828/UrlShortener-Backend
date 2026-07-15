using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.Identity.Email.RequestEmailChange;

public sealed record RequestEmailChangeCommand(
    Guid UserId,
    string NewEmail) : ICommand;