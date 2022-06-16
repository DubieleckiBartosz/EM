using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EventManagement.Application.Models.Authorization
{
    public class AuthenticationModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        [JsonIgnore] public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}