using Josephan.CQRS;
using UrlShortener.Abstractions.Authentication.DTOs;

namespace UrlShortener.API.Features.Users.Refresh;

public sealed record RefreshUserCommand(string Token) : ICommand<AuthResponse>;
