namespace Shortener.Primitives.Response;

public class Error
{
    public static Error None => new(string.Empty, string.Empty);
    public static Error ValueNotFound => new("ValueNotFound", "داده درخواستی یافت نشد.");
    public Error(string code, string message)
    {
        ArgumentNullException.ThrowIfNull(code, nameof(code));
        ArgumentNullException.ThrowIfNull(message, nameof(message));
        Code = code;
        Message = message;
    }
    public string Code { get; init; }
    public string Message { get; init; }
}