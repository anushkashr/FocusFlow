using FocusFlow.Api.Auth;
using FocusFlow.Shared.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FocusFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    // POST api/auth/login
    [HttpPost("login")]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await authService.LoginAsync(request);
        return response is null
            ? Unauthorized(new ProblemDetails { Title = "Wrong username or password" })
            : Ok(response);
    }
}
