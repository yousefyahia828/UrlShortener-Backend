using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.Identity.Password.RequestPasswordReset;

public sealed record RequestPasswordResetCommand(string Email) : ICommand;
