using Microsoft.EntityFrameworkCore;
using UrlShortener.Abstractions.Persistence;
using UrlShortener.Domain.Users;

namespace UrlShortener.API.Features.Users.Register;

internal sealed class RegisterUserCommandHandler(
    IApplicationDbContext dbContext,
    IPasswordHasher passwordHasher)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    private const string DefaultImageUrl = "avatars/avatar.jpg";

    public async Task<Result<Guid>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        return await Result
            .From(dbContext.Users.AnyAsync(
                u => u.Email == command.Email,
                cancellationToken))
            .EnsureFalseAsync(UserErrors.EmailNotUnique)
            .MapAsync(_ => User.Register(
                 command.FirstName,
                 command.LastName,
                 command.Email,
                 passwordHasher.HashPassword(command.Password),
                 DefaultImageUrl))
            .TapAsync(async user => await dbContext.Users.AddAsync(user, cancellationToken))
            .TapAsync(_ => dbContext.SaveChangesAsync(cancellationToken))
            .MapAsync(user => user.Id);
    }
}