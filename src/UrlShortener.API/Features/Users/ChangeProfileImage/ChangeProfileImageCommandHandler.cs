using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UrlShortener.Abstractions.Infrastructure;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.Users;
using UrlShortener.Infrastructure.Users;

namespace UrlShortener.API.Features.Users.ChangeProfileImage;

internal sealed class ChangeProfileImageCommandHandler(
    IApplicationDbContext context,
    IOptionsSnapshot<ProfileImageOptions> profileImageOptions,
    ILocalStorage localStorage,
    IWebHostEnvironment env)
    : ICommandHandler<ChangeProfileImageCommand, string>
{
    private readonly ProfileImageOptions _imageOptions = profileImageOptions.Value;
    private readonly string _rootPath = env.WebRootPath;

    public async Task<Result<string>> Handle(
        ChangeProfileImageCommand command,
        CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return UserErrors.NotFound;
        }

        return await Result
            .From(command.Image)
            .Ensure(
                (x => _imageOptions.AllowedContentTypes.Contains(x.ContentType, StringComparer.OrdinalIgnoreCase),
                    UserErrors.InvalidProfileImageContentType),
                (x => _imageOptions.AllowedExtensions.Contains(Path.GetExtension(x.FileName), StringComparer.OrdinalIgnoreCase),
                    UserErrors.InvalidProfileImageExtension),
                (x => x.Length > 0, UserErrors.EmptyProfileImage),
                (x => x.Length <= _imageOptions.MaximumSizeInMegaBytes * 1024L * 1024L,
                    UserErrors.LargeProfileImage(_imageOptions.MaximumSizeInMegaBytes)))
            .MapAsync(image =>
            {
                if (user.ProfileImageUrl != "avatars/avatar.jpg")
                    File.Delete(Path.Combine(_rootPath, user.ProfileImageUrl));

                return localStorage.SaveFileAsync(
                    "avatars",
                    image,
                    cancellationToken);
            })
            .TapAsync(user.UpdateProfileImage)
            .TapAsync(_ => context.SaveChangesAsync(cancellationToken));
    }
}