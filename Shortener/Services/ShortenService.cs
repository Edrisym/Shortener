using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shortener.Common.Models;
using Shortener.Persistence;

namespace Shortener.Services;

public class ShortenService(IOptions<AppSettings> options, ShortenerDbContext dbContext) : IShortenService
{
    public async Task<string> MakeShortenUrl(string originalUrl, CancellationToken cancellationToken)
    {
        var shortCode = GenerateHashing(originalUrl);

        //TODO -- wth what is this
        if (CheckDuplicate(shortCode))
        {
            return $"{options.Value.BaseUrl}{shortCode}";
        }

        var urls = new ShortUrl
        {
            CreatedAt = DateTime.UtcNow,
            OriginalUrl = originalUrl,
            ShortCode = shortCode
        };
        await dbContext.ShortUrl.AddAsync(urls, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return $"{options.Value.BaseUrl}{shortCode}";
    }

    private bool SegmentHashCode(string hashCode, out List<string> segments)
    {
        segments = [];

        if (string.IsNullOrEmpty(hashCode))
        {
            return false;
        }

        for (var i = 0; i < hashCode.Length; i += 5)
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
        var index = 0;
        var hash = segments
            .Where(segment => segment.Length >= 4)
            .Select(segment => segment[index]);
        var shortCode = string.Join(string.Empty, hash);

        return shortCode;
    }


    private bool CheckDuplicate(string shortCode)
    {
        return dbContext.ShortUrl.Any(url => url.ShortCode == shortCode);
    }

    private string GenerateHashing(string longUrl)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(longUrl));
        var hashCode = Convert.ToBase64String(hash)
            .Replace(oldValue: "=", newValue: "");


        return SegmentHashCode(hashCode, out var segments)
            ? ExtractHashFromSegments(segments)
            : string.Empty;
    }
}