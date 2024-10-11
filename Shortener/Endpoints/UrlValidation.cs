using System.Text.RegularExpressions;

namespace Shortener.Endpoints;

public static class UrlValidation
{
    public static bool IsValid(this string? value)
    {
        if (value != null || !string.IsNullOrEmpty(value))
        {
            return false;
        }

        var urlPattern = @"^(https?:\/\/)?((localhost(:\d{1,5})?)|([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,})(\/[^\s]*)?$";
        return Regex.IsMatch(value!, urlPattern);
    }
}