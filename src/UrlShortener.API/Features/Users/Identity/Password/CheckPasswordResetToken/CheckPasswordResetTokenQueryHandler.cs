using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Authentication;
using UrlShortener.Abstractions.Persistence;

namespace UrlShortener.API.Features.Users.Identity.Password.CheckPasswordResetToken;

internal sealed class CheckPasswordResetTokenQueryHandler(
    IApplicationDbContext dbContext,
    IPasswordResetService passwordResetService)
    : IQueryHandler<CheckPasswordResetTokenQuery, bool>
{
    public async Task<Result<bool>> Handle(
        CheckPasswordResetTokenQuery query,
        CancellationToken cancellationToken)
    {
        var token = await dbContext.PasswordResetTokens
            .AsNoTracking()
            .Where(t => t.TokenHash == passwordResetService.HashToken(query.Token))
            .FirstOrDefaultAsync(cancellationToken);

        return token is not null && !token.IsExpired && !token.IsUsed;
    }
}