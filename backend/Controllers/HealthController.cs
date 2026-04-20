using CineStream.Models;
using Microsoft.AspNetCore.Mvc;

namespace CineStream.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet]
    public ActionResult<HealthResponse> Get()
    {
        return Ok(new HealthResponse
        {
            Status = "healthy",
            Message = "API is running"
        });
    }
}

[ApiController]
[Route("")]
public class RootController : ControllerBase
{
    /// <summary>
    /// Root endpoint
    /// </summary>
    [HttpGet("/")]
    public ActionResult<object> Get()
    {
        return Ok(new { message = "Cine Stream API is running!" });
    }
}
