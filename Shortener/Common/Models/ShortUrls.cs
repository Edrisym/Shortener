using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;

namespace Shortener.Common.Models;

[Collection("shortUrls")]
public class ShortUrls
{
    public ObjectId Id { get; set; }
    public string OriginalUrl { get; set; }
    public string ShortCode { get; set; }
    public DateTime CreatedAt { get; set; }
}