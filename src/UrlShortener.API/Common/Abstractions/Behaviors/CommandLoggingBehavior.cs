using Josephan.CQRS;
using Microsoft.Extensions.Logging;
using Serilog.Context;

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
            using (LogContext.PushProperty("Errors", result.Errors, true))
            {
                logger.LogError("Completed command {Command} with errors", commandName);
            }
        }

        return result;
    }
}
