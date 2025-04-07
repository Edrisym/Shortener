using Microsoft.AspNetCore.Authorization;

namespace Shortener.Controllers.Admin;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin")]
public partial class ShortenerController : ControllerBase
{
}