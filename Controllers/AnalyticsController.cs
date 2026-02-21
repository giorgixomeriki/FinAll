using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FinancialProfileManagerAPI.Models;
using FinancialProfileManagerAPI.Services;

namespace FinancialProfileManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly UserProfileService _service;

    public AnalyticsController(UserProfileService service)
    {
        _service = service;
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(Analytics), StatusCodes.Status200OK)]
    public IActionResult GetAnalyticsSummary()
    {
        try
        {
            var analytics = _service.GetAnalytics();
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
        }
    }
}
