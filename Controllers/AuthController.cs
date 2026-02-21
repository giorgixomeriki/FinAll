using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FinancialProfileManagerAPI.Services;
using FinancialProfileManagerAPI.Models;

namespace FinancialProfileManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public AuthController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            return BadRequest(new { message = "Username and password are required" });

        // Simple validation - in production, check against database
        if (request.Username == "admin" && request.Password == "password")
        {
            var token = _tokenService.GenerateToken("1", request.Username);
            return Ok(new LoginResponse { Token = token, Username = request.Username });
        }

        return BadRequest(new { message = "Invalid credentials" });
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; }
    public string Username { get; set; }
}
