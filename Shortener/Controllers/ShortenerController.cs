using Shortener.Services;

namespace Shortener.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ShortenerController(
    ShortenerDbContext dbContext,
    IShortenService shortenService) : ControllerBase
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
        var shortenedUrl = await shortenService.RedirectToUrl(
            new RedirectRequest
            {
                Code = code
            },
            cancellationToken);

        if (shortenedUrl is null)
            return NotFound("No URL is found");

        if (DateTime.UtcNow > shortenedUrl.ExpiresAt)
            return UnprocessableEntity("The Code is Expired. Try shortening the URL again.");

#pragma warning disable SYSLIB0013
        // todo: find an alternative
        var url = Uri.EscapeUriString(shortenedUrl.LongUrl);
#pragma warning restore SYSLIB0013

        return Redirect(url);
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