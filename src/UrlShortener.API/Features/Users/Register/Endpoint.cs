using Josephan.CQRS;
using UrlShortener.API.Common.Abstractions.Presentation;
using UrlShortener.API.Common.Inftrastructure;

namespace UrlShortener.API.Features.Users.Register;

public sealed class Endpoint : IEndpoint
{
    /// <summary>
    /// Register request.
    /// </summary>
    public sealed record RegisterUserRequest(
        /// <summary>User's first name.</summary>
        string FirstName,

        /// <summary>User's last name.</summary>
        string LastName,

        /// <summary>User's email address.</summary>
        string Email,

        /// <summary>User's password.</summary>
        string Password);

    public sealed record RegisterUserResponse(Guid Id);


    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("auth/register", async (RegisterUserRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.MapToCommand());

            return result.Match(
                value => Results.Ok(new RegisterUserResponse(value)),
                CustomResults.Problem);
        })
        .WithName("Register")
        .WithTags(Tags.Auth)
        .WithSummary("Register a new user.")
        .WithDescription("""
            Creates a new user account.
            
            On success, returns the unique identifier of the newly created user.
            If email verification is enabled, a verification email will be sent to the provided email address.
            """)
        .Produces<RegisterUserResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}