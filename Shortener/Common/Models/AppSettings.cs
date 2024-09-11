namespace Shortener.Common.Models;

public class AppSettings
{
    public int HashParts { get; init; }
    public string ConnectionString { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
    public string BaseUrl { get; set; }
}