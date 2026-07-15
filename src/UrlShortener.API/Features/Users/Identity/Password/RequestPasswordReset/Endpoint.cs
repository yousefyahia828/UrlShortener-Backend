using Josephan.CQRS;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;

namespace UrlShortener.API.Features.Users.Identity.Password.RequestPasswordReset;

public sealed class Endpoint : IEndpoint
{
    /// <summary>
    /// Represents a request to send a password reset link.
    /// </summary>
    /// <param name="Email">
    /// The email address associated with the account.
    /// </param>
    public sealed record ResetPasswordRequest(string Email);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/forgot-password", async (
            [FromBody] ResetPasswordRequest request,
            ISender sender) =>
        {
            return await sender.Send(new RequestPasswordResetCommand(request.Email))
                .MatchAsync(Results.NoContent, CustomResults.Problem);
        })
        .WithName("ForgotPassword")
        .WithTags(Tags.Auth)
        .WithSummary("Request a password reset link.")
        .WithDescription(
        """
        Sends a password reset link to the specified email address, if applicable.
        
        For security reasons, this endpoint always returns the same response regardless of whether the email address is associated with an account.
        """)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .RequireRateLimiting("fixed");
    }
}
