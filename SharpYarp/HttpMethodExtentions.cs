namespace SharpYarp;

public enum HttpMethodEnum
{
    Get,
    Post,
    Put,
    Delete,
    Unsupported,
}

public static class HttpMethodExtensions
{
    public static HttpMethodEnum ToHttpMethodEnum(this string method)
    {
        return method.ToUpperInvariant() switch
        {
            "GET" => HttpMethodEnum.Get,
            "POST" => HttpMethodEnum.Post,
            "PUT" => HttpMethodEnum.Put,
            "DELETE" => HttpMethodEnum.Delete,
            _ => HttpMethodEnum.Unsupported
        };
    }
}