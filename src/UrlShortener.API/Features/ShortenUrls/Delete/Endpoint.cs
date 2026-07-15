using Josephan.CQRS;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.ShortenUrls.Delete;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("urls/{id}", async (
            Guid id,
            ICurrentUser currentUser,
            ISender sender) =>
        {
            return await Result
                .From(new DeleteUrlCommand(currentUser.Id, id))
                .BindAsync(cmd => sender.Send(cmd))
                .MatchAsync(() => Results.NoContent(), CustomResults.Problem);
        })
        .WithName("DeleteUrl")
        .WithTags(Tags.Urls)
        .WithSummary("Delete a shortened URL.")
        .WithDescription("""
        Permanently deletes a shortened URL owned by the authenticated user.
        """)
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .RequireAuthorization()
        .RequireRateLimiting("fixed");
    }
}
