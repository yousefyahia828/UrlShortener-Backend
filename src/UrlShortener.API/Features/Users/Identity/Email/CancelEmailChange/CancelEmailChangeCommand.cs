using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.Identity.Email.CancelEmailChange;

public sealed record CancelEmailChangeCommand(Guid UserId) : ICommand;
