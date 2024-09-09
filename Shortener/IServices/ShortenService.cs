namespace Shortener.IServices;

public interface IShortenService
{
    string MakeShortenUrl(string longUrl, CancellationToken cancellationToken);
}