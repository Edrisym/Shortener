namespace Shortener.Common.Models;

public class AppSettings
{
    public int HashParts { get; init; }
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public string BaseUrl { get; set; }
}