using blink.Controllers.User.DTOs.Requests;
using blink.Controllers.User.DTOs.Responses;
using blink.Services;

namespace blink.Controllers.User;

[ApiController]
[Route("api/v1/blink/[controller]")]
public class AnalyticsController(IAnalyticService service) : ControllerBase
{
    [HttpGet]
    public async Task<List<UrlAnalyticResponse>> GetUrls(
        [FromHeader(Name = "userId")] string userId,
        CancellationToken cancellationToken)
    {
        return await service.GetUrlAnalytics(
            new UrlAnalyticRequest
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