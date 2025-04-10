using System.Text.Json;
using StackExchange.Redis;

namespace blink.Services;

public interface IRedisCacheService
{
    Task SetCacheValueAsync<T>(string key,
        T value,
        TimeSpan? expiration = null);

    Task<T?> GetCacheValueAsync<T>(string key);

    Task PersistAndIncrementCounterAsync(string key);
}

public class RedisCacheService(IConnectionMultiplexer redis) : IRedisCacheService
{
    private const string VisitCounter = "visit_counter_{0}";

    public async Task SetCacheValueAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null)
    {
        var db = redis.GetDatabase();
        var json = JsonSerializer.Serialize(value);
        await db.StringSetAsync(key, json, expiration, flags: CommandFlags.FireAndForget);
        await db.KeyPersistAsync(string.Format(VisitCounter, key), flags: CommandFlags.FireAndForget);
    }

    public async Task<T?> GetCacheValueAsync<T>(
        string key)
    {
        var db = redis.GetDatabase();
        var json = await db.StringGetAsync(key);
        return json.HasValue ? JsonSerializer.Deserialize<T>(json!) : default;
    }

    public async Task PersistAndIncrementCounterAsync(
        string key)
    {
        var db = redis.GetDatabase();
        try
        {
            await db.StringIncrementAsync(string.Format(VisitCounter, key), flags: CommandFlags.FireAndForget);
        }
        catch (RedisException ex)
        {
            Console.WriteLine($"Error persisting and incrementing counter: {ex.Message}");
            throw;
        }
    }
}