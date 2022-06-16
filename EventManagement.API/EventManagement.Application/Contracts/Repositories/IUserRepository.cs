using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using EventManagement.Application.Models.Authorization;

namespace EventManagement.Application.Contracts.Repositories
{
    public interface IUserRepository
    {
        Task<User> FindUserByTokenAsync(string tokenKey);
        Task<User> FindByEmailAsync(string email);
        Task<int?> CreateAsync(User user);
        Task<bool> UpdateAsync(User user, IDbTransaction transaction = null);
        Task<bool> AddToRoleAsync(int userId, int roleId, IDbTransaction transaction = null);
        Task<List<string>> GetUserRolesAsync(User user);
        Task ClearTokens();
    }
}
