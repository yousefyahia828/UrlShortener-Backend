namespace UrlShortener.API.Common.Abstractions.Behaviors;

internal sealed class CommandLoggingBehavior<TCommand, TResposne>(
    ILogger<CommandLoggingBehavior<TCommand, TResposne>> logger)
    : ICommandPipelineBehavior<TCommand, TResposne>
    where TCommand : IRequest<TResposne>
    where TResposne : Result
{
    public async Task<TResposne> Handle(
        TCommand request,
        RequestHandlerDelegate<TCommand, TResposne> next,
        CancellationToken cancellationToken)
    {
        var commandName = typeof(TCommand).Name;

        logger.LogInformation("Processing command {Command}", commandName);

        var result = await next(request, cancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation("Completed command {Command}", commandName);
        }
        else
        {
            logger.LogError(
                "Completed command {Command} with errors {@Errors}",
                commandName,
                result.Errors);
        }

        return result;
    }
}