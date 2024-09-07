namespace Shortener.Services;

public interface IShortenService
{
    Task<string> MakeShortenUrl(string url, CancellationToken cancellationToken);
}