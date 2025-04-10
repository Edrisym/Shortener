namespace Shortener.Common.Models;

public interface ITrackableEntity
{
    string? CreatedBy { get; set; }
    string? ModifiedBy { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime? ModifiedAt { get; set; }
}