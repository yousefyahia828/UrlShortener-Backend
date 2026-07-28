using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Users;
using UrlShortener.Infrastructure.Database;

namespace UrlShortener.API.Features.Users.Identity.Email.RequestEmailChange;

internal sealed class RequestEmailChangeCommandHandler(
    ApplicationDbContext context)
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
            .BindAsync(u => u.RequestEmailChange(command.NewEmail));
    }
}