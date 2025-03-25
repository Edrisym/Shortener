using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.EntityFrameworkCore;

namespace Shortener.Common.Models;

[Collection("Url")]
public class Url
{
    public ObjectId Id { get; set; }
    public string LongUrl { get; set; }
    public string ShortCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}