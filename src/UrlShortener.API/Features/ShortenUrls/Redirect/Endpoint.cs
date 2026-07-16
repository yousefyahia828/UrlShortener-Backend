using Microsoft.Extensions.Options;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.Domain.ShortenUrls;
using UrlShortener.Infrastructure;

namespace UrlShortener.API.Features.ShortenUrls.Redirect;

public sealed class Endpoint : IEndpoint
{
    public const string RouteName = "Redirect";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("urls/{code}", async (
            string code,
            ISender sender,
            IOptions<AppOptions> options) =>
        {
            var result = await sender.Send(new RedirectToLongUrlQuery(code));

            // certain errors
            if (result.IsFailure)
            {
                var error = result.Errors.First();

                var reason = error == UrlErrors.NotFound ? "notfound" : "disabled";

                return Results.Redirect(
                    options.Value.FrontendUrl + $"/not-found.html?reason={reason}");
            }

            return Results.Redirect(result.Value);
        })
        .WithName(RouteName)
        .WithTags(Tags.Urls)
        .WithSummary("Redirect to the original URL.")
        .WithDescription("""
        Redirects the client to the original URL associated with the specified short code.
        """)
        .Produces(StatusCodes.Status302Found)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .AllowAnonymous();
    }
}
