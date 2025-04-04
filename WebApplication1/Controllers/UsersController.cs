using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        _logger.LogInformation("Getting all users");
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        _logger.LogInformation("Getting user with ID: {UserId}", id);
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            _logger.LogWarning("User not found with ID: {UserId}", id);
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        _logger.LogInformation("Creating new user: {@UserData}", user);
        var createdUser = await _userService.CreateUserAsync(user);
        _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);
        return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
        _logger.LogInformation("Updating user with ID: {UserId}. New data: {@UserData}", id, user);
        var existingUser = await _userService.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            _logger.LogWarning("Update failed. User with ID: {UserId} not found", id);
            return NotFound();
        }

        await _userService.UpdateUserAsync(id, user);
        _logger.LogInformation("User with ID: {UserId} updated successfully", id);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        _logger.LogInformation("Attempting to delete user with ID: {UserId}", id);
        var existingUser = await _userService.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            _logger.LogWarning("Delete failed. User with ID: {UserId} not found", id);
            return NotFound();
        }

        await _userService.DeleteUserAsync(id);
        _logger.LogInformation("User with ID: {UserId} deleted successfully", id);
        return NoContent();
    }
}