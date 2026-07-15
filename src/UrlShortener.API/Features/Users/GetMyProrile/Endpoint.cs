using Josephan.CQRS;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.GetMyProrile;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users/me", async (ICurrentUser currentUser, ISender sender) =>
        {
            return await Result
                .From(new GetMyProfileQuery(currentUser.Id))
                .BindAsync(query => sender.Send(query))
                .MatchAsync(Results.Ok, CustomResults.Problem);
        })
        .WithName("GetMyProfile")
        .WithTags(Tags.Users)
        .WithSummary("Get the current user's profile.")
        .WithDescription("""
        Retrieves the profile information of the authenticated user.
        """)
        .Produces<UserProfileResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}
