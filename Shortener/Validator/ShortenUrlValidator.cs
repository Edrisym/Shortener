using System.Text.RegularExpressions;

namespace Shortener.Validator;

public class ShortenUrlValidator : AbstractValidator<ShortenUrl>
{
    public ShortenUrlValidator()
    {
        RuleFor(x => x.LongUrl)
            .NotEmpty().WithMessage("URL cannot be empty.")
            .Must(IsValidUrl).WithMessage("Invalid URL format.");
    }

    private bool IsValidUrl(string url)
    {
        var urlPattern = @"^(https?:\/\/)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/[^\s]*)?$";
        return Regex.IsMatch(url, urlPattern);
    }
}