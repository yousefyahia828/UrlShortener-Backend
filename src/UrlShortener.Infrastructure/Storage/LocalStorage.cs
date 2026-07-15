using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using UrlShortener.Abstractions.Infrastructure;

namespace UrlShortener.Infrastructure.Storage;

internal sealed class LocalStorage(IWebHostEnvironment env) : ILocalStorage
{
    private readonly string _rootPath = env.WebRootPath;

    public async Task<string> SaveFileAsync(
        string folder,
        IFormFile file,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(file);

        if (file.Length == 0)
        {
            throw new ArgumentException("File is empty.", nameof(file));
        }

        string safeFolder = NormalizeFolder(folder);
        string directoryPath = Path.Combine(_rootPath, safeFolder);
        Directory.CreateDirectory(directoryPath);

        string normalizedFileName = NormalizeFileName(file.FileName);
        string uniqueFileName = $"{Guid.CreateVersion7()}-{normalizedFileName}";
        string filePath = Path.Combine(directoryPath, uniqueFileName);

        await using (Stream stream = File.Create(filePath))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        return Path.Combine(safeFolder, uniqueFileName).Replace('\\', '/');
    }

    private static string NormalizeFolder(string folder)
    {
        string trimmed = folder.Trim('/', '\\');

        if (trimmed.Contains("..", StringComparison.Ordinal))
        {
            throw new ArgumentException("Invalid folder path.", nameof(folder));
        }

        return trimmed;
    }

    private static string NormalizeFileName(string fileName)
    {
        string name = Path.GetFileName(fileName);

        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }

        return name;
    }
}