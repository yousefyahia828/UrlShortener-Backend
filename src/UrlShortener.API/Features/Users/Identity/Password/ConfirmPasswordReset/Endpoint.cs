using Josephan.CQRS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.API.Features.Users.Identity.Password.CheckPasswordResetToken;
using UrlShortener.Infrastructure;

namespace UrlShortener.API.Features.Users.Identity.Password.ConfirmPasswordReset;

public sealed class Endpoint : IEndpoint
{
    /// <summary>
    /// Represents a request to reset a user's password.
    /// </summary>
    /// <param name="Token">The password reset token.</param>
    /// <param name="NewPassword">The new password.</param>
    /// <param name="ConfirmNewPassword">The confirmation of the new password.</param>
    public sealed record ResetPasswordRequest(
        string Token,
        string NewPassword,
        string ConfirmNewPassword);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/reset-password", async (
            [FromBody] ResetPasswordRequest request,
            ISender sender) =>
        {
            return await sender.Send(request.MapToCommand())
                .MatchAsync(Results.NoContent, CustomResults.Problem);
        })
        .WithName("Reset Password")
        .WithTags(Tags.Auth)
        .WithSummary("Reset a user's password.")
        .WithDescription("""
        Resets a user's password using a valid password reset token.
        
        The password reset token is obtained from the password reset link sent to the user's email address.
        """)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest);


        app.MapGet("auth/password-reset-link", async (
            [FromQuery] string token,
            ISender sender,
            IOptions<AppOptions> options) =>
        {
            var query = new CheckPasswordResetTokenQuery(token);

            var result = await sender.Send(query);

            var frontendUrl = options.Value.FrontendUrl;

            // 100% success
            return result.Value ?
                Results.Redirect(frontendUrl + $"/reset-password.html?token={token}") :
                Results.Redirect(frontendUrl + "/invalid-link.html?type=password");
        })
        .WithName("PasswordResetRedirector")
        .WithSummary("Validate a password reset link.")
        .WithDescription(
        """
        Validates a password reset token received from a password reset email.
        
        If the token is valid, the client is redirected to the frontend password reset page where the user can choose a new password. Otherwise, the client is redirected to an invalid link page.
        """)
        .Produces(StatusCodes.Status302Found);
    }
}
