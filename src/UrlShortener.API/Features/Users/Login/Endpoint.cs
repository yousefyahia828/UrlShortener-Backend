using Microsoft.AspNetCore.Mvc;
using UrlShortener.Abstractions.Authentication.DTOs;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;

namespace UrlShortener.API.Features.Users.Login;

public sealed class Endpoint : IEndpoint
{
    /// <summary>
    /// Login request.
    /// </summary>
    /// <param name="Email">Email address.</param>
    /// <param name="Password">Password.</param>
    public sealed record LoginUserRequest(string Email, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/login", async (
            [FromBody] LoginUserRequest request,
            ISender sender,
            HttpContext httpContext) =>
        {
            return await Result
                .From(request.MapToCommand())
                .BindAsync(cmd => sender.Send(cmd))
                .TapAsync(res => AuthHelpers
                    .SetRefreshCookie(
                        httpContext,
                        res.RefreshToken,
                        res.ExpiresOnUtc))
                .MapAsync(res => res.Payload)
                .MatchAsync(Results.Ok, CustomResults.Problem);
        })
        .WithName("Login")
        .WithTags(Tags.Auth)
        .WithSummary("Authenticate a user.")
        .WithDescription("""
        Authenticates a user using their email and password.
        
        On success:
        - Returns the access token and user information.
        - Stores the refresh token in a secure, HttpOnly cookie.
        
        The refresh token is never included in the response body.
        """)
        .Produces<AuthPayload>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}
