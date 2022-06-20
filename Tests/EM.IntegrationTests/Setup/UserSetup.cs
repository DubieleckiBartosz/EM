using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EventManagement.Application.Models.Enums.Auth;

namespace EM.IntegrationTests.Setup
{
    public class UserSetup
    {
        public static ClaimsPrincipal UserPrincipals()
        {
            var claimsPrincipal = new ClaimsPrincipal();
            var firstName = $"SuperUser_FirstName_Test";
            var lastName = $"SuperUser_LastName_Test";
            var userName = $"SuperUserTest";

            claimsPrincipal.AddIdentity(new ClaimsIdentity(new[]
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

            return claimsPrincipal;
        }
    }
}
