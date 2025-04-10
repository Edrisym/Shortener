namespace blink.Common;

public abstract class GatewayHeaders
{
    public const string UserId = "X-Forwarded-UserId";
    public const string Agent = "X-Forwarded-Agent";
    public const string Referer = "X-Forwarded-Referer";
    public const string Ip = "X-Forwarded-IP";
    public const string GeoLocation = "X-Forwarded-Geo-Location";
    public const string ForwardedHost = "X-Forwarded-Host";
}