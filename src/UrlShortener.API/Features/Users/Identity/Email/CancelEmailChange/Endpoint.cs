using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.Identity.Email.CancelEmailChange;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/cancel-email-change", async (
            ICurrentUser currentUser,
            ISender sender) =>
        {
            return await Result
                .From(new CancelEmailChangeCommand(currentUser.Id))
                .BindAsync(cmd => sender.Send(cmd))
                .MatchAsync(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .WithSummary("Cancel a pending email change request.")
        .WithDescription("Cancels the authenticated user's pending email change request.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
