using WebApplication1.Models;

namespace WebApplication1.Repositories;

public interface  IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User> CreateAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
    bool ExistsByEmail(string email);
    bool ExistsByPhoneNumber(string email);
}