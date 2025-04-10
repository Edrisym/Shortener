using Shortener.Services;

namespace Shortener.Controllers.User;

[ApiController]
[Route("api/v1/shortener/[controller]")]
public class AnalyticsController(IAnalyticService service) : ControllerBase
{
    [HttpGet]
    public async Task<List<UrlAnalyticResponse>> GetUrls(
        [FromHeader(Name = "userId")] string userId,
        CancellationToken cancellationToken)
    {
        return await service.GetUrlAnalytics(new UrlAnalyticRequest
        {
            CreatedBy = userId
        }, cancellationToken);
    }

    // [HttpGet("{id}")]
    // public async Task<UrlResponse?> GetUrl(
    //     string id,
    //     CancellationToken cancellationToken)
    // {
    //     return await service.GetUrlAnalytic(id, cancellationToken);
    // }
}