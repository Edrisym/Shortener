using System.Text;
using Microsoft.Extensions.Options;
using Shortener.Models;

namespace Shortener.Services;

public class ShortenService(IOptions<StaticDataOption> options) : IShortenService
{
    private readonly StaticDataOption _options = options.Value;

    public string MakeShortenUrl(string longUrl, CancellationToken cancellationToken)
    {
        var hashCode = GenerateHashing(ref longUrl);
        var sixths = TakeHashPart(ref hashCode);
        return sixths;
    }

    private string TakeHashPart(ref string hashCode)
    {
        var segments = new List<string>();

        for (var i = 0; i < hashCode.Length; i += 10)
        {
            var sixth = i + _options.HashParts <= hashCode.Length
                ? hashCode[i..(i + _options.HashParts)]
                : hashCode[i..];

            segments.Add(sixth);
        }

        var hash = new List<char>();

        foreach (var item in segments.Where(x => x.Length > 5))
        {
            var firstLetter = item[0];
            hash.Add(firstLetter);
        }

        var output = string.Join(string.Empty, hash);
        return output;
    }

    private string GenerateHashing(ref string longUrl)
    {
        var bytes = Encoding.UTF8.GetBytes(longUrl);
        return Base64UrlEncoder.Encoder.Encode(bytes);
    }
}