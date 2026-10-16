using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        var result = await _userService.GetUser();

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        var result = await _userService.UpdateUser(request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }

    [HttpPut("password")]
    public async Task<IActionResult> UpdateUserPassword([FromBody] UpdatePasswordRequest request)
    {
        var result = await _userService.UpdateUserPassword(request);

        if (!result.IsSuccess)
            return BadRequest(new { message = result.Error });

        return Ok(result);
    }
}