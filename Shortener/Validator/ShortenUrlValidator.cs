using FluentValidation;
using Shortener.Common;

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
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}