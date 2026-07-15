using Josephan.CQRS;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using UrlShortener.API.Common.Abstractions.Pagination;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Extensions;
using UrlShortener.API.Common.Inftrastructure;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.ShortenUrls.Get;

public sealed class Endpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("urls", async (
            HttpContext httpContext,
            ICurrentUser currentUser,
            ISender sender,
            [Description("Page number (1-based).")][FromQuery] int? page = null,
            [Description("Number of items per page.")][FromQuery] int? pageSize = null,
            [Description("Sort expression, e.g. 'createdAt_desc'.")][FromQuery] string? order = null,
            [Description("Whether to return only enabled URLs or disabled URLs.")][FromQuery] bool enabled = true) =>
        {
            string baseUrl = httpContext.GetBaseUrl() + "/api/urls";
            Guid userId = currentUser.Id;

            var result = await sender.Send(
                new GetUserUrlsQuery(
                    userId,
                    baseUrl,
                    page,
                    pageSize,
                    order,
                    enabled));

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithName("GetUrls")
        .WithTags(Tags.Urls)
        .WithSummary("Get the current user's shortened URLs.")
        .WithDescription("""
        Retrieves the authenticated user's shortened URLs.
        
        Supports pagination, sorting, and filtering by enabled status.
        """)
        .Produces<PageResult<ShortenUrlResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization()
        .RequireRateLimiting("fixed");
    }
}
