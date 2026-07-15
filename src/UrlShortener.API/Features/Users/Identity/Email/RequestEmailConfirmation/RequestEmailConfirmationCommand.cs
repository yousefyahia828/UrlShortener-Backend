using Josephan.CQRS;

namespace UrlShortener.API.Features.Users.Identity.Email.RequestEmailConfirmation;

public sealed record RequestEmailConfirmationCommand(string Email) : ICommand;
