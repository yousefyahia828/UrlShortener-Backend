namespace UrlShortener.API.Features.Users.GetMyProrile;

public sealed record UserProfileResponse
{
    public required Guid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required string? PendingEmail { get; init; }
    public required bool EmailVerified { get; init; }
    public required string ProfileImageUrl { get; init; }
    public required DateTime CreatedOnUtc { get; init; }
    public required int TotalUrls { get; init; }
    public required int ActiveUrls { get; init; }
    public required int DisabledUrls { get; init; }
}