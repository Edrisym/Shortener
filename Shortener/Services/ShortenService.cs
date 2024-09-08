using System.Text;
using Microsoft.Extensions.Options;
using Shortener.Models;

namespace Shortener.Services;

public class ShortenService : IShortenService
{
    private readonly StaticDataOption _options;

    public ShortenService(IOptions<StaticDataOption> options)
    {
        _options = options.Value;
    }

    public async Task<string> MakeShortenUrl(string longUrl, CancellationToken cancellationToken)
    {
        var hashCode = GenerateHashing(longUrl);
        var sixths = TakeHashPart(hashCode);
        return sixths;
    }

    private string TakeHashPart(string hashCode)
    {
        var sixth = string.Empty;
        for (var i = 0; i < hashCode.Length; i += 5)
        {
            sixth = i + _options.HashParts <= hashCode.Length
                ? hashCode[i..(i + _options.HashParts)]
                : hashCode[i..];

            Console.WriteLine(sixth);
        }

        return sixth;
    }

    private string GenerateHashing(string longUrl)
    {
        var pbText = Encoding.Default.GetBytes(longUrl);
        var hash = Blake3.Hasher.Hash(pbText);
        return hash.ToString();
    }
}