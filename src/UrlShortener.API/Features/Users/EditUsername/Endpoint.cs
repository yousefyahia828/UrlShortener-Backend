using Josephan.CQRS;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.EditUsername;

public sealed class Endpoint : IEndpoint
{
    /// <summary>
    /// Represents a request to update the current user's profile.
    /// </summary>
    /// <param name="FirstName">The user's first name.</param>
    /// <param name="LastName">The user's last name.</param>
    public sealed record EditUsernameRequest(
        string FirstName,
        string LastName);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/change-name", async (
            [FromBody] EditUsernameRequest request,
            ICurrentUser currentUser,
            ISender sender) =>
        {
            return await sender.Send(request.MapToCommand(currentUser.Id))
                .MatchAsync(Results.NoContent, CustomResults.Problem);
        })
        .WithName("EditUsername")
        .WithTags(Tags.Users)
        .WithSummary("Update the current user's name.")
        .WithDescription("""
        Updates the authenticated user's profile information.
        
        Only the user's first name and last name can be updated.
        """)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
