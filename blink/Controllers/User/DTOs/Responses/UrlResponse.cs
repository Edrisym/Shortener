namespace blink.Controllers.User.DTOs.Responses;

public record UrlResponse(
    [Required] string LongUrl,
    [Required] string ShortCode,
    DateTime CreatedAt,
    DateTime ExpiresAt);