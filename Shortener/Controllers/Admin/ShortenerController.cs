using Microsoft.AspNetCore.Authorization;

namespace Shortener.Controllers.Admin;

[ApiController]
[Route("api/v1/admin/[controller]")]
[Authorize(Roles = "Admin")]
public partial class DashboardShortenerController() : ControllerBase
{
    
}