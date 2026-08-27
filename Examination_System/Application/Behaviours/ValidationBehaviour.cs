using Domain.Common;
using Domain.Common.Errors;
using FluentValidation;
using MediatR;

namespace Application.Behaviours;

public sealed class ValidationBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            var errorMessage = string.Join(" ", failures.Select(f => f.ErrorMessage));

            var validationError = new Error(
                "Validation.Error",
                errorMessage,
                Microsoft.AspNetCore.Http.StatusCodes.Status422UnprocessableEntity // 422
            );

            return CreateFailureResult(validationError);
        }

        return await next();
    }
    private TResponse CreateFailureResult(Error error)
    {
        // 1. Check if TResponse is generic (e.g., Result<RegisterResponse>)
        if (typeof(TResponse).IsGenericType)
        {
            var valueType = typeof(TResponse).GetGenericArguments()[0];

            // Calling Result.Failure<TValue>(Error error) via Reflection
            var failureMethod = typeof(Result)
                .GetMethods()
                .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethod)
                .MakeGenericMethod(valueType);

            return (TResponse)failureMethod.Invoke(null, new object[] { error })!;
        }

        // 2. If TResponse is non-generic Result
        return (TResponse)Result.Failure(error);
    }
}