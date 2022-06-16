using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using EventManagement.Application.Contracts;
using EventManagement.Application.Exceptions;
using EventManagement.Application.Models.Enums.Auth;
using EventManagement.Application.Strings.Responses;
using Microsoft.AspNetCore.Http;

namespace EventManagement.Application.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User =>
            _httpContextAccessor.HttpContext?.User;

        public string UserId => !User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier) || User == null
            ? null
            : User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        public string UserMail => !User.HasClaim(c => c.Type == ClaimTypes.Email) || User == null
            ? null
            : User.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;

        public List<string> UserRoles => User?.Claims.Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value).ToList();

        public bool IsAdmin() => User?.IsInRole(role: Roles.Admin.ToString()) ?? false;
        public bool IsOwner() => User?.IsInRole(role: Roles.Owner.ToString()) ?? false;
        public bool HasSuperAccess() => this.IsAdmin() || this.IsOwner();

        public void CheckUserPermission(string userId)
        {
            if ((UserId == null) || (userId != UserId && !this.IsAdmin() && !this.IsOwner()))
            {
                throw new EventManagementException(ResponseStrings.NoPermission);
            }
        }
    }
}