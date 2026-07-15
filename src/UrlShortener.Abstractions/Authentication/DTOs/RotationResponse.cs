using UrlShortener.Domain.Users;

namespace UrlShortener.Abstractions.Authentication.DTOs;


public sealed record RotationResponse
{
    public required User User { get; init; }
    public required RotationPayload Payload { get; init; }
}
