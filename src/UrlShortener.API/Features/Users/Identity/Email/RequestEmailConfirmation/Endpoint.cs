using Josephan.CQRS;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;

namespace UrlShortener.API.Features.Users.Identity.Email.RequestEmailConfirmation;

public sealed class Endpoint : IEndpoint
{
    /// <summary>
    /// Represents a request to send an email verification link.
    /// </summary>
    /// <param name="Email">
    /// The email address associated with the account.
    /// </param>
    public sealed record EmailConfirmationRequest(string Email);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/request-email-verification", async (
            [FromBody] EmailConfirmationRequest request,
            ISender sender) =>
        {
            return await Result
                .From(new RequestEmailConfirmationCommand(request.Email))
                .BindAsync(cmd => sender.Send(cmd))
                .MatchAsync(Results.NoContent, CustomResults.Problem);
        })
        .WithName("RequestEmailVerification")
        .WithTags(Tags.Auth)
        .WithSummary("Request an email verification link.")
        .WithDescription("""
        Sends a new email verification link to the specified email address.
        
        If the account exists and is not yet verified, a new verification email is sent.
        """)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
