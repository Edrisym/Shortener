using System.Text;
using Microsoft.Extensions.Options;
using Shortener.Common.Models;

namespace Shortener.Services;

public class ShortenService(IOptions<AppSettings> options) : IShortenService
{
    public string MakeShortenUrl(string longUrl, CancellationToken cancellationToken)
    {
        var hashCode = GenerateHashing(longUrl);

        return SegmentHashCode(hashCode, out var segments)
            ? ExtractHashFromSegments(segments)
            : string.Empty;
    }

    private bool SegmentHashCode(string hashCode, out List<string> segments)
    {
        segments = new List<string>();

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

    private string ExtractHashFromSegments(List<string> segments)
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
        return Base64UrlEncoder.Encoder.Encode(bytes);
    }
}