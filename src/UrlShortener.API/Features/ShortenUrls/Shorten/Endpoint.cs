using Josephan.CQRS;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.ShortenUrls.Shorten;

public sealed class Endpoint : IEndpoint
{
    /// <summary>
    /// Represents a request to create a shortened URL.
    /// </summary>
    /// <param name="LongUrl">
    /// The original URL to shorten.
    /// </param>
    public sealed record ShortenUrlRequest(string LongUrl, string? Description);

    /// <summary>
    /// Represents the result of a successful URL shortening operation.
    /// </summary>
    /// <param name="ShortUrl">
    /// The generated shortened URL.
    /// </param>
    public sealed record ShortenUrlResponse(string ShortUrl);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("urls/shorten", async (
            [FromBody] ShortenUrlRequest request,
            HttpContext httpContext,
            ICurrentUser currentUser,
            ISender sender,
            LinkGenerator linkGenerator) =>
        {
            var result = await sender.Send(request.MapToCommand(currentUser.Id));

            return result.Match(
                code =>
                {
                    string shortenUrl = linkGenerator.GetUriByName(
                        httpContext,
                        Redirect.Endpoint.RouteName,
                        new { code })!;

                    return Results.Ok(new ShortenUrlResponse(shortenUrl));
                },
                CustomResults.Problem);
        })
        .WithName("ShortenUrl")
        .WithTags(Tags.Urls)
        .WithSummary("Create a shortened URL.")
        .WithDescription("""
        Creates a new shortened URL for the authenticated user.
        
        On success, returns the absolute shortened URL that can be used to redirect to the original URL.
        """)
        .Produces<ShortenUrlResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .RequireRateLimiting("fixed");
    }
}
