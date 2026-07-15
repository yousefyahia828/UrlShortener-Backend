using Josephan.CQRS;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.EditUsername;

internal sealed class EditUsernameCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<EditUsernameCommand>
{
    public async Task<Result> Handle(
        EditUsernameCommand command,
        CancellationToken cancellationToken)
    {
        return await Result
            .From(context.Users
                .FirstOrDefaultAsync(u => u.Id == command.UserId, cancellationToken))
            .EnsureNotNullAsync(UserErrors.NotFound)
            .BindAsync(user => user.UpdateProfile(command.FirstName, command.LastName))
            .TapAsync(() => context.SaveChangesAsync(cancellationToken));
    }
}