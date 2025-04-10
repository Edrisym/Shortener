using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;

namespace blink.Common.Models;

[Collection("Url")]
public class Url : BaseEntity<string>
{
    public Url(string longUrl, string shortCode)
    {
        Id = ObjectId.GenerateNewId().ToString();
        LongUrl = longUrl;
        ShortCode = shortCode;
        ExpiresAt = DateTime.UtcNow.Add(TimeSpan.FromDays(3));
    }

    public string LongUrl { get; set; }
    public string ShortCode { get; set; }
    public DateTime ExpiresAt { get; set; }
    // public IReadOnlyCollection<string> Viewers { get; set; }
}