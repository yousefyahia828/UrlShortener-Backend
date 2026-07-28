using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain.Users;
using UrlShortener.Infrastructure.Database;

namespace UrlShortener.API.Features.Users.EditUsername;

internal sealed class EditUsernameCommandHandler(
    ApplicationDbContext context)
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
            .BindAsync(user => user.UpdateProfile(command.FirstName, command.LastName));
    }
}