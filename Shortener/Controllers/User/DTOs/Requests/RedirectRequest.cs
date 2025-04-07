namespace Shortener.Controllers.User.DTOs.Requests;

public class RedirectRequest : IValidatableObject
{
    public string Code { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        //TODO: write better message
        if (string.IsNullOrWhiteSpace(Code))
            throw new InvalidOperationException("Invalid Code!, try again.");
        yield break;
    }
}