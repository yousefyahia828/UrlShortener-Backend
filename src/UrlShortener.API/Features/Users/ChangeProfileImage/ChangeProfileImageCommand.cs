using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.ChangeProfileImage;

public sealed record ChangeProfileImageCommand(
    Guid UserId,
    IFormFile Image) : ICommand<string>;
