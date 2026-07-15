using Josephan.CQRS;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.Identity.Password.ChangePassword;

public sealed class Endpoint : IEndpoint
{
    /// <summary>
    /// Represents a request to change the current user's password.
    /// </summary>
    /// <param name="CurrentPassword">The user's current password.</param>
    /// <param name="NewPassword">The new password.</param>
    /// <param name="ConfirmNewPassword">Confirmation of the new password.</param>
    /// <param name="RevokeAllSessions">
    /// Indicates whether all existing sessions should be signed out after the password is changed.
    /// </param>
    public sealed record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword,
        string ConfirmNewPassword,
        bool RevokeAllSessions = true);


    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/change-password", async (
            [FromBody] ChangePasswordRequest request,
            HttpRequest httpRequest,
            ICurrentUser currentUser,
            ISender sender) =>
        {
            var refreshToken = httpRequest.Cookies[AuthHelpers.RefreshTokenCookieName];

            if (request.RevokeAllSessions &&
                string.IsNullOrWhiteSpace(refreshToken))
            {
                return Results.Unauthorized();
            }

            var command = request.MapToCommand(currentUser.Id, refreshToken);

            return await sender.Send(command)
                .MatchAsync(Results.NoContent, CustomResults.Problem);
        })
        .WithName("ChangePassword")
        .WithTags(Tags.Users)
        .WithSummary("Change the current user's password.")
        .WithDescription("""
        Changes the password of the authenticated user.
        
        The current password must be provided to verify the user's identity.
        If 'RevokeAllSessions' is true, all existing refresh tokens are revoked except the current session, requiring the user to sign in again on all devices except the current device.
        """)
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
    }
}
