using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using EventManagement.Application.Models.Enums.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace EM.IntegrationTests.Common
{
    public class FakePolicyEvaluator : IPolicyEvaluator
    {
        public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            var firstName = $"SuperUser_FirstName_Test";
            var lastName = $"SuperUser_LastName_Test";
            var userName = $"SuperUserTest";

            claimsPrincipal.AddIdentity(new ClaimsIdentity(new []
            {
                new Claim(ClaimTypes.Role, Roles.User.ToString()),
                new Claim(ClaimTypes.Role, Roles.Owner.ToString()),
                new Claim(ClaimTypes.Role, Roles.Performer.ToString()),
                new Claim(ClaimTypes.Role, Roles.Admin.ToString()),
                new Claim(ClaimTypes.Name, $"{firstName}_{lastName}-{userName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email,  "SuperUser@test.com"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));

            var authTicket = new AuthenticationTicket(claimsPrincipal, "ticketTest");
            var result = AuthenticateResult.Success(authTicket);
            return Task.FromResult(result);
        }

        public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
            AuthenticateResult authenticationResult, HttpContext context,
            object? resource)
        {
            var result = PolicyAuthorizationResult.Success();

            return Task.FromResult(result);
        }
    }
}