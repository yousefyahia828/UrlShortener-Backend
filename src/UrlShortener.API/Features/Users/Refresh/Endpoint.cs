using Josephan.CQRS;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;

namespace UrlShortener.API.Features.Users.Refresh;

public sealed class Endpoint : IEndpoint
{
    public sealed record RefreshResponse(
        string AccessToken,
        DateTime ExpiresOnUtc);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/refresh", async (HttpContext httpContext, ISender sender) =>
        {
            var token = httpContext.Request.Cookies[AuthHelpers.RefreshTokenCookieName];

            if (string.IsNullOrWhiteSpace(token))
            {
                return Results.Unauthorized();
            }

            return await Result
                .From(new RefreshUserCommand(token))
                .BindAsync(cmd => sender.Send(cmd))
                .TapAsync(res => AuthHelpers.SetRefreshCookie(httpContext, res.RefreshToken, res.ExpiresOnUtc))
                .MatchAsync(
                    value => Results.Ok(new RefreshResponse(value.Payload.AccessToken, value.Payload.ExpiresOnUtc)),
                    CustomResults.Problem);
        })
        .WithName("Refresh")
        .WithTags(Tags.Auth)
        .WithSummary("Refresh an access token.")
        .WithDescription("""
        Generates a new access token using the refresh token stored in the HttpOnly cookie.
        
        On success:
        - Returns a new access token and its expiration time.
        - Rotates the refresh token and stores the new one in a secure HttpOnly cookie.
        
        If the refresh token is missing, invalid, expired, or revoked, the request is rejected.
        """)
        .Produces<RefreshResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
