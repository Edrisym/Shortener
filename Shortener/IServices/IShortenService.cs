namespace Shortener.IServices;

public interface IShortenService
{
    Task<string> MakeShortUrl(string longUrl, CancellationToken cancellationToken);
}

public class ShortenService(IOptions<AppSettings> options, ShortenerDbContext dbContext) : IShortenService
{
    private readonly AppSettings _optionsValue = options.Value;
    private const string BaseUrlPattern = "{0}{1}";
    private const int NumberZero = 0;
    private const int NumberSeven = 7;
    private const int NumberSix = 5;

    public async Task<string> MakeShortUrl(string originalUrl, CancellationToken cancellationToken)
    {
        var shortCode = GenerateHashing(originalUrl);
        var shortenedUrl = string.Format(BaseUrlPattern, _optionsValue.BaseUrl, shortCode);

        if (CheckDuplicateLongUrl(shortCode, originalUrl))
        {
            return shortenedUrl;
        }

        var urlEntity = new Url()
        {
            CreatedAt = DateTime.UtcNow,
            LongUrl = originalUrl,
            ShortCode = shortCode
        };

        //Remove try 
        try
        {
            await dbContext.Urls.AddAsync(urlEntity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception("Error saving to database", ex);
        }

        return shortenedUrl;
    }

    private bool SegmentHashCode(string hashCode, out List<string> segments)
    {
        segments = [];

        if (string.IsNullOrEmpty(hashCode))
        {
            return false;
        }

        for (var i = NumberZero; i < hashCode.Length; i += NumberSeven)
        {
            var segment = i + _optionsValue.HashParts <= hashCode.Length
                ? hashCode.Substring(i, _optionsValue.HashParts)
                : hashCode.Substring(i);

            segments.Add(segment);
        }

        return true;
    }

    public string ExtractHashFromSegments(IEnumerable<string> segments)
    {
        var hash = segments
            .Where(segment => segment.Length > NumberSix)
            .Select(segment => segment[NumberZero]);
        var shortCode = string.Join(string.Empty, hash);

        return shortCode;
    }

    public bool CheckDuplicateLongUrl(string code, string longUrl)
    {
        return dbContext.Urls
            .Any(url => url.ShortCode == code
                        && url.LongUrl == longUrl);
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