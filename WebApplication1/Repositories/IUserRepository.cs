using System;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(Guid id);
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);
        bool ExistsByEmail(string email);
        bool ExistsByPhoneNumber(string phoneNumber);
        Task<(IEnumerable<User> Users, int TotalCount)> GetUsersAsync(int pageIndex, int pageSize);
    }
}