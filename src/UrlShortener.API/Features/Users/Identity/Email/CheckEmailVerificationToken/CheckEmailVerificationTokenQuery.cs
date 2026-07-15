using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.Identity.Email.CheckEmailVerificationToken;

public sealed record CheckEmailVerificationTokenQuery(Guid UserId, Guid TokenId)
    : IQuery<bool>;
