namespace Shortener.Services;

public interface IShortenService
{
    string ShortenUrl(string url);
}