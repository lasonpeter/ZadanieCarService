using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(User user);
    Task UpdateUserAsync(int id, User user);
    Task DeleteUserAsync(int id);
}