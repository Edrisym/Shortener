using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Shortener.Common.Models;
using Shortener.Persistence;

namespace Shortener.Services;

public class ShortenService(
    IOptions<AppSettings> options,
    ShortenerDbContext dbContext,
    ILogger<ShortUrl> logger) : IShortenService
{
    public async Task<string> MakeShortUrl(string originalUrl, CancellationToken cancellationToken)
    {
        var shortCode = GenerateHashing(originalUrl);

        if (CheckDuplicateLongUrl(shortCode, originalUrl))
        {
            logger.LogWarning($"User requested a existing url => Url = {originalUrl} shortCode = {shortCode}");
            return $"{options.Value.BaseUrl}{shortCode}";
        }

        var urlEntity = new ShortUrl
        {
            CreatedAt = DateTime.UtcNow,
            OriginalUrl = originalUrl,
            ShortCode = shortCode
        };

        try
        {
            await dbContext.ShortUrl.AddAsync(urlEntity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError($"Database Error on shortening service while saving short code => {shortCode}");

            // TODO make a Result pattern
            throw new Exception("Error saving to database", ex);
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

        for (var i = 0; i < hashCode.Length; i += 7)
        {
            var segment = i + options.Value.HashParts <= hashCode.Length
                ? hashCode.Substring(i, options.Value.HashParts)
                : hashCode.Substring(i);

            segments.Add(segment);
        }

        return true;
    }

    public string ExtractHashFromSegments(IEnumerable<string> segments)
    {
        const int zero = 0;
        var hash = segments
            .Where(segment => segment.Length > zero)
            .Select(segment => segment[zero]);
        var shortCode = string.Join(string.Empty, hash);

        return shortCode;
    }
    
    public bool CheckDuplicateLongUrl(string shortCode, string originalUrl)
    {
        return dbContext.ShortUrl
            .Any(url => url.ShortCode == shortCode
                        && url.OriginalUrl == originalUrl);
    }

    public string GenerateHashing(string longUrl)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(longUrl));
        var base64Hash = Convert.ToBase64String(hashBytes);

        var cleanedHash = Regex.Replace(base64Hash, "[+/=]", string.Empty);

        return SegmentHashCode(cleanedHash, out var segments)
            ? ExtractHashFromSegments(segments)
            : string.Empty;
    }
}