using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.Identity.Email.ConfirmEmail;

public sealed record ConfirmEmailCommand(Guid UserId, Guid TokenId) : ICommand;
