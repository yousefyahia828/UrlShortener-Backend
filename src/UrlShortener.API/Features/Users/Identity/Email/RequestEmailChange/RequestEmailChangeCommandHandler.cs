using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.Identity.Email.RequestEmailChange;

internal sealed class RequestEmailChangeCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<RequestEmailChangeCommand>
{
    public async Task<Result> Handle(
        RequestEmailChangeCommand command,
        CancellationToken cancellationToken)
    {
        return await ResultExtensions
            .Ensure(
                await context.Users.AnyAsync(
                    u => u.Email == command.NewEmail,
                    cancellationToken),
                x => x == false,
                UserErrors.EmailNotUnique)
            .MapAsync(_ => context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == command.UserId,
                    cancellationToken))
            .EnsureNotNullAsync(UserErrors.NotFound)
            .BindAsync(u => u.RequestEmailChange(command.NewEmail))
            .TapAsync(() => context.SaveChangesAsync(cancellationToken));
    }
}