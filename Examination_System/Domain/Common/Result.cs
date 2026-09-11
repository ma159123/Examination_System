using Domain.Common.Errors;
using System.Text.Json.Serialization;

namespace Domain.Common;

public class Result
{
    protected Result(bool isSuccess, string? message, Error error, object? meta = null)
    {
        if ((isSuccess && error != Error.None) || (!isSuccess && error == Error.None))
            throw new InvalidOperationException("Invalid result state.");

        IsSuccess = isSuccess;
        Message = message ?? (isSuccess ? "Operation completed successfully." : error.Description);
        Error = isSuccess ? null : error;
        Meta = meta;
    }

    [JsonPropertyName("success")]
    public bool IsSuccess { get; }

    [JsonIgnore]
    public bool IsFailure => !IsSuccess;

    [JsonPropertyName("message")]
    public string Message { get; }

    [JsonPropertyName("error")]
    public Error? Error { get; }

    [JsonPropertyName("meta")]
    public object? Meta { get; }

    // Static Factory Methods for non-generic Result
    public static Result Success(string message = "Operation completed successfully.", object? meta = null)
        => new(true, message, Error.None, meta);

    public static Result Failure(Error error, string? message = null)
        => new(false, message, error);

    // Static Factory Methods for generic Result<TValue>
    public static Result<TValue> Success<TValue>(TValue data, string message = "Operation completed successfully.", object? meta = null)
        => new(data, true, message, Error.None, meta);

    public static Result<TValue> Failure<TValue>(Error error, string? message = null)
        => new(default, false, message, error);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, string? message, Error error, object? meta = null)
        : base(isSuccess, message, error, meta)
    {
        _value = value;
    }

    [JsonPropertyName("data")]
    public TValue? Data => IsSuccess ? _value : default;

    [JsonIgnore]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Failure results cannot have a value.");

    // Implicit Operators
    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);
}