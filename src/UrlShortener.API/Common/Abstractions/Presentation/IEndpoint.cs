namespace UrlShortener.API.Common.Abstractions.Presentation;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}

