using Josephan.CQRS;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;

namespace UrlShortener.API.Features.ShortenUrls.Redirect;

public sealed class Endpoint : IEndpoint
{
    public const string RouteName = "Redirect";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("urls/{code}", async (
            string code,
            ISender sender) =>
        {
            var result = await sender.Send(new RedirectToLongUrlQuery(code));

            return result.Match(
                longUrl => Results.Redirect(longUrl, permanent: false),
                CustomResults.Problem);
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
