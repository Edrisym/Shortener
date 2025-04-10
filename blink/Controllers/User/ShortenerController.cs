using blink.Controllers.User.DTOs.Requests;
using blink.Services;

namespace blink.Controllers.User;

[ApiController]
[Route("api/v1/blink/[controller]")]
public class ShortenerController(IShortenService shortenService) : ControllerBase
{
    [HttpPost("shorten")]
    public async Task<IActionResult> ShortenUrl(
        [FromBody] ShortenUrlRequest request,
        CancellationToken cancellationToken)
    {
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
}