using UrlShortener.Abstractions.Authentication.DTOs;

namespace UrlShortener.API.Features.Users.Login;

public sealed record LoginUserCommand(
    string Email,
    string Password)
    : ICommand<AuthResponse>;
