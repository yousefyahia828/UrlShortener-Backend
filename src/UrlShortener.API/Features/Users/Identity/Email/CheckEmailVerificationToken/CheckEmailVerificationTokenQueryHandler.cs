using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Persistence;

namespace UrlShortener.API.Features.Users.Identity.Email.CheckEmailVerificationToken;

internal sealed class CheckEmailVerificationTokenQueryHandler(
    IApplicationDbContext dbContext)
    : IQueryHandler<CheckEmailVerificationTokenQuery, bool>
{
    public async Task<Result<bool>> Handle(
        CheckEmailVerificationTokenQuery query,
        CancellationToken cancellationToken)
    {
        return await dbContext.EmailVerificationTokens
            .AsNoTracking()
            .AnyAsync(t => t.UserId == query.UserId &&
                           t.Id == query.TokenId &&
                           t.ExpiresOnUtc > DateTime.UtcNow,
                      cancellationToken);
    }
}