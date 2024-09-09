namespace Shortener.Services;

public interface IShortenService
{
    string MakeShortenUrl(string longUrl, CancellationToken cancellationToken);
}