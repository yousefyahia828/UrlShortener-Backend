using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.GetMyProrile;

internal sealed class GetMyProfileQueryHandler(
    IApplicationDbContext context)
    : IQueryHandler<GetMyProfileQuery, UserProfileResponse>
{
    public async Task<Result<UserProfileResponse>> Handle(
        GetMyProfileQuery query,
        CancellationToken cancellationToken)
    {
        return await Result
            .From(context.Users
                .AsNoTracking()
                .Where(u => u.Id == query.UserId)
                .Select(u => new UserProfileResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    PendingEmail = u.PendingEmail,
                    EmailVerified = u.EmailConfirmed,
                    ProfileImageUrl = u.ProfileImageUrl,
                    CreatedOnUtc = u.CreatedOnUtc,
                    TotalUrls = u.ShortenUrls.Count,
                    ActiveUrls = u.ShortenUrls.Count(url => url.Enabled),
                    DisabledUrls = u.ShortenUrls.Count(url => !url.Enabled)
                })
                .FirstOrDefaultAsync(cancellationToken))
            .EnsureNotNullAsync(UserErrors.NotFound);
    }
}