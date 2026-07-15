using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.Logout;

public sealed record LogoutUserCommand(string Token) : ICommand;
