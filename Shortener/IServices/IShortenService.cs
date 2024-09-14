namespace Shortener.IServices;

public interface IShortenService
{
    Task<string> MakeShortUrl(string longUrl, CancellationToken cancellationToken);
    string GenerateHashing(string longUrl);
    bool CheckDuplicateLongUrl(string shortCode, string originalUrl);
    string ExtractHashFromSegments(IEnumerable<string> segments);
}