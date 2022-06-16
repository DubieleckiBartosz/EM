using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using EventManagement.Application.Contracts;
using EventManagement.Application.Models.Authorization;
using EventManagement.Application.Settings;
using EventManagement.Application.Wrappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using EventManagement.Application.Contracts.Repositories;
using EventManagement.Application.Exceptions;
using EventManagement.Application.Helpers;
using EventManagement.Application.Models.Enums.Auth;
using EventManagement.Application.Strings.Responses;
using Microsoft.IdentityModel.Tokens;

namespace EventManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager<UserService> _loggerManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JWTSettings _jwtSettings;

        public UserService(IUserRepository userRepository, ILoggerManager<UserService> loggerManager,
            IPasswordHasher<User> passwordHasher, IOptions<JWTSettings> jwtSettings)
        {
            this._userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this._loggerManager = loggerManager ?? throw new ArgumentNullException(nameof(loggerManager));
            this._passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            this._jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
        }

        public async Task<Response<string>> AddToRoleAsync(UserAddToRoleModel userAddToRole)
        {
            if (userAddToRole == null)
            {
                throw new ArgumentNullException(nameof(userAddToRole));
            }

            var user = await this._userRepository.FindByEmailAsync(userAddToRole.Email);
            if (user == null)
            {
                throw new AuthException(ResponseStrings.UserNotFound);
            }

            var allRoles = EnumHelpers.GetStringValuesFromEnum<Roles>();
            var newRole = allRoles.FirstOrDefault(_ =>
                _.ToLower() == userAddToRole.Role.ToLower());

            if (newRole == null)
            {
                throw new AuthException(ResponseStrings.RoleNotFound(allRoles));
            }

            var currentRoles = await _userRepository.GetUserRolesAsync(user);
            var userHas = currentRoles.Any(_ => _.ToLower() == userAddToRole.Role.ToLower());
            if (userHas)
            {
                throw new AuthException(ResponseStrings.UserAlreadyHasRole);
            }

            var success = await this._userRepository.AddToRoleAsync(user.Id, (int) newRole.ToEnum<Roles>());
            if (!success)
            {
                throw new AuthException(ResponseStrings.NewRoleForUserFailed);
            }

            return Response<string>.Ok(ResponseStrings.OperationSuccess);
        }

        public async Task<Response<AuthenticationModel>> LoginAsync(LoginModel loginModel)
        {
            if (loginModel == null)
            {
                throw new ArgumentNullException(nameof(loginModel));
            }


            var user = await this._userRepository.FindByEmailAsync(loginModel.Email);
            if (user == null)
            {
                throw new AuthException(ResponseStrings.IncorrectData);
            }

            var verificationResult =
                this._passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginModel.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                throw new AuthException(ResponseStrings.IncorrectCredentials(loginModel.Email));
            }

            var authenticationModel = new AuthenticationModel();
            var roles = await _userRepository.GetUserRolesAsync(user);
            var jwtSecurityToken = this.CreateJwtToken(user, roles);
            authenticationModel.Token = jwtSecurityToken;
            authenticationModel.Email = user.Email;
            authenticationModel.UserName = user.UserName;
            authenticationModel.Roles = roles;

            var activeRefreshToken = user.RefreshTokens?.FirstOrDefault(_ => _.IsActive);
            if (activeRefreshToken == null)
            {
                var refreshToken = this.CreateRefreshToken(user);
                authenticationModel.RefreshToken = refreshToken.Token;
                authenticationModel.RefreshTokenExpiration = refreshToken.Expires;
                if (user.RefreshTokens == null)
                {
                    user.RefreshTokens = new List<RefreshToken>();
                }

                user.RefreshTokens.Add(refreshToken);
                await this._userRepository.UpdateAsync(user);
            }
            else
            {
                authenticationModel.RefreshToken = activeRefreshToken.Token;
                authenticationModel.RefreshTokenExpiration = activeRefreshToken.Expires;
            }

            return Response<AuthenticationModel>.Ok(authenticationModel);
        }

        public async Task<Response<string>> RegisterAsync(RegisterModel registerModel)
        {
            if (registerModel == null)
            {
                throw new ArgumentNullException(nameof(registerModel));
            }

            var user = await this._userRepository.FindByEmailAsync(registerModel.Email);

            if (user != null)
            {
                throw new AuthException(ResponseStrings.UserExist(registerModel.Email));
            }

            var newUser = new User()
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                UserName = registerModel.UserName,
                Email = registerModel.Email,
                PhoneNumber = registerModel.PhoneNumber
            };
            newUser.PasswordHash = this._passwordHasher.HashPassword(newUser, registerModel.Password);

            try
            {
                var identifier = await this._userRepository.CreateAsync(newUser);
                if (identifier == null)
                {
                    throw new AuthException(ResponseStrings.RegisterFailed);
                }

                newUser.Id = identifier.Value;
                var roleCreatedForUser =
                    await this._userRepository.AddToRoleAsync(newUser.Id, (int) Roles.User);
                if (!roleCreatedForUser)
                {
                    throw new AuthException(ResponseStrings.RegisterFailed);
                }


                return Response<string>.Ok(ResponseStrings.RegisterSuccess);
            }
            catch
            {
                this._loggerManager.LogError(new
                {
                    Message = "Attempt to create a user has failed.",
                    UserMail = registerModel.Email
                });

                throw;
            }
        }


        public async Task<Response<AuthenticationModel>> RefreshTokenAsync(string refreshTokenKey)
        {
            var user = await this._userRepository.FindUserByTokenAsync(refreshTokenKey);
            if (user == null)
            {
                throw new AuthException(ResponseStrings.TokenNotMatch);
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == refreshTokenKey);
            if (!refreshToken.IsActive)
            {
                throw new AuthException(ResponseStrings.TokenNotActive);
            }

            refreshToken.Revoked = DateTime.UtcNow.ToLongTimeString();
            var newRefreshToken = CreateRefreshToken(user);
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);

            await this._userRepository.UpdateAsync(user);
            var roles = await _userRepository.GetUserRolesAsync(user);
            var jwtSecurityToken = this.CreateJwtToken(user, roles);

            var responseModel = this.AuthenticationModelMap(jwtSecurityToken, user.Email,
                user.UserName, newRefreshToken.Token, roles,
                newRefreshToken.Expires);

            return Response<AuthenticationModel>.Ok(responseModel);
        }


        public async Task<Response<string>> RevokeTokenAsync(string tokenKey)
        {
            if (string.IsNullOrEmpty(tokenKey))
            {
                throw new AuthException(ResponseStrings.TokenIsEmptyOrNull);
            }

            var user = await this._userRepository.FindUserByTokenAsync(tokenKey);
            if (user == null)
            {
                throw new AuthException(ResponseStrings.TokenNotMatch);
            }

            var refreshToken = user.RefreshTokens.Single(x => x.Token == tokenKey);
            if (!refreshToken.IsActive)
            {
                throw new AuthException(ResponseStrings.TokenNotActive);
            }

            refreshToken.Revoked = DateTime.UtcNow.ToLongTimeString();
            await this._userRepository.UpdateAsync(user);

            return Response<string>.Ok(ResponseStrings.OperationSuccess);
        }

        public async Task<Response<UserCurrentIFullInfo>> GetCurrentUserInfoAsync(string token)
        {
            var user = await this._userRepository.FindUserByTokenAsync(token);
            if (user == null)
            {
                throw new AuthException(ResponseStrings.TokenNotMatch);
            }

            var roles = await this._userRepository.GetUserRolesAsync(user);

            var modelResponse = new UserCurrentIFullInfo()
            {
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Roles = roles
            };

            return Response<UserCurrentIFullInfo>.Ok(modelResponse);
        }


        #region private

        private AuthenticationModel AuthenticationModelMap(string token, string email, string userName,
            string refreshToken, List<string> roles, DateTime refreshTokenExpiration)
        {
            var authenticationModel = new AuthenticationModel()
            {
                Token = token,
                Email = email,
                UserName = userName,
                Roles = roles,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiration
            };
            return authenticationModel;
        }

        private RefreshToken CreateRefreshToken(User user)
        {
            var refreshToken = this._passwordHasher.HashPassword(user, Guid.NewGuid().ToString())
                .Replace("+", string.Empty)
                .Replace("=", string.Empty)
                .Replace("/", string.Empty);
            return new RefreshToken()
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(5),
                Created = DateTime.Now
            };
        }

        private string CreateJwtToken(User user, List<string> roles)
        {
            var roleClaims = new List<Claim>();
            roleClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList());
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, $"{user.FirstName}_{user.LastName}-{user.UserName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }.Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: this._jwtSettings.Issuer,
                audience: this._jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(this._jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
        #endregion
    }
}