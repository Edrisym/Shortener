namespace Shortener.IServices;

public interface IShortenService
{
    Task<string> ToShortUrl(string longUrl, CancellationToken cancellationToken);
}

public class ShortenService(
    IOptions<AppSettings> options,
    ShortenerDbContext dbContext) : IShortenService
{
    private readonly AppSettings _optionsValue = options.Value;
    private const string BaseUrlPattern = "{0}{1}";
    private const int NumberZero = 0;
    private const int NumberSeven = 7;
    private const int NumberSix = 5;

    public async Task<string> ToShortUrl(string originalUrl, CancellationToken cancellationToken)
    {
        var shortCode = GenerateShortCode(originalUrl);
        var shortUrl = string.Format(BaseUrlPattern, _optionsValue.BaseUrl, shortCode);

        if (IsDuplicatedUrl(shortCode, originalUrl))
            return shortUrl;

        var url = new Url
        {
            LongUrl = originalUrl,
            ShortCode = shortCode
        };

        //Remove try 
        try
        {
            await dbContext.Urls.AddAsync(url, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new Exception("Error saving to database", ex);
        }

        return shortUrl;
    }

    public string GenerateShortCode(string longUrl)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(longUrl));
        var base64Hash = Convert.ToBase64String(hashBytes);

        var cleanedHash = Regex.Replace(base64Hash, "[+/=]", string.Empty);

        return SegmentAndBuildHashCode(cleanedHash);
    }

    private string SegmentAndBuildHashCode(string hashCode)
    {
        if (string.IsNullOrEmpty(hashCode))
            return string.Empty;

        var segments = new List<string>();
        var sb = new StringBuilder();

        for (var i = 0; i < hashCode.Length; i += _optionsValue.HashParts)
        {
            var segment = i + _optionsValue.HashParts <= hashCode.Length
                ? hashCode.Substring(i, _optionsValue.HashParts)
                : hashCode.Substring(i);

            segments.Add(segment);
        }

        foreach (var segment in segments)
        {
            if (segment.Length > NumberSix)
            {
                sb.Append(segment[NumberZero]);
            }
        }

        return sb.ToString();
    }

    private bool IsDuplicatedUrl(string code, string longUrl)
    {
        return dbContext.Urls
            .Any(url => url.ShortCode == code
                        && url.LongUrl == longUrl);
    }
}