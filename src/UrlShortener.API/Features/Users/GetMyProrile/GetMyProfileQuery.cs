using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.GetMyProrile;

public sealed record GetMyProfileQuery(Guid UserId) : IQuery<UserProfileResponse>;
