using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;

namespace Shortener.Common.Models;

[Collection("shortUrl")]
// [Index(nameof(ShortCode), IsUnique = true)]
public class ShortUrl
{
    public ObjectId Id { get; set; }
    public string OriginalUrl { get; set; }
    public string ShortCode { get; set; }
    public DateTime CreatedAt { get; set; }
}