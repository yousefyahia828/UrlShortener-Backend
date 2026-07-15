using Josephan.CQRS;
using UrlShortener.Abstractions.Authentication;

namespace UrlShortener.API.Features.Users.Logout;

internal sealed class LogoutUserCommandHandler(
    IRefreshTokenProvider refreshTokenProvider)
    : ICommandHandler<LogoutUserCommand>
{
    public async Task<Result> Handle(LogoutUserCommand command, CancellationToken cancellationToken)
    {
        return await refreshTokenProvider.RevokeAsync(command.Token);
    }
}