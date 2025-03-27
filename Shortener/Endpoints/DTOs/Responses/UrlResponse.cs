namespace Shortener.Endpoints.DTOs.Responses;

public record UrlResponse(
    string LongUrl,
    string ShortCode,
    DateTime CreatedAt,
    DateTime ExpiresAt);