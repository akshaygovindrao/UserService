using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Features.Users.GetAllUsers;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET api/users?cursor=&pageSize=
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? cursor, [FromQuery] int pageSize = 50)
    {
        var clampedPageSize = Math.Clamp(pageSize, 1, 200);
        var response = await _mediator.Send(new GetAllUsersQuery(cursor, clampedPageSize));
        return Ok(response);
    }
}
