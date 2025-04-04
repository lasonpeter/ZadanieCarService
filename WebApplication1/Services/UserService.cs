using FluentValidation;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<User> _validator;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, IValidator<User> validator, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public Task<(IEnumerable<User> Users, int TotalCount)> GetUsersAsync(int pageIndex, int pageSize)
    {
        return _userRepository.GetUsersAsync(pageIndex, pageSize);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = Guid.NewGuid(),
            ["Operation"] = "CreateUser"
        });

        // Ensure a new ID is generated server-side
        user.Id = Guid.NewGuid();
        _logger.LogInformation("Starting validation for user creation: {@UserProperties}", 
            new { user.FirstName, user.LastName, user.Email, user.PhoneNumber });

        var validationResult = await _validator.ValidateAsync(user);
        
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                _logger.LogError("Validation error for property {Property}: {Error}, Attempted value: {Value}", 
                    error.PropertyName, error.ErrorMessage, error.AttemptedValue);
            }
            _logger.LogError("User validation failed. Total validation errors: {ErrorCount}", validationResult.Errors.Count);
            throw new ValidationException(validationResult.Errors);
        }

        _logger.LogInformation("User validation successful, proceeding with creation");
        var createdUser = await _userRepository.CreateAsync(user);
        _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);
        
        return createdUser;
    }

    public async Task UpdateUserAsync(Guid id, User user)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = Guid.NewGuid(),
            ["Operation"] = "UpdateUser",
            ["UserId"] = id
        });

        _logger.LogInformation("Starting validation for user update. ID: {UserId}", id);

        var validationResult = await _validator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                _logger.LogError("Validation error for property {Property}: {Error}, Attempted value: {Value}", 
                    error.PropertyName, error.ErrorMessage, error.AttemptedValue);
            }
            _logger.LogError("User validation failed for update. Total validation errors: {ErrorCount}", validationResult.Errors.Count);
            throw new ValidationException(validationResult.Errors);
        }

        _logger.LogInformation("User validation successful, proceeding with update");
        await _userRepository.UpdateAsync(user);
        _logger.LogInformation("User updated successfully");
    }

    public async Task DeleteUserAsync(Guid id)
    {
        await _userRepository.DeleteAsync(id);
    }
}