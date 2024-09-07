namespace Shortener.Services;

public class ShortenService : IShortenService
{
    public async Task<string> MakeShortenUrl(string url, CancellationToken cancellationToken)
    {
        return "a1jh3";
    }
}