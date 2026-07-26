using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Dtos;
using UserService.Features.Authentication.Login;
using UserService.Features.Authentication.Logout;
using UserService.Features.Authentication.Register;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST api/authentication/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var response = await _mediator.Send(new RegisterCommand(request.Email, request.Password));
        return Ok(response);
    }

    // POST api/authentication/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _mediator.Send(new LoginCommand(request.Email, request.Password));
        return Ok(response);
    }

    // POST api/authentication/logout
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _mediator.Send(new LogoutCommand());
        return NoContent();
    }
}
