using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication1.Models;
using WebApplication1.Services;
using FluentValidation;

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
    

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(Guid id)
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
        // Ensure ID is handled by the service layer
        user.Id = Guid.Empty; // Clear any user-provided ID
        _logger.LogInformation("Creating new user: {@UserData}", user);
        try
        {
            var createdUser = await _userService.CreateUserAsync(user);
            _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {Errors}", ex.Errors);
            return BadRequest(ex.Errors.Select(e => e.ErrorMessage));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, User user)
    {
        _logger.LogInformation("Updating user with ID: {UserId}. New data: {@UserData}", id, user);
        var existingUser = await _userService.GetUserByIdAsync(id);
        if (existingUser == null)
        {
            _logger.LogWarning("Update failed. User with ID: {UserId} not found", id);
            return NotFound();
        }

        try
        {
            await _userService.UpdateUserAsync(id, user);
            _logger.LogInformation("User with ID: {UserId} updated successfully", id);
            return NoContent();
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {Errors}", ex.Errors);
            return BadRequest(ex.Errors.Select(e => e.ErrorMessage));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
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
    
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        var (users, totalCount) = await _userService.GetUsersAsync(pageIndex, pageSize);

        var response = new
        {
            TotalCount = totalCount,
            PageIndex = pageIndex,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Users = users
        };

        return Ok(response);
    }
}