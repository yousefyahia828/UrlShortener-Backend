using Josephan.CQRS;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.Identity.Email.RequestEmailChange;

public sealed class Endpoint : IEndpoint
{
    /// <summary>
    /// Represents a request to change the current user's email address.
    /// </summary>
    /// <param name="NewEmail">
    /// The new email address that will receive the confirmation email.
    /// </param>
    public sealed record ChangeEmailRequest(string NewEmail);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/request-email-change", async (
            [FromBody] ChangeEmailRequest request,
            ICurrentUser currentUser,
            ISender sender) =>
        {
            return await Result
                .From(request.MapToCommand(currentUser.Id))
                .BindAsync(cmd => sender.Send(cmd))
                .MatchAsync(Results.NoContent, CustomResults.Problem);
        })
        .WithName("RequestEmailChange")
        .WithTags(Tags.Users)
        .WithSummary("Request an email address change.")
        .WithDescription("""
        Initiates the email address change process for the authenticated user.
        
        A confirmation email is sent to the new email address. The email address is not changed until the confirmation link is successfully used.
        """)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
