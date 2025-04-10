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
    Task<int> GetVisitCounter(IEnumerable<string> codes);
}

public class RedisCacheService(IConnectionMultiplexer redis) : IRedisCacheService
{
    private const string VisitCounter = "visit_counter_{0}";
    private readonly IDatabase _db = redis.GetDatabase();


    public async Task SetCacheValueAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiration, flags: CommandFlags.FireAndForget);
        await _db.KeyPersistAsync(string.Format(VisitCounter, key), flags: CommandFlags.FireAndForget);
    }

    public async Task<T?> GetCacheValueAsync<T>(
        string key)
    {
        var json = await _db.StringGetAsync(key);
        return json.HasValue ? JsonSerializer.Deserialize<T>(json!) : default;
    }

    public async Task PersistAndIncrementCounterAsync(
        string key)
    {
        try
        {
            await _db.StringIncrementAsync(string.Format(VisitCounter, key), flags: CommandFlags.FireAndForget);
        }
        catch (RedisException ex)
        {
            Console.WriteLine($"Error persisting and incrementing counter: {ex.Message}");
            throw;
        }
    }

    public async Task<int> GetVisitCounter(IEnumerable<string> codes)
    {
        var redisKeys = codes.Select(code =>
            (Code: code, RedisKey: $"visit_counter_{code}")).ToList();

        var redisTasks = redisKeys.Select(k =>
            _db.StringGetAsync(k.RedisKey)
                .ContinueWith(t => new { k.Code, Count = (int?)t.Result ?? 0 })).ToArray();
        var results = await Task.WhenAll(redisTasks);

        return results.Sum(r => r.Count);
    }
}