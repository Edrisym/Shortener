namespace blink.Controllers.User.DTOs.Responses;

public sealed class UrlAnalyticResponse
{
    public string OriginalUrl { get; set; }
    public string ShortCode { get; set; }
    public string CraatedBy { get; set; }
    public IReadOnlyCollection<string> ViewedBy { get; set; }
    public int VisitCount { get; set; }
}