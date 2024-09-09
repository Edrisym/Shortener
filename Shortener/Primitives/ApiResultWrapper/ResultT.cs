using Shortener.Primitives.Response;

namespace Shortener.Primitives.ApiResultWrapper;

public class Result<TValue>(
    bool isSuccess,
    string message,
    Error[] errors,
    TValue? value) : Result(isSuccess, message, errors)
{
    public TValue? Data => value;

    public static implicit operator Result<TValue>(TValue value)
        => Create(value);

    public static Result<TValue> Success(TValue value)
        => new(true, BaseMessages.OperationSuccessfulMessage.Text, [], value);

    private static Result<TValue> Create(TValue value)
    {
        return new Result<TValue>(true, BaseMessages.OperationSuccessfulMessage.Text, [], value);
    }
}