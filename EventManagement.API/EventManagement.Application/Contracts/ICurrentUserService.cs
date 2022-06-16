using System.Collections.Generic;
using System.Security.Claims;

namespace EventManagement.Application.Contracts
{
    public interface ICurrentUserService
    {
        ClaimsPrincipal User { get; }
        string UserId { get; }
        string UserMail { get; }
        List<string> UserRoles { get; }
        bool IsAdmin();
        bool IsOwner();
        void CheckUserPermission(string userId);
        bool HasSuperAccess() => this.IsAdmin() || this.IsOwner();
    }
}
