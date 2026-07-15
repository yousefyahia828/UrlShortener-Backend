namespace UrlShortener.Infrastructure.Users;

public sealed class ProfileImageOptions
{
    public const string SectionName = "ProfileImage";

    public int MaximumSizeInMegaBytes { get; set; }
    public required HashSet<string> AllowedContentTypes { get; set; }
    public required HashSet<string> AllowedExtensions { get; set; }
}
