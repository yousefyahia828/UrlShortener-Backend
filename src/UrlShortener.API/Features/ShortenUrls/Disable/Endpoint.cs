using Josephan.CQRS;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.ShortenUrls.Disable;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("urls/disable/{urlId}", async (
            Guid urlId,
            ICurrentUser currentUser,
            ISender sender) =>
        {
            return await Result
                .From(new DisableUrlCommand(currentUser.Id, urlId))
                .BindAsync(cmd => sender.Send(cmd))
                .MatchAsync(Results.NoContent, CustomResults.Problem);
        })
        .WithName("DisableUrl")
        .WithTags(Tags.Urls)
        .WithSummary("Disable a shortened URL.")
        .WithDescription("""
        Disables a shortened URL owned by the authenticated user.
        
        If the URL is already disabled, the request may return a conflict.
        """)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
