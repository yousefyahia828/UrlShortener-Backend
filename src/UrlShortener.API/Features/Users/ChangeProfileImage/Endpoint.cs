using Josephan.CQRS;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.ChangeProfileImage;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/profile-image", async (
            IFormFile profileImage,
            ICurrentUser currentUser,
            ISender sender) =>
        {
            var command = new ChangeProfileImageCommand(
                currentUser.Id,
                profileImage);

            return await sender.Send(command)
                .MatchAsync(
                    value => Results.Ok(new { ProfileImageUrl = value }),
                    CustomResults.Problem);
        })
        .RequireAuthorization()
        .DisableAntiforgery()
        .Accepts<IFormFile>("multipart/form-data")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .WithTags(Tags.Users)
        .WithName("UpdateProfileImage")
        .WithSummary("Updates the current user's profile image.")
        .WithDescription(
            "Uploads a new profile image for the authenticated user, replacing any existing image. " +
            "Accepts multipart/form-data with a single image file. " +
            "The image is validated against the configured allowed content types, allowed extensions, " +
            "and maximum file size before being stored.")
        .WithTags("Users");
    }
}
