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
        if (await dbContext.Users.AnyAsync(
            u => u.Email == command.Email, cancellationToken))
        {
            return UserErrors.EmailNotUnique;
        }

        var user = User.Register(
            Guid.CreateVersion7(),
            command.FirstName,
            command.LastName,
            command.Email,
            passwordHasher.HashPassword(command.Password),
            DefaultImageUrl);

        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}