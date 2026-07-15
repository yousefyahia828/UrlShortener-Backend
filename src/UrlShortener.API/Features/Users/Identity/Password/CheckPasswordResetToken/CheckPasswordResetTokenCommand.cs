using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.Identity.Password.CheckPasswordResetToken;

public sealed record CheckPasswordResetTokenQuery(string Token) : IQuery<bool>;
