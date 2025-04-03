using Shortener.Services;

namespace Shortener.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ShortenerController(IShortenService shortenService) : ControllerBase
{
    [HttpPost("shorten")]
    public async Task<IActionResult> ShortenUrl(
        [FromQuery] string longUrl,
        CancellationToken cancellationToken)
    {
        var request = new ShortenUrlRequest
        {
            LongUrl = longUrl
        };

        var result = await shortenService.ToShortUrl(request, cancellationToken);
        return Ok(new { LongUrl = result });
    }

    [HttpGet("redirect")]
    public async Task<IActionResult> RedirectToUrl(
        [FromQuery] string code,
        CancellationToken cancellationToken)
    {
        var longUrl = await shortenService.RedirectToUrl(
            new RedirectRequest
            {
                Code = code
            },
            cancellationToken);
        
        if (longUrl is null)
            return NotFound("No URL is found");

        return Redirect(longUrl!);
    }

    [HttpGet]
    public async Task<List<UrlResponse>> GetUrls(
        CancellationToken cancellationToken)
    {
        return await shortenService.GetUrls(cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<UrlResponse?> GetUrl(
        string id,
        CancellationToken cancellationToken)
    {
        return await shortenService.GetUrl(id, cancellationToken);
    }
}