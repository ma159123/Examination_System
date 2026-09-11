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
        // 1. Check if TResponse is generic (e.g., Result<LoginResponse>)
        if (typeof(TResponse).IsGenericType)
        {
            var valueType = typeof(TResponse).GetGenericArguments()[0];

            // البحث عن دالة Failure الجينيريك التي تأخذ معاملين (Error, string?)
            var failureMethod = typeof(Result)
                .GetMethods()
                .First(m => m.Name == nameof(Result.Failure)
                            && m.IsGenericMethod
                            && m.GetParameters().Length == 2);

            var genericMethod = failureMethod.MakeGenericMethod(valueType);

            // تمرير المعاملين: الأول Error والثاني message (null)
            return (TResponse)genericMethod.Invoke(null, new object?[] { error, null })!;
        }

        // 2. If TResponse is non-generic Result
        return (TResponse)(object)Result.Failure(error);
    }
}