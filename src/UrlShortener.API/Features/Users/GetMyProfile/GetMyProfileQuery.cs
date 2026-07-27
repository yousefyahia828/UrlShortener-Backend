namespace UrlShortener.API.Features.Users.GetMyProfile;

public sealed record GetMyProfileQuery(Guid UserId) : IQuery<UserProfileResponse>;
