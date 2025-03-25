namespace Shortener.Common.Models;

public class AppSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public Collections Collections { get; set; }
    public int HashParts { get; set; }
    public string BaseUrl { get; set; }
}

public class Collections
{
    public string shortenerCollection { get; set; }
}