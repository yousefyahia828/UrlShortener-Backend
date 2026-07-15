using Microsoft.AspNetCore.Http;

namespace UrlShortener.Abstractions.Infrastructure;

public interface ILocalStorage
{
    Task<string> SaveFileAsync(string folder, IFormFile file, CancellationToken cancellationToken = default);
}