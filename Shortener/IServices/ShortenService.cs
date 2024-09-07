namespace Shortener.Services;

public interface IShortenService
{
    Task<string> MakeShortenUrl(string longUrl, CancellationToken cancellationToken);
}