using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    
    Task<(IEnumerable<User> Users, int TotalCount)> GetUsersAsync(int pageIndex, int pageSize);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User> CreateUserAsync(User user);
    Task UpdateUserAsync(Guid id, User user);
    Task DeleteUserAsync(Guid id);
}