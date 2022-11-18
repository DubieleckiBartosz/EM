using System;
using System.Threading.Tasks;
using EventManagement.Application.Contracts;
using EventManagement.Application.Models.Authorization;
using EventManagement.Application.Strings;
using EventManagement.Application.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EventManagement.API.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<UserCurrentIFullInfo>), 200)]
        [SwaggerOperation(Summary = "Get info about current user")]
        [Authorize]
        [HttpGet("[action]")]
        public async Task<IActionResult> CurrentUserInfo()
        {
            var refreshToken = Request.Cookies[Constants.CookieRefreshToken];
            var response = await this._userService.GetCurrentUserInfoAsync(refreshToken);
            return Ok(response);
        }

        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Register user")]
        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            return Ok(await this._userService.RegisterAsync(registerModel));
        }

        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<AuthenticationModel>), 200)]
        [SwaggerOperation(Summary = "User login")]
        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var response = await this._userService.LoginAsync(loginModel);
            this.SetRefreshTokenInCookie(response.Data.RefreshToken);
            return Ok(response);
        }

        [ProducesResponseType(401)]
        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Add user to new role")]
        [HttpPost("[action]")]
        public async Task<IActionResult> AddUserToRole([FromBody] UserAddToRoleModel newRoleForUserModel)
        {
            return Ok(await this._userService.AddToRoleAsync(newRoleForUserModel));
        }

        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<AuthenticationModel>), 200)]
        [SwaggerOperation(Summary = "Refresh token")]
        [HttpPost("[action]")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies[Constants.CookieRefreshToken];
            var response = await _userService.RefreshTokenAsync(refreshToken);
            if (!string.IsNullOrEmpty(response.Data.RefreshToken))
            {
                this.SetRefreshTokenInCookie(response.Data.RefreshToken);
            }

            return Ok(response);
        }

        [ProducesResponseType(typeof(Response<string>), 400)]
        [ProducesResponseType(typeof(Response<string>), 200)]
        [SwaggerOperation(Summary = "Revoke token")]
        [HttpPost("[action]")]
        public async Task<IActionResult> RevokeToken()
        {
            var result = await _userService.RevokeTokenAsync(Request.Cookies[Constants.CookieRefreshToken]);
            Response.Cookies.Delete(Constants.CookieRefreshToken);
            return Ok(result);
        }

        private void SetRefreshTokenInCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(5),
                IsEssential = true,
                SameSite = SameSiteMode.None,
                Secure = true,
            };
            Response.Cookies.Append(Constants.CookieRefreshToken, refreshToken, cookieOptions);
        }
    }
}