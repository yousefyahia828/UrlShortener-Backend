using UrlShortener.Abstractions.Idempotency;

namespace UrlShortener.API.Features.Users.Register;

public sealed record RegisterUserCommand(
    Guid CommandId,
    string FirstName,
    string LastName,
    string Email,
    string Password) : IdempotentCommand<Guid>(CommandId);
