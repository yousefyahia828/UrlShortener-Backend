using Josephan.CQRS;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;

namespace UrlShortener.API.Features.Users.Logout;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/logout", async (HttpContext httpContext, ISender sender) =>
        {
            var token = httpContext.Request.Cookies[AuthHelpers.RefreshTokenCookieName];
            if (string.IsNullOrWhiteSpace(token))
            {
                return Results.Unauthorized();
            }

            return await Result
                .From(new LogoutUserCommand(token))
                .BindAsync(cmd => sender.Send(cmd))
                .TapAsync(() => AuthHelpers.RemoveRefreshCookie(httpContext))
                .MatchAsync(Results.NoContent, CustomResults.Problem);
        })
        .WithName("Logout")
        .WithTags(Tags.Auth)
        .WithSummary("Sign out the current user.")
        .WithDescription("""
        Signs out the current user using the refresh token stored in the HttpOnly cookie.
        
        On success:
        - Revokes the refresh token.
        - Removes the refresh token cookie.
        - Returns no content.
        """)
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
