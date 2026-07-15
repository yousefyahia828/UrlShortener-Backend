using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.Identity.Email.ConfirmEmailChange;

public sealed record ConfirmEmailChangeCommand(
    Guid UserId,
    Guid TokenId) : ICommand;
