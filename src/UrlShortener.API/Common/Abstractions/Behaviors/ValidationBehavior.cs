using FluentValidation;
using Josephan.CQRS;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace UrlShortener.API.Common.Abstractions.Behaviors;

internal sealed class ValidationBehavior<TRequest, TResponse>
    : ICommandPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    private static readonly ConcurrentDictionary<Type, Func<Error[], Result>> _failureFactories = new();

    private static readonly MethodInfo _genericFailureMethod = typeof(Result)
        .GetMethods()
        .Where(m => m.Name == nameof(Result.Failure) &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 1 &&
                    m.GetParameters()[0].ParameterType == typeof(Error[]))
        .First();

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TRequest, TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(request, cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .Select(f =>
            {
                var propertyName = f.PropertyName;
                var camelCase = char.ToLowerInvariant(propertyName[0]) + propertyName[1..];

                return Error.Validation(camelCase, f.ErrorMessage);
            })
            .ToArray();

        if (errors.Length == 0)
        {
            return await next(request, cancellationToken);
        }

        var failure = GetOrCreateFailure();

        return (TResponse)failure(errors);
    }

    private static Func<Error[], Result> GetOrCreateFailure()
    {
        return _failureFactories.GetOrAdd(typeof(TRequest),
            static type =>
            {
                if (typeof(TResponse) == typeof(Result))
                {
                    return errors => Result.Failure(errors);
                }

                MethodInfo closedMethod = _genericFailureMethod
                    .MakeGenericMethod(typeof(TResponse).GenericTypeArguments[0]);

                ParameterExpression errorsParam = Expression.Parameter(typeof(Error[]), "errors");

                MethodCallExpression body = Expression.Call(closedMethod, errorsParam);

                return Expression
                    .Lambda<Func<Error[], Result>>(body, errorsParam)
                    .Compile();
            });
    }
}