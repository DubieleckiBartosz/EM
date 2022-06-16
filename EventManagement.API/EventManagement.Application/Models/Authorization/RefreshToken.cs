using System;

namespace EventManagement.Application.Models.Authorization
{
    public class RefreshTokenBase
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string ReplacedByToken { get; set; }
        public string Revoked { get; set; }
    }

    public class RefreshToken : RefreshTokenBase
    {
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => Revoked == null && !IsExpired;

        public RefreshTokenBase MapToBase()
        {
            return new RefreshTokenBase
            {
                Token = this.Token,
                Expires = this.Expires,
                Created = this.Created,
                Revoked = this.Revoked,
                ReplacedByToken = this.ReplacedByToken
            };
        }
    }
}