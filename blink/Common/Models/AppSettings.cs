namespace blink.Common.Models;

public class AppSettings
{
    public DatabaseSettings DatabaseSettings { get; set; }
    public UrlSettings UrlSettings { get; set; }
    public MyRateLimitSettings MyRateLimitSettings { get; set; }
}

public class DatabaseSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public Collections Collections { get; set; }
    public Redis Redis { get; set; }
}

public class Redis
{
    public string Configuration { get; set; }
    public string InstanceName { get; set; }
}

public class Collections
{
    public string ShortenerCollection { get; set; }
}

public class MyRateLimitSettings
{
    public int PermitLimit { get; set; }
    public int Window { get; set; }
    public int SegmentsPerWindow { get; set; }
    public int QueueLimit { get; set; }
}

public class UrlSettings
{
    public BaseUrls BaseUrls { get; set; }

    //TODO: this is for local
    public string Endpoint { get; set; }
}

public class BaseUrls
{
    public string Local { get; set; }
    public string Gateway { get; set; }
}