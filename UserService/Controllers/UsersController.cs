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
    #region Fields

    private readonly IMediator _mediator;

    #endregion

    #region Constructors

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    #endregion

    #region Public Methods

    // GET api/users?cursor=&pageSize=
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? cursor, [FromQuery] int pageSize = 50)
    {
        var clampedPageSize = Math.Clamp(pageSize, 1, 200);
        var response = await _mediator.Send(new GetAllUsersQuery(cursor, clampedPageSize));
        return Ok(response);
    }

    #endregion
}
