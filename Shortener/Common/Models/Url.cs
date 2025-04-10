using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;

namespace Shortener.Common.Models;

[Collection("Url")]
public class Url : BaseEntity<string>
{
    public string LongUrl { get; set; }
    public string ShortCode { get; set; }
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.Add(TimeSpan.FromDays(3));
    // public IReadOnlyCollection<string> Viewers { get; set; }
}