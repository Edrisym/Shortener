using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace blink.Common.Models;

public class BaseEntity<T> : ITrackableEntity
{
    protected BaseEntity()
    {
    }

    [BsonId]
    [MaxLength(64)]
    [BsonRepresentation(BsonType.ObjectId)]
    public T Id { get; protected set; }
    [MaxLength(64)] public string? CreatedBy { get; set; }
    [MaxLength(64)] public string? ModifiedBy { get; set; }
    [Required] public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public DateTime? RemovedAt { get; set; }
    public string? RemovedBy { get; set; }
}