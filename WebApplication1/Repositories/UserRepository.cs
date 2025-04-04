using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all users.");
                throw;
            }
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the user with ID {UserId}.", id);
                throw;
            }
        }

        public async Task<User> CreateAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new user.");
                throw;
            }
        }

        public async Task UpdateAsync(User user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user with ID {UserId}.", user.Id);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user with ID {UserId}.", id);
                throw;
            }
        }

        public bool ExistsByEmail(string email)
        {
            try
            {
                return _context.Users.Any(u => u.Email == email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if a user exists with email {Email}.", email);
                throw;
            }
        }

        public bool ExistsByPhoneNumber(string phoneNumber)
        {
            try
            {
                return _context.Users.Any(u => u.PhoneNumber == phoneNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if a user exists with phone number {PhoneNumber}.", phoneNumber);
                throw;
            }
        }

        public async Task<(IEnumerable<User> Users, int TotalCount)> GetUsersAsync(int pageIndex, int pageSize)
        {
            try
            {
                var totalCount = await _context.Users.CountAsync();
                var users = await _context.Users
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return (users, totalCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving paginated users.");
                throw;
            }
        }
    }
}