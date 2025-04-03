using FluentValidation;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<User> _validator;

    public UserService(IUserRepository userRepository, IValidator<User> validator)
    {
        _userRepository = userRepository;
        _validator = validator;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User> CreateUserAsync(User user)
    {
        var validationResult = await _validator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        return await _userRepository.CreateAsync(user);
    }

    public async Task UpdateUserAsync(int id, User user)
    {
        var validationResult = await _validator.ValidateAsync(user);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        user.Id = id;
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        await _userRepository.DeleteAsync(id);
    }
}