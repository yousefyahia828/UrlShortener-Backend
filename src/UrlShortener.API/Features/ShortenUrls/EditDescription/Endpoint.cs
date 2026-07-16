using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.ShortenUrls.EditDescription;

public sealed class Endpoint : IEndpoint
{
    public sealed record EditDescriptionRequest(string? Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("urls/description/{urlId}", async (
            Guid urlId,
            [FromBody] EditDescriptionRequest request,
            ICurrentUser currentUser,
            ISender sender) =>
        {
            return (await sender
                .Send(new EditUrlDescriptionCommand(
                    currentUser.Id,
                    urlId,
                    request.Description)))
                .Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Urls)
        .RequireAuthorization()
        .RequireRateLimiting("fixed");
    }
}
