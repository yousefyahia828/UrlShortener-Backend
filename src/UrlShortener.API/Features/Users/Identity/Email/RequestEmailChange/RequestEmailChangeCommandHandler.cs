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
        if (await context.Users.AnyAsync(
            u => u.Email == command.NewEmail,
            cancellationToken))
        {
            return UserErrors.EmailNotUnique;
        }

        return await Result
            .From(context.Users
                .Where(u => u.Id == command.UserId)
                .FirstOrDefaultAsync(cancellationToken))
            .EnsureNotNullAsync(UserErrors.NotFound)
            .BindAsync(u => u.RequestEmailChange(command.NewEmail))
            .TapAsync(() => context.SaveChangesAsync(cancellationToken));
    }
}