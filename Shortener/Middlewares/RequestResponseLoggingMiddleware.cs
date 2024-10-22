using System.Text;

namespace Shortener.Middlewares;

public class RequestResponseLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestResponseLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger = logger;

    public async Task Invoke(HttpContext context)
    {
        LogRequest(context);

        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        await _next(context);
        await LogResponse(context);
        await responseBody.CopyToAsync(originalBodyStream);
    }

    private void LogRequest(HttpContext context)
    {
        var request = context.Request;

        var requestLog = new StringBuilder();
        requestLog.AppendLine("Incoming Request:");
        requestLog.AppendLine($"HTTP {request.Method} {request.Path}");
        requestLog.AppendLine($"Host: {request.Host}");
        requestLog.AppendLine($"Content-Type: {request.ContentType}");
        requestLog.AppendLine($"Content-Length: {request.ContentLength}");

        _logger.LogInformation(requestLog.ToString());
    }

    private async Task LogResponse(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();

        var responseLog = new StringBuilder();
        responseLog.AppendLine("Outgoing Response:");
        responseLog.AppendLine($"HTTP {context.Response.StatusCode}");
        responseLog.AppendLine($"Content-Type: {context.Response.ContentType}");
        responseLog.AppendLine($"Content-Length: {context.Response.ContentLength}");
        responseLog.AppendLine($"Response Body: {responseText}");

        _logger.LogInformation(responseLog.ToString());

        context.Response.Body.Seek(0, SeekOrigin.Begin);
    }
}