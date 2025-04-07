using StackExchange.Redis;

namespace Shortener.Controllers.Dev;

[ApiController]
[Route("api/v1/[controller]")]
//TODO
// [Authorize(Roles = "Developer")]
public class DevController(IConnectionMultiplexer redis) : ControllerBase
{
    private readonly IDatabase _redisDatabase = redis.GetDatabase();

    [HttpPost("clear-cache")]
    public async Task<IActionResult> ClearCacheAsync()
    {
        try
        {
            var endpoints = _redisDatabase.Multiplexer.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = _redisDatabase.Multiplexer.GetServer(endpoint);
                await server.FlushAllDatabasesAsync();
            }

            return Ok("Cache cleared successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while clearing the cache: {ex}");
        }
    }
}