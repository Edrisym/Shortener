namespace blink.Controllers.User.DTOs.Requests;

public class ShortenUrlRequest : IValidatableObject
{
    public string LongUrl { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!LongUrl.IsUrlValid())
            throw new InvalidOperationException(
                "The provided longUrl is not valid. Please ensure it is a properly formatted URL.",
                new Exception(nameof(LongUrl)));

        yield break;
    }
}