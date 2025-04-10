using StackExchange.Redis;

namespace blink.Controllers.Dev;

[ApiController]
[Route("api/v1/[controller]")]
//TODO
// [Authorize(Roles = "Developer")]
public class DevController(IConnectionMultiplexer redis) : ControllerBase
{
    [HttpPost("clear-cache")]
    public async Task<IActionResult> ClearCacheAsync()
    {
        try
        {
            await redis.GetDatabase().ExecuteAsync("FLUSHDB");
            return Ok("Cache cleared successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while clearing the cache: {ex}");
        }
    }
}