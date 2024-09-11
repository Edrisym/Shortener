namespace Shortener.IServices;

public interface IShortenService
{
    Task<string> MakeShortenUrl(string longUrl, CancellationToken cancellationToken);
}