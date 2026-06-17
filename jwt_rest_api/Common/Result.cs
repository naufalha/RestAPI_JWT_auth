using System;

namespace jwt_rest_api.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; }
    public ResultType Type { get; }

    protected Result(bool isSuccess, ResultType type, string errorMessage)
    {
        IsSuccess = isSuccess;
        Type = type;
        ErrorMessage = errorMessage ?? throw new ArgumentNullException(nameof(errorMessage));
    }

    public static Result Success() => new(true, ResultType.Success, string.Empty);
    public static Result Failure(ResultType type, string errorMessage) => new(false, type, errorMessage);
    public static Result NotFound(string errorMessage) => new(false, ResultType.NotFound, errorMessage);
    public static Result ValidationError(string errorMessage) => new(false, ResultType.ValidationError, errorMessage);
    public static Result Unauthorized(string errorMessage) => new(false, ResultType.Unauthorized, errorMessage);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T? value, bool isSuccess, ResultType type, string errorMessage)
        : base(isSuccess, type, errorMessage)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(value, true, ResultType.Success, string.Empty);
    
    public static new Result<T> Failure(ResultType type, string errorMessage) 
        => new(default, false, type, errorMessage);
        
    public static new Result<T> NotFound(string errorMessage) 
        => new(default, false, ResultType.NotFound, errorMessage);
        
    public static new Result<T> ValidationError(string errorMessage) 
        => new(default, false, ResultType.ValidationError, errorMessage);
        
    public static new Result<T> Unauthorized(string errorMessage) 
        => new(default, false, ResultType.Unauthorized, errorMessage);
}
