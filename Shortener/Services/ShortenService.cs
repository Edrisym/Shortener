using System.Text;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shortener.Common.Models;
using Shortener.Persistence;

namespace Shortener.Services;

public class ShortenService(IOptions<AppSettings> options, ShortenerDbContext dbContext) : IShortenService
{
    public async Task<string> MakeShortenUrl(string originalUrl, CancellationToken cancellationToken)
    {
        var shortCode = GenerateHashing(originalUrl);
        try
        {
            var urls = new ShortUrl
            {
                CreatedAt = DateTime.UtcNow,
                OriginalUrl = originalUrl,
                ShortCode = shortCode
            };
            await dbContext.ShortUrl.AddAsync(urls, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            if (e.Message.Contains("Duplicate"))
            {
                // TODO -- Think more
            }
        }

        return $"{options.Value.BaseUrl}{shortCode}";
    }

    private bool SegmentHashCode(string hashCode, out List<string> segments)
    {
        segments = [];

        if (string.IsNullOrEmpty(hashCode))
        {
            return false;
        }

        for (var i = 0; i < hashCode.Length; i += 10)
        {
            var segment = i + options.Value.HashParts <= hashCode.Length
                ? hashCode[i..(i + options.Value.HashParts)]
                : hashCode[i..];

            segments.Add(segment);
        }

        return true;
    }

    private string ExtractHashFromSegments(IEnumerable<string> segments)
    {
        var hash = new List<char>();

        foreach (var segment in segments.Where(x => x.Length > 5))
        {
            var firstLetter = segment[0];
            hash.Add(firstLetter);
        }

        var output = string.Join(string.Empty, hash);
        return output;
    }

    private string GenerateHashing(string longUrl)
    {
        var bytes = Encoding.UTF8.GetBytes(longUrl);
        var hashCode = Base64UrlEncoder.Encoder.Encode(bytes);

        return SegmentHashCode(hashCode, out var segments)
            ? ExtractHashFromSegments(segments)
            : string.Empty;
    }
}