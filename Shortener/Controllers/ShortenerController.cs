namespace Shortener.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ShortenerController(
    ShortenerDbContext dbContext,
    IShortenService shortenService) : ControllerBase
{
    [HttpPost("shorten")]
    public async Task<IActionResult> ShortenUrl([FromQuery] string longUrl, CancellationToken cancellationToken)
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
        var request = new RedirectRequest
        {
            Code = code
        };

        var shortenedUrl = await dbContext.Urls
            .SingleOrDefaultAsync(s => s.ShortCode == request.Code, cancellationToken);

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
    public async Task<ActionResult<List<UrlResponse>>> GetUrls(CancellationToken cancellationToken)
    {
        var urls = await dbContext.Urls
            .Select(x => new UrlResponse(x.LongUrl, x.ShortCode, x.CreatedAt, x.ExpiresAt))
            .ToListAsync(cancellationToken);

        return Ok(urls);
    }
}