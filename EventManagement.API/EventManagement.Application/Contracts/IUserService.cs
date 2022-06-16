using System.Threading.Tasks;
using EventManagement.Application.Models.Authorization;
using EventManagement.Application.Wrappers;

namespace EventManagement.Application.Contracts
{
    public interface IUserService
    {
        Task<Response<string>> RegisterAsync(RegisterModel registerModel);
        Task<Response<AuthenticationModel>> LoginAsync(LoginModel registerModel);
        Task<Response<string>> AddToRoleAsync(UserAddToRoleModel userAddToRole);
        Task<Response<AuthenticationModel>> RefreshTokenAsync(string refreshTokenKey);
        Task<Response<string>> RevokeTokenAsync(string tokenKey);
        Task<Response<UserCurrentIFullInfo>> GetCurrentUserInfoAsync(string token);
    }
}
